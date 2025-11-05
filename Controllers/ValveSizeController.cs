namespace ValveService.Controllers;

[ApiController]
[Route("[controller]")]
public class ValveSizeController : ControllerBase
{
    private IValveCode _code;

    public ValveSizeController(IValveCode code)
    {
        _code = code;
    }

[HttpGet("getSizesForValve/{id}")]
public async Task<IActionResult> getSizesForValve(int id){ var result = await _code.getSizesForValve(id);  return Ok(result);}

[HttpPut]
public async Task<IActionResult> updateSize([FromBody]Valve_Size vs){ var result = await _code.updateValveSize(vs); return Ok(result);}

[HttpDelete("{SizeId}")]
public async Task<IActionResult> deleteSize(int SizeId){ var result = await _code.deleteValveSize(SizeId); return Ok(result);}

[HttpPost]
public async Task<IActionResult> addSize([FromBody]Valve_Size vs){ var result = await _code.addValveSize(vs); return Ok(result);}

[HttpGet("{SizeId}")]
public async Task<IActionResult> getSize(int SizeId){ var result = await _code.getSize(SizeId); return Ok(result);}

[HttpGet("getSizesForSuggestedValves/{minid}/{maxid}/{soort}")]
public async Task<IActionResult> getSizesForSuggestedValves(float minid, float maxid,string soort){
        var result = await _code.getSizesForSuggestedValves(minid,maxid, soort);
        return Ok(result);
    }

}