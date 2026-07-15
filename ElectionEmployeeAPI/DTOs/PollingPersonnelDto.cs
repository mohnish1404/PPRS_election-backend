using System.Text.Json;
using System.Text.Json.Serialization;

namespace ElectionEmployeeAPI.DTOs
{
    public class PollingPersonnelDto
    {
        // ======================
        // Basic Details
        // ======================
        public int? EmpCode { get; set; }
        public string EmpName { get; set; }
        public string EmpName_En { get; set; }

        public string SurName { get; set; }
        public string SurName_En { get; set; }

        public DateTime DOB { get; set; }

        // SexID: M / F / O
        public string SexId { get; set; }

        public string MobileNo { get; set; }

        public string? EmpImagePath { get; set; }

        // ======================
        // EPIC Details
        // ======================
        public bool HasEPIC { get; set; }
        public string? EPICNo { get; set; }
        public bool EPICVerified { get; set; }
        public string? EPIC_District_ID { get; set; }
        public string? EPIC_Block_ID { get; set; }
        public string? EPIC_Urban_Rural { get; set; }

        // ======================
        // PWD Details
        // ======================
        // PWDType: Y / N
        [JsonConverter(typeof(Int32NullableConverter))]
        public int? PWDTypeId { get; set; }
        public string? PWDPercentage { get; set; }
        public string? PWDCertificatePath { get; set; }


        // ======================
        // Address Details
        // ======================
        public string HomeDistrictId { get; set; }   // varchar(2)
        public string HomeBlockId { get; set; }      // varchar(3)

        // U / R
        public string UrbanRural { get; set; }

        public string WorkDistrictId { get; set; }

        public string? WorkBlockId { get; set; }

        public string? WorkUrbanRural { get; set; }

        // ======================
        // Office & Service
        // ======================
        public int DeptId { get; set; }
        public int Office_ID { get; set; }
        public int? Designation_Id { get; set; }
        public string? VargId { get; set; }
        public string? ReservationCategory { get; set; }
        public int? EmpTypeId { get; set; }

        public decimal? Salary { get; set; }

        // ======================
        // Assembly
        // ======================
        public int? ResAC { get; set; }
        public int? WorkAC { get; set; }

        // ======================
        // Bank Details
        // ======================
        public int? BankCode { get; set; }
        public string IFSCode { get; set; }
        public string AccountNumber { get; set; }
        public int? AC_No { get; set; }
        public int? Part_No { get; set; }
        public int? Serial_No { get; set; }


        // ======================
        // Experience
        // ======================
        public string ExperiencePolling { get; set; }   // Y / N
        public string ExperienceCounting { get; set; }  // Y / N

        // ======================
        public string? IsFieldDuty { get; set; }
        public int? Field_AC { get; set; }
        public string? Field_District_ID { get; set; }
        public string? Field_Block_ID { get; set; }
        // Election / Duty
        // ======================
        //public int? OtherElectionWorkId { get; set; }
        public int? DutyPostId { get; set; }
        public int? ElectionDesignation_Id { get; set; }
    }

    // Add a custom converter:
    public class Int32NullableConverter : JsonConverter<int?>
    {
        public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                    return null;

                if (int.TryParse(stringValue, out int intValue))
                    return intValue;

                return null;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteNumberValue(value.Value);
            else
                writer.WriteNullValue();
        }
    }
}
