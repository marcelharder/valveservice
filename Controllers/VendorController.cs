namespace ValveService.Controllers;

[ApiController]
[Route("[controller]")]
public class VendorController : ControllerBase
{
    private IValveCode _code;
    public VendorController(IValveCode code)
    {
        _code = code;

    }

    [HttpGet("valveCodesPerVTP/{id}/{type}/{position}")]
    public async Task<List<Valve_Code>?> getValveCodesByVTP(int id, string type, string position)
    {
        var help = await _code.getAllProductsByVTP(id, type, position);
        return help;
    }

    [HttpGet("valveCodesPerCountry/{id}/{isoCode}")]
    public async Task<List<Valve_Code>?> getValveCodes(int id, string isoCode)
    {
        var help = await _code.getAllProductsByVC(id, isoCode);
        return help;
    }

    [HttpGet("valveCodesItemsPerCountry/{id}/{isoCode}")]
    public async Task<List<Class_Item>?> getValveCodesItems(int id, string isoCode)
    {
        var help = await _code.getAllProductsItemsByVC(id, isoCode);
        return help;
    }





}
