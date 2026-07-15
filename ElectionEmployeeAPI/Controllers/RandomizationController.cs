using ElectionEmployeeAPI.Data;
using ElectionEmployeeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class RandomizationController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RandomizationController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("summary")]
    public IActionResult GetSummary(int acNo, bool pollingExp = false,
    bool countingExp = false, bool female = false, bool pwd = false)
    {
        var districtId = User.FindFirst("District_ID")?.Value;

        var query = _context.PollingPersonnel
            .Where(e => e.WorkAC == acNo && e.dflag == false);

        // Apply district filter only if admin has a district assigned
        if (!string.IsNullOrEmpty(districtId))
        {
            query = query.Where(e => e.WorkDistrictId == districtId);
        }

        // In RandomizationController.GetSummary, after filtering by WorkAC, add:
        var assignedEmpCodes = _context.PollingTeamMembers
            .Where(m => _context.PollingTeams.Any(t => t.TeamId == m.TeamId && t.AC_No == acNo))
            .Select(m => m.EmpCode)
            .Distinct();
        query = query.Where(e => !assignedEmpCodes.Contains(e.EmpCode));

        // Apply other filters
        if (pollingExp)
            query = query.Where(e => e.ExperiencePolling == "Y");
        if (countingExp)
            query = query.Where(e => e.ExperienceCounting == "Y");
        if (female)
            query = query.Where(e => e.SexId == "F");
        if (pwd)
            query = query.Where(e => e.PWDTypeId != null);

        var data = query
            .GroupBy(e => e.Designation_Id)
            .Select(g => new
            {
                designation = g.Key,
                designationName = _context.Designations
                    .Where(d => d.DesignationId == g.Key)
                    .Select(d => d.DesignationEnglish)
                    .FirstOrDefault(),
                count = g.Count(),
                avgSalary = g.Average(x => x.Salary) ?? 0

            })
            .ToList();

        return Ok(data);
    }

    [HttpGet("booths")]
    public IActionResult GetBoothsCount(int acNo)
    {
        var count = _context.NewPartLists.Count(x => x.AC_No == acNo);
        return Ok(count);
    }

    [HttpGet("assigned-count")]
    public async Task<IActionResult> GetAssignedCount(int acNo)
    {
        // Get all team IDs for this AC
        var teamIds = await _context.PollingTeams
            .Where(t => t.AC_No == acNo)
            .Select(t => t.TeamId)
            .ToListAsync();

        // Get unique EmpCodes assigned to these teams
        var assignedEmpCodes = await _context.PollingTeamMembers
            .Where(m => teamIds.Contains(m.TeamId) && m.EmpCode.HasValue)
            .Select(m => m.EmpCode.Value)
            .Distinct()
            .ToListAsync();

        var assignedCount = assignedEmpCodes.Count;
        return Ok(assignedCount);
    }

    [HttpGet("total-employees")]
    public async Task<IActionResult> GetTotalEmployeesCount(int acNo)
    {
        var count = await _context.PollingPersonnel
            .Where(e => e.WorkAC == acNo && e.dflag == false)
            .CountAsync();
        return Ok(count);
    }

    [Authorize(Roles = "Admin")]  // only admin can assign duty
    [HttpPost("assign")]
    public async Task<IActionResult> AssignDuty([FromBody] AssignRequest req)
    {
        var districtId = User.FindFirst("District_ID")?.Value;
        if (string.IsNullOrEmpty(districtId))
            return BadRequest("District not assigned to this admin.");

        int acNo = req.acNo;
        var selectedDesignations = req.designations;

        // 1. Get all booths for this AC
        var booths = await _context.NewPartLists
            .Where(x => x.District_ID == districtId && x.AC_No == acNo)
            .ToListAsync();

        if (booths.Count == 0)
            return BadRequest("No booths found for this AC.");

        int required = booths.Count * 4;                     // 12
        int requiredWithBuffer = (int)Math.Ceiling(required * 1.2); // 15

        // 2. Get existing teams for this AC (only teams that exist)
        var existingTeams = await _context.PollingTeams
            .Where(t => t.District_ID == districtId && t.AC_No == acNo)
            .ToListAsync();

        // 3. Get currently assigned employees (unique EmpCodes)
        var teamIds = existingTeams.Select(t => t.TeamId).ToList();
        var assignedEmpCodes = new HashSet<int>();
        if (teamIds.Any())
        {
            assignedEmpCodes = new HashSet<int>(
                await _context.PollingTeamMembers
                    .Where(m => teamIds.Contains(m.TeamId) && m.EmpCode.HasValue)
                    .Select(m => m.EmpCode.Value)
                    .Distinct()
                    .ToListAsync()
            );
        }
        int alreadyAssigned = assignedEmpCodes.Count;

        // 4. Check if already fully assigned
        if (alreadyAssigned >= required)
            return BadRequest("All duties are already assigned for this AC.");

        // 5. Get eligible employees (filtered by designations, not already assigned)
        var eligibleEmployees = await _context.PollingPersonnel
            .Where(e => e.WorkDistrictId == districtId
                        && e.WorkAC == acNo
                        && e.dflag == false
                        && e.Designation_Id.HasValue
                        && selectedDesignations.Contains(e.Designation_Id.Value)
                        && !assignedEmpCodes.Contains(e.EmpCode.Value))
            .ToListAsync();

        // 6. Buffer check only for first assignment
        if (alreadyAssigned == 0)
        {
            if (eligibleEmployees.Count < requiredWithBuffer)
            {
                return BadRequest(
                    $"Not enough employees for initial assignment. Required (with 20% buffer): {requiredWithBuffer}, Available: {eligibleEmployees.Count}"
                );
            }
        }
        else
        {
            int remainingNeeded = required - alreadyAssigned;
            if (eligibleEmployees.Count < remainingNeeded)
            {
                return BadRequest(
                    $"Not enough employees to fill remaining {remainingNeeded} position(s). Available: {eligibleEmployees.Count}"
                );
            }
        }

        // 7. Smart sort (salary high first, senior designation, older first)
        var sorted = eligibleEmployees
            .OrderByDescending(e => e.Salary)
            .ThenByDescending(e => e.Designation_Id)
            .ThenBy(e => e.DOB)
            .ToList();

        // 8. Prepare data structures for assignment
        // We will maintain a queue of employees to assign
        var employeeQueue = new Queue<PollingPersonnel>(sorted);
        int totalAssignedNow = 0;
        int needed = required - alreadyAssigned;
        if (needed <= 0) return BadRequest("No vacancies to fill.");

        // 9. First, try to fill incomplete existing teams (those with less than 4 members)
        // Get current members per team
        var teamMembers = new Dictionary<int, List<PollingTeamMember>>();
        foreach (var team in existingTeams)
        {
            var members = await _context.PollingTeamMembers
                .Where(m => m.TeamId == team.TeamId)
                .ToListAsync();
            teamMembers[team.TeamId] = members;
        }

        // Find teams that have < 4 members and add employees to them
        foreach (var team in existingTeams)
        {
            if (needed <= 0) break;
            var currentMembers = teamMembers[team.TeamId];
            int currentCount = currentMembers.Count;
            if (currentCount >= 4) continue;

            int slotsToFill = 4 - currentCount;
            int toAdd = Math.Min(slotsToFill, needed);
            for (int i = 0; i < toAdd && employeeQueue.Count > 0; i++)
            {
                var emp = employeeQueue.Dequeue();
                var dutyPostId = currentCount + i + 1; // 1,2,3,4 based on next slot
                                                       // Map dutyPostId: if Presiding Officer not assigned yet, assign that etc.
                                                       // For simplicity, assign the next available duty post.
                                                       // Better: we should assign specific posts if missing. We'll keep simple.
                var newMember = new PollingTeamMember
                {
                    TeamId = team.TeamId,
                    EmpCode = emp.EmpCode,
                    DutyPostId = (currentCount + i + 1) // 1,2,3,4
                };
                _context.PollingTeamMembers.Add(newMember);
                totalAssignedNow++;
                needed--;
            }
        }

        // 10. If still needed, create new teams for booths that don't have any team yet
        var boothWithTeam = existingTeams.Select(t => t.Part_No).ToHashSet();
        var boothsWithoutTeam = booths.Where(b => !boothWithTeam.Contains(b.Part_No)).ToList();

        foreach (var booth in boothsWithoutTeam)
        {
            if (needed <= 0) break;
            // Create a new team
            var newTeam = new PollingTeam
            {
                District_ID = booth.District_ID,
                AC_No = booth.AC_No,
                Part_No = booth.Part_No,
                DutyBy_UserId = GetCurrentUserId(), // implement helper
                DutyDateTime = DateTime.Now
            };
            _context.PollingTeams.Add(newTeam);
            await _context.SaveChangesAsync(); // to get TeamId

            // Add up to 4 members
            for (int i = 0; i < 4 && employeeQueue.Count > 0 && needed > 0; i++)
            {
                var emp = employeeQueue.Dequeue();
                var member = new PollingTeamMember
                {
                    TeamId = newTeam.TeamId,
                    EmpCode = emp.EmpCode,
                    DutyPostId = i + 1
                };
                _context.PollingTeamMembers.Add(member);
                totalAssignedNow++;
                needed--;
            }
        }

        await _context.SaveChangesAsync();

        if (totalAssignedNow == 0)
            return BadRequest("No new duties assigned. All booths already have full teams or no eligible employees.");

        return Ok($"✅ Successfully assigned {totalAssignedNow} new duty position(s). Remaining vacancies: {needed}");
    }

    // Helper to get current user ID from JWT (add to your controller)
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out int id))
            return id;
        return 0;
    }
}