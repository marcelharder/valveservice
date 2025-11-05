namespace ValveService.Data.dtos;

public class CountryDto
{
    public string? IsoCode { get; set; }
    public int TelCode { get; set; }
    public string? Description { get; set; }

    public string? Cities { get; set; }
}