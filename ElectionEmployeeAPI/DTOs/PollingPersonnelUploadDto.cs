using Microsoft.AspNetCore.Http;

namespace ElectionEmployeeAPI.DTOs
{

    public class PollingPersonnelUploadDto
    {
        public IFormFile? EmpImagePath { get; set; }
        public IFormFile? PWDCertificatePath { get; set; }
    }

}
