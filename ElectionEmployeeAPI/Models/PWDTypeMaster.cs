using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("pwdtypemaster")]
public class PWDTypeMaster
{
    [Key]
    public int? PWDTypeId { get; set; }

    public string? PWDTypeNameEnglish { get; set; }
    public string? PWDTypeNameHindi { get; set; }

    public bool IsActive { get; set; }
}