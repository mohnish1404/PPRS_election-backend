using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ElectionEmployeeAPI.Models;
using ElectionEmployeeAPI.DTOs;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ElectionEmployeeAPI.Data;


namespace ElectionEmployeeAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ✅ Sabhi Pending Approvals dekho (with full user registration details)
        [HttpGet("pending-approvals")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var approvals = await (
                from a in _db.AdminApprovals
                join u in _db.Users on a.UserId equals u.UserId into userJoin
                from u in userJoin.DefaultIfEmpty()
                where a.Status == "Pending"
                orderby a.RequestedAt descending
                select new
                {
                    a.Id,
                    a.UserId,
                    a.RequestType,
                    a.Status,
                    a.RequestedAt,
                    a.ApprovedAt,
                    a.Remarks,
                    FullName = u != null ? u.FullName : null,
                    MobileNumber = u != null ? u.MobileNumber : null,
                    Email = u != null ? u.Email : null,
                    Role = u != null ? u.Role : null,
                    CreatedAt = u != null ? u.CreatedAt : (DateTime?)null,
                }
            ).ToListAsync();

            return Ok(new { success = true, data = approvals });
        }
        // ✅ Approval Approve karo
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var approval = await _db.AdminApprovals.FindAsync(id);
            if (approval == null)
                return NotFound(new { success = false, message = "Request not found" });

            approval.Status = "Approved";
            approval.ApprovedAt = DateTime.UtcNow;

            // User activate karo
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == approval.UserId);

            if (user != null)
            {
                user.IsApproved = true;
                user.ApprovedAt = DateTime.UtcNow;
                user.IsActive = true;
            }

            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Request approved successfully" });
        }

        // ✅ Approval Reject karo
        [HttpPost("reject/{id}")]
        public async Task<IActionResult> RejectRequest(int id, [FromBody] string remarks)
        {
            var approval = await _db.AdminApprovals.FindAsync(id);
            if (approval == null)
                return NotFound(new { success = false, message = "Request not found" });

            approval.Status = "Rejected";
            approval.ApprovedAt = DateTime.UtcNow;
            approval.Remarks = remarks;

            await _db.SaveChangesAsync();

            return Ok(new { success = true, message = "Request rejected" });
        }

        // ✅ Sabhi Users dekho
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _db.Users
                .Select(u => new {
                    u.Id,
                    u.UserId,
                    u.FullName,
                    u.MobileNumber,
                    u.Email,
                    u.IsActive,
                    u.IsApproved,
                    u.Role,
                    u.CreatedAt,
                    u.IsPasswordExpired
                })
                .ToListAsync();

            return Ok(new { success = true, data = users });
        }

        // ✅ User Activate/Deactivate karo
        [HttpPost("toggle-user/{userId}")]
        public async Task<IActionResult> ToggleUser(string userId)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return NotFound(new { success = false, message = "User not found" });

            user.IsActive = !user.IsActive;
            await _db.SaveChangesAsync();

            var status = user.IsActive ? "activated" : "deactivated";

            // ---- Audit log save karo ----
            var performedBy = User.FindFirst(
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "Unknown";

            var auditLog = new UserAuditLog
            {
                TargetUserId = userId,
                ActionType = user.IsActive ? "Activated" : "Deactivated",
                PerformedBy = performedBy,
                PerformedAt = DateTime.Now
            };
            _db.UserAuditLogs.Add(auditLog);
            await _db.SaveChangesAsync();
            // -------------------------------

            return Ok(new { success = true, message = $"User {status} successfully" });
        }
        // ========== DUTY REPORT ENDPOINTS ==========

        [HttpGet("assigned-duties")]
        public async Task<IActionResult> GetAssignedDuties(int? acNo = null)
        {
            var query = from team in _db.PollingTeams
                        join member in _db.PollingTeamMembers on team.TeamId equals member.TeamId
                        join emp in _db.PollingPersonnel on member.EmpCode equals emp.EmpCode
                        join dutyPost in _db.DutyPosts on member.DutyPostId equals dutyPost.DutyPostId
                        where emp.dflag == false
&& !_db.DutyExemptions.Any(ex => ex.MemberId == member.Id && ex.Status == "Active")
                        select new
                        {
                            team.TeamId,
                            MemberId = member.Id,
                            emp.EmpCode,
                            emp.EmpName,
                            emp.EmpName_En,
                            team.District_ID,
                            AC_No = team.AC_No,
                            Part_No = team.Part_No,
                            team.DutyDateTime,
                            DutyPost = dutyPost.DutyPostEnglish,
                            emp.Designation_Id,
                            emp.MobileNo
                        };

            if (acNo.HasValue && acNo.Value > 0)
                query = query.Where(x => x.AC_No == acNo.Value);

            var result = await query.OrderByDescending(x => x.DutyDateTime).ToListAsync();
            return Ok(result);
        }

        [HttpPost("remove-duty")]
        public async Task<IActionResult> RemoveDuty([FromBody] RemoveDutyRequest request)
        {
            if (request.MemberIds == null || !request.MemberIds.Any())
                return BadRequest("No member IDs provided.");

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var members = await _db.PollingTeamMembers
                    .Where(m => request.MemberIds.Contains(m.Id))
                    .ToListAsync();

                if (!members.Any())
                    return NotFound("No matching duty assignments found.");

                var teamIds = members.Select(m => m.TeamId).Distinct().ToList();

                // Log removal before deleting
                foreach (var member in members)
                {
                    var team = await _db.PollingTeams.FindAsync(member.TeamId);
                    var removalLog = new DutyRemovalLog
                    {
                        EmpCode = member.EmpCode ?? 0,
                        TeamId = member.TeamId,
                        AC_No = team?.AC_No,
                        Part_No = team?.Part_No,
                        RemovedBy = GetCurrentUserId(),
                        RemovedAt = DateTime.UtcNow,
                        Remarks = request.Remarks
                    };
                    _db.DutyRemovalLogs.Add(removalLog);
                }

                _db.PollingTeamMembers.RemoveRange(members);
                await _db.SaveChangesAsync();

                // Delete teams with no members left
                foreach (var teamId in teamIds)
                {
                    bool hasMembers = await _db.PollingTeamMembers.AnyAsync(m => m.TeamId == teamId);
                    if (!hasMembers)
                    {
                        var team = await _db.PollingTeams.FindAsync(teamId);
                        if (team != null)
                            _db.PollingTeams.Remove(team);
                    }
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = $"Successfully removed {members.Count} duty assignment(s)." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("recent-removals")]
        public async Task<IActionResult> GetRecentRemovals(int? acNo = null, int days = 30)
        {
            var cutoff = DateTime.UtcNow.AddDays(-days);
            var query = _db.DutyRemovalLogs.Where(l => l.RemovedAt >= cutoff);

            if (acNo.HasValue && acNo.Value > 0)
                query = query.Where(l => l.AC_No == acNo.Value);

            var result = await query
                .OrderByDescending(l => l.RemovedAt)
                .Select(l => new
                {
                    l.Id,
                    l.EmpCode,
                    EmployeeName = _db.PollingPersonnel
                        .Where(p => p.EmpCode == l.EmpCode)
                        .Select(p => p.EmpName)
                        .FirstOrDefault() ?? "Unknown",
                    l.AC_No,
                    l.Part_No,
                    l.RemovedAt,
                    l.Remarks,
                    RemovedBy = l.RemovedBy
                })
                .ToListAsync();

            return Ok(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int id))
                return id;
            return 0;
        }

        [HttpPost("exemptions")] 
        public async Task<IActionResult> GrantExemption([FromForm] int memberId, [FromForm] int reasonId, [FromForm] string? remarks, IFormFile document)
        {
            if (document == null || document.Length == 0)
                return BadRequest(new { message = "Supporting document is required to grant an exemption." });

            var allowedTypes = new[] { ".pdf", ".doc", ".docx" };
            var ext = Path.GetExtension(document.FileName).ToLower();
            if (!allowedTypes.Contains(ext))
                return BadRequest(new { message = "Only PDF, DOC, DOCX files allowed." });

            if (document.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "File size must be under 5MB." });

            var member = await _db.PollingTeamMembers.FindAsync(memberId);
            if (member == null)
                return NotFound(new { message = "Member not found." });

            var team = await _db.PollingTeams.FindAsync(member.TeamId);
            var emp = await _db.PollingPersonnel.FirstOrDefaultAsync(e => e.EmpCode == member.EmpCode);
            var dutyPost = await _db.DutyPosts.FindAsync(member.DutyPostId);

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "exemptions");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
                await document.CopyToAsync(stream);

            var exemption = new DutyExemption
            {
                MemberId = memberId,
                EmpCode = emp?.EmpCode.ToString() ?? "",
                AC_No = team?.AC_No ?? 0,
                Part_No = team?.Part_No ?? 0,
                DutyPost = dutyPost?.DutyPostEnglish,
                ReasonId = reasonId,
                Remarks = remarks,
                DocumentPath = $"/uploads/exemptions/{fileName}",
                DocumentOriginalName = document.FileName,
                ExemptedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin",
                ExemptionDateTime = DateTime.UtcNow,
                Status = "Active"
            };

            _db.DutyExemptions.Add(exemption);
            _db.PollingTeamMembers.Remove(member);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Duty exempted successfully.", exemptionId = exemption.ExemptionId });
        }

        [HttpGet("exemptions")]
        public async Task<IActionResult> GetExemptions(int? acNo = null)
        {
            var query = from ex in _db.DutyExemptions
                        join reason in _db.ExemptionReasons on ex.ReasonId equals reason.ReasonId
                        select new
                        {
                            ex.ExemptionId,
                            ex.MemberId,
                            ex.EmpCode,
                            ex.AC_No,
                            ex.Part_No,
                            ex.DutyPost,
                            ReasonText = reason.ReasonText,
                            ex.Remarks,
                            ex.DocumentPath,
                            ex.DocumentOriginalName,
                            ex.ExemptedBy,
                            ex.ExemptionDateTime,
                            ex.Status,
                            ex.RestoredBy,
                            ex.RestoredDateTime
                        };

            if (acNo.HasValue && acNo.Value > 0)
                query = query.Where(x => x.AC_No == acNo.Value);

            var result = await query
                .OrderByDescending(x => x.ExemptionDateTime)
                .ToListAsync();

            return Ok(result);
        }

        [HttpPost("exemptions/{id}/restore")]
        public async Task<IActionResult> RestoreExemption(int id)
        {
            var exemption = await _db.DutyExemptions.FindAsync(id);
            if (exemption == null)
                return NotFound(new { message = "Exemption record not found." });

            if (exemption.Status == "Restored")
                return BadRequest(new { message = "This exemption is already restored." });

            exemption.Status = "Restored";
            exemption.RestoredBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Admin";
            exemption.RestoredDateTime = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Ok(new { message = "Exemption restored successfully. Employee is now available in the pool." });
        }

        [HttpGet("exemption-reasons")]
        public async Task<IActionResult> GetExemptionReasons()
        {
            var reasons = await _db.ExemptionReasons
                .Where(r => r.IsActive)
                .OrderBy(r => r.ReasonId)
                .Select(r => new { r.ReasonId, r.ReasonText })
                .ToListAsync();

            return Ok(reasons);
        }

        [HttpGet("audit-logs/{userId}")]
        public async Task<IActionResult> GetUserAuditLogs(string userId)
        {
            var logs = await _db.UserAuditLogs
                .Where(l => l.TargetUserId == userId)
                .OrderByDescending(l => l.PerformedAt)
                .ToListAsync();

            return Ok(logs);
        }


    }
}