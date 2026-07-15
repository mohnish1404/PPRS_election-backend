using ElectionEmployeeAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElectionEmployeeAPI.Controllers
{
    [ApiController]
    [Route("api/masters")]
    public class MastersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MastersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("districts")]
        public async Task<IActionResult> GetDistricts()
        {
            return Ok(await _context.Districts.ToListAsync());
        }

        [HttpGet("blocks/{districtId}")]
        public async Task<IActionResult> GetBlocks(string districtId)
        {
            return Ok(await _context.Blocks
                .Where(b => b.DistrictId == districtId && b.dflag == false)
                .ToListAsync());
        }

        [HttpGet("offices")]
        public async Task<IActionResult> GetOffices()
        {
            var data = await _context.Offices
                .Where(o => o.dflag == false)
                .OrderBy(o => o.OfficeHindi)
                .ToListAsync();

            return Ok(data);
        }

        // 🔹 Offices by Department
        [HttpGet("offices/by-department/{deptId}")]
        public async Task<IActionResult> GetOfficesByDepartment(int deptId)
        {
            var data = await _context.Offices
                .Where(o => o.DeptId == deptId && o.dflag == false)
                .OrderBy(o => o.OfficeHindi)
                .ToListAsync();

            return Ok(data);
        }

        // 🔹 Offices by District
        [HttpGet("offices/by-district/{districtId}")]
        public async Task<IActionResult> GetOfficesByDistrict(string districtId)
        {
            var data = await _context.Offices
                .Where(o => o.DistrictId == districtId && o.dflag == false)
                .OrderBy(o => o.OfficeHindi)
                .ToListAsync();

            return Ok(data);
        }
        // ============================
        // DEPARTMENT MASTER
        // ============================

        // 🔹 All active departments
        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var data = await _context.Departments
                .Where(d => d.dflag == false)
                .OrderBy(d => d.DeptEnglish)
                .ToListAsync();

            return Ok(data);
        }

        // ============================
        // DESIGNATION MASTER
        // ============================

        // 🔹 All active designations
        [HttpGet("designations")]
        public async Task<IActionResult> GetDesignations()
        {
            var data = await _context.Designations
                .Where(d => d.dflag == false)
                .OrderBy(d => d.DesignationEnglish)
                .ToListAsync();

            return Ok(data);
        }

        // 🔹 Designations by Varg
        [HttpGet("designations/by-varg/{vargId}")]
        public async Task<IActionResult> GetDesignationsByVarg(string vargId)
        {
            var data = await _context.Designations
                .Where(d => d.VargId == vargId && d.dflag == false)
                .OrderBy(d => d.DesignationEnglish)
                .ToListAsync();

            return Ok(data);
        }

        // ============================
        [HttpGet("vargs-from-designation")]
        public async Task<IActionResult> GetVargsFromDesignation()
        {
            var data = await _context.Designations
                .Where(d => d.dflag == false)
                .Select(d => new
                {
                    VargId = d.VargId,
                    Name = d.VargId == "I" ? "Class I" :
                           d.VargId == "II" ? "Class II" :
                           d.VargId == "III" ? "Class III" : ""
                })
                .Distinct()
                .ToListAsync();

            return Ok(data);
        }
        // EMP TYPE MASTER
        // ============================
        [HttpGet("pwd-types")]
        public async Task<IActionResult> GetPWDTypes()
        {
            var data = await _context.PWDTypeMaster
                .Where(x => x.IsActive == true)
                .ToListAsync();

            return Ok(data);
        }

        // 🔹 All active employee types
        [HttpGet("emp-types")]
        public async Task<IActionResult> GetEmpTypes()
        {
            var data = await _context.EmpTypes
                .Where(e => e.dflag == false)
                .OrderBy(e => e.EmpTypeEnglish)
                .ToListAsync();

            return Ok(data);
        }
        // ============================
        // ASSEMBLY CONSTITUENCY (AC)
        // ============================

        // 🔹 All active ACs
        //[HttpGet("ac-list")]
        //public async Task<IActionResult> GetAllACs()
        //{
        //    var data = await _context.AcList
        //        .Where(a => a.dflag == false)
        //        .OrderBy(a => a.AcNo)
        //        .ToListAsync();

        //    return Ok(data);
        //}
       //[Authorize]  // Ensure only authenticated users can access
        [HttpGet("ac-list")]
        public async Task<IActionResult> GetAllACs()
        {
            // Get district from JWT claim (set during login)
            var districtId = User.FindFirst("District_ID")?.Value;

            var query = _context.AcList.Where(a => a.dflag == false);

            // If district is assigned (not null/empty), filter by it
            if (!string.IsNullOrEmpty(districtId))
            {
                query = query.Where(a => a.DistrictId == districtId);
            }
            // Else super admin sees all ACs

            var data = await query.OrderBy(a => a.AcNo).ToListAsync();
            return Ok(data);
        }

        // 🔹 ACs by District
        [HttpGet("ac-list/by-district/{districtId}")]
        public async Task<IActionResult> GetACsByDistrict(string districtId)
        {
            var data = await _context.AcList
                .Where(a => a.DistrictId == districtId && a.dflag == false)
                .OrderBy(a => a.AcNo)
                .ToListAsync();

            return Ok(data);
        }

        // 🔹 ACs by Urban / Rural
        [HttpGet("ac-list/by-type/{urbanRural}")]
        public async Task<IActionResult> GetACsByType(string urbanRural)
        {
            var data = await _context.AcList
                .Where(a => a.UrbanRural == urbanRural && a.dflag == false)
                .OrderBy(a => a.AcNo)
                .ToListAsync();

            return Ok(data);
        }
        // ============================
        // BANK MASTER
        // ============================

        // 🔹 All active banks
        [HttpGet("banks")]
        public async Task<IActionResult> GetBanks()
        {
            var data = await _context.Banks
                .Where(b => b.dflag == false)
                .OrderBy(b => b.BankNameEnglish)
                .ToListAsync();

            return Ok(data);
        }
        // ============================
        // BRANCH MASTER
        // ============================

        // 🔹 Branches by Bank
        [HttpGet("branches/by-bank/{bankCode}")]
        public async Task<IActionResult> GetBranchesByBank(int bankCode)
        {
            var data = await _context.Branches
                .Where(b => b.BankCode == bankCode && b.dflag == false)
                .OrderBy(b => b.BranchNameEnglish)
                .ToListAsync();

            return Ok(data);
        }
        // 🔹 Branches by Bank & District
        [HttpGet("branches/by-bank-district/{bankCode}/{districtId}")]
        public async Task<IActionResult> GetBranchesByBankAndDistrict(int bankCode, string districtId)
        {
            var data = await _context.Branches
                .Where(b =>
                    b.BankCode == bankCode &&
                    b.DistrictId == districtId &&
                    b.dflag == false)
                .OrderBy(b => b.BranchNameEnglish)
                .ToListAsync();

            return Ok(data);
        }

        // ============================
        // DUTY POST MASTER
        // ============================

        // 🔹 All active duty posts
        [HttpGet("duty-posts")]
        public async Task<IActionResult> GetDutyPosts()
        {
            var data = await _context.DutyPosts
                .Where(d => d.dflag == false)
                .OrderBy(d => d.DutyPostEnglish)
                .ToListAsync();

            return Ok(data);
        }
        // ============================
        // DESIGNATION ELECTION DUTY MASTER
        // ============================

        // 🔹 All active election designations
        [HttpGet("designation-election-duties")]
        public async Task<IActionResult> GetDesignationElectionDuties()
        {
            var data = await _context.DesignationElectionDuties
                .Where(d => d.dflag == false)
                .OrderBy(d => d.DesignationEnglish)
                .ToListAsync();

            return Ok(data);
        }

        // 🔹 By Varg (optional, but useful)
        [HttpGet("designation-election-duties/by-varg/{vargId}")]
        public async Task<IActionResult> GetDesignationElectionDutiesByVarg(string vargId)
        {
            var data = await _context.DesignationElectionDuties
                .Where(d => d.VargId == vargId && d.dflag == false)
                .OrderBy(d => d.DesignationEnglish)
                .ToListAsync();

            return Ok(data);
        }
        // ============================
        // ELECTION WORK MASTER
        // ============================

        // 🔹 All active election works
        [HttpGet("election-works")]
        public async Task<IActionResult> GetElectionWorks()
        {
            var data = await _context.ElectionWorkMasters
                .Where(e => e.dflag == false)
                .OrderBy(e => e.ElectionWorkEnglish)
                .ToListAsync();

            return Ok(data);
        }
    

    }
}
