using ElectionEmployeeAPI.Data;
using ElectionEmployeeAPI.DTOs;
using ElectionEmployeeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectionEmployeeAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/pollingpersonnel")]
    public class PollingPersonnelController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PollingPersonnelController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================================
        // 1️⃣ FILE UPLOAD API
        // ======================================
        [HttpPost("upload-files")]
        public async Task<IActionResult> UploadFiles([FromForm] PollingPersonnelUploadDto dto)
        {
            string? imagePath = null;
            string? pwdPath = null;

            // ---- IMAGE ----
            if (dto.EmpImagePath != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.EmpImagePath.FileName);
                var folder = Path.Combine("wwwroot/uploads/images");
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.EmpImagePath.CopyToAsync(stream);

                imagePath = "uploads/images/" + fileName;
            }

            // ---- PWD PDF ----
            if (dto.PWDCertificatePath != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(dto.PWDCertificatePath.FileName);
                var folder = Path.Combine("wwwroot/uploads/pwd");
                Directory.CreateDirectory(folder);

                var filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.PWDCertificatePath.CopyToAsync(stream);

                pwdPath = "uploads/pwd/" + fileName;
            }

            return Ok(new { imagePath, pwdPath });
        }

        // ======================================
        // 2️⃣ BULK SAVE (WITHOUT FILE OBJECT)
        // ======================================
  
        [HttpPost("bulk-save")]
        public async Task<IActionResult> BulkSave([FromBody] BulkPollingPersonnelDto dto)
        {
            if (dto == null || dto.Employees == null || dto.Employees.Count == 0)
                return BadRequest("No employee data received");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var e in dto.Employees)
                {
                    // 🔥 DUPLICATE CHECK (MobileNo se)
                    var alreadyExists = await _context.PollingPersonnel
                        .AnyAsync(x => x.MobileNo == e.MobileNo && x.dflag == false);

                    if (alreadyExists)
                    {
                        return BadRequest($"Employee with Mobile {e.MobileNo} already saved.");
                    }
                    if (!string.IsNullOrEmpty(e.EPICNo))
                    {
                        var epicExists = await _context.PollingPersonnel
                            .AnyAsync(x => x.EPICNo == e.EPICNo && x.dflag == false);

                        if (epicExists)
                        {
                            return BadRequest($"EPIC {e.EPICNo} already exists.");
                        }
                    }

                    if (e.IsFieldDuty == "Y")
                    {
                        if (e.Field_AC == null || string.IsNullOrEmpty(e.Field_District_ID) || string.IsNullOrEmpty(e.Field_Block_ID))
                        {
                            return BadRequest("Field Duty details are required when Yes is selected.");
                        }
                    }
                    if (e.IsFieldDuty != "Y" && e.IsFieldDuty != "N")
                    {
                        return BadRequest("Invalid value for Field Duty");
                    }
                    var entity = new PollingPersonnel
                    {
                        EmpCode = e.EmpCode,
                        EmpName = e.EmpName,
                        EmpName_En = e.EmpName_En,
                        SurName = e.SurName,
                        SurName_En = e.SurName_En,
                        DOB = e.DOB,
                        SexId = e.SexId,
                        MobileNo = e.MobileNo,

                        EmpImagePath = e.EmpImagePath,
                        PWDCertificatePath = e.PWDCertificatePath,

                        HasEPIC = e.HasEPIC,
                        EPICNo = e.EPICNo,
                        EPICVerified = e.EPICVerified,
                        EPIC_District_ID = e.EPIC_District_ID,
                        EPIC_Block_ID = e.EPIC_Block_ID,
                        EPIC_Urban_Rural = e.EPIC_Urban_Rural,

                        PWDTypeId = e.PWDTypeId,
                        PWDPercentage =
                            string.IsNullOrEmpty(e.PWDPercentage)
                               ? null
                               : int.Parse(e.PWDPercentage),


                        HomeDistrictId = e.HomeDistrictId,
                        HomeBlockId = e.HomeBlockId,
                        UrbanRural = e.UrbanRural,
                        WorkDistrictId = e.WorkDistrictId,
                        WorkBlockId = e.WorkBlockId,
                        WorkUrbanRural = e.WorkUrbanRural,

                        DeptId = e.DeptId,
                        Office_ID = e.Office_ID,
                        Designation_Id = e.Designation_Id,
                        VargId = e.VargId,
                        ReservationCategory = e.ReservationCategory,
                        EmpTypeId = e.EmpTypeId,
                        Salary = e.Salary,

                        ResAC = e.ResAC,
                        WorkAC = e.WorkAC,

                        BankCode = e.BankCode,
                        IFSCode = e.IFSCode,
                        AccountNumber = e.AccountNumber,
                        AC_No = e.AC_No,
                        Part_No = e.Part_No,
                        Serial_No = e.Serial_No,
                        ExperiencePolling = e.ExperiencePolling,
                        ExperienceCounting = e.ExperienceCounting,
                        IsFieldDuty = e.IsFieldDuty,
                        Field_AC = e.Field_AC,
                        Field_District_ID = e.Field_District_ID,
                        Field_Block_ID = e.Field_Block_ID,



                        dflag = false
                    };

                    await _context.PollingPersonnel.AddAsync(entity);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Saved Successfully");
            }
            //catch (Exception ex)
            //{
            //    await transaction.RollbackAsync();
            //    return StatusCode(500, ex.Message);
            //}
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        // ======================================
        // 3️⃣ GET ACTIVE
        // ======================================
        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var data = await _context.PollingPersonnel
        //        .Where(p => p.dflag == false)
        //        .OrderByDescending(p => p.EmpCode)
        //        .ToListAsync();

        //    return Ok(data);
        //}
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var districtId = User.FindFirst("District_ID")?.Value;
            var query = _context.PollingPersonnel.Where(p => p.dflag == false);

            // If district is assigned (admin with specific district), filter by WorkDistrictId
            if (!string.IsNullOrEmpty(districtId))
            {
                query = query.Where(p => p.WorkDistrictId == districtId);
            }
            // Super admin (districtId empty) sees all records

            var data = await query.OrderByDescending(p => p.EmpCode).ToListAsync();
            return Ok(data);
        }



        // ======================================
        // 4️⃣ SOFT DELETE
        // ======================================

        //[HttpDelete("{empCode}")]
        //public async Task<IActionResult> SoftDelete(int empCode)
        //{
        //    var emp = await _context.PollingPersonnel.FindAsync(empCode);
        //    if (emp == null)
        //        return NotFound("Employee not found");

        //    emp.dflag = true;
        //    emp.UpdateDt = DateTime.Now;

        //    await _context.SaveChangesAsync();
        //    return Ok("Soft Deleted");
        //}

        [Authorize(Roles = "Admin")]  // only admin can delete
        [HttpDelete("{empCode}")]
        public async Task<IActionResult> SoftDelete(int empCode)
        {
            var districtId = User.FindFirst("District_ID")?.Value;
            var emp = await _context.PollingPersonnel.FindAsync(empCode);
            if (emp == null)
                return NotFound("Employee not found");

            // If admin has specific district, check that employee belongs to that district
            if (!string.IsNullOrEmpty(districtId) && emp.WorkDistrictId != districtId)
            {
                return Forbid("You can only delete employees from your own district.");
            }

            emp.dflag = true;
            emp.UpdateDt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok("Soft Deleted");
        }



        //[HttpPost("generate-randomization")]
        //public async Task<IActionResult> GenerateRandomization(int acNo)
        //{
        //    int userId = 2;

        //    // 🔹 Booths
        //    var booths = _context.NewPartLists
        //        .Where(x => x.AC_No == acNo)
        //        .ToList();

        //    if (booths.Count == 0)
        //        return BadRequest("No booths found");

        //    // 🔹 Employees (WorkDistrict + AC match)
        //    var employees = _context.PollingPersonnel
        //        .Where(e => e.WorkAC == acNo && e.dflag == false)
        //        .ToList();

        //    if (employees.Count == 0)
        //        return BadRequest("No employees found");

        //    // 🔹 Shuffle
        //    Random rnd = new Random();
        //    employees = employees.OrderBy(x => rnd.Next()).ToList();

        //    // 🔹 Assign
        //    for (int i = 0; i < booths.Count; i++)
        //    {
        //        var booth = booths[i];

        //        var team = new PollingTeam
        //        {
        //            District_ID = booth.District_ID,
        //            AC_No = booth.AC_No,
        //            Part_No = booth.Part_No,
        //            DutyBy_UserId = userId,
        //            DutyDateTime = DateTime.Now
        //        };

        //        _context.PollingTeams.Add(team);
        //        await _context.SaveChangesAsync();

        //        var members = new List<PollingTeamMember>
        //{
        //    new PollingTeamMember { TeamId = team.TeamId, EmpCode = employees[i % employees.Count].EmpCode, DutyPostId = 1 },
        //    new PollingTeamMember { TeamId = team.TeamId, EmpCode = employees[(i+1) % employees.Count].EmpCode, DutyPostId = 2 },
        //    new PollingTeamMember { TeamId = team.TeamId, EmpCode = employees[(i+2) % employees.Count].EmpCode, DutyPostId = 3 },
        //    new PollingTeamMember { TeamId = team.TeamId, EmpCode = employees[(i+3) % employees.Count].EmpCode, DutyPostId = 4 }
        //};

        //        _context.PollingTeamMembers.AddRange(members);
        //        await _context.SaveChangesAsync();
        //    }

        //    return Ok("✅ Randomization Completed");
        //}
    }
}
