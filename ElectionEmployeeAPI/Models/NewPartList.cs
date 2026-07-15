using System.ComponentModel.DataAnnotations.Schema;

[Table("newpartlist")]
public class NewPartList
{
    public string District_ID { get; set; }
    public int AC_No { get; set; }
    public int Part_No { get; set; }
}