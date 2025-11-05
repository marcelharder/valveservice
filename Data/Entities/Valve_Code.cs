
namespace ValveService.Data.Entities;
public class Valve_Code
{

    [Key]
    public virtual int ValveTypeId { get; set; }
    public string? hospitalId {get; set;}
    public int No { get; set; }
    public string? Vendor_description { get; set; }
    public int Vendor_code { get; set; }
    public ICollection<Valve_Size>? Valve_size { get; set; }
    public string? Model_code { get; set; }
    public string? Implant_position { get; set; }
    public string? uk_code { get; set; }
    public string? us_code { get; set; }
    public string? image { get; set; }
    public string? Description { get; set; }
    [Required]
    public string? Type { get; set; }
    public string? countries { get; set; }
}