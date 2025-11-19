namespace ValveService.Controllers;

[ApiController]
[Route("[controller]")]
public class ValveCodeController : ControllerBase
{
    private IValveCode _code;
    private Cloudinary _cloudinary;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

    public ValveCodeController(IValveCode code, IOptions<CloudinarySettings> cloudinaryConfig)
    {
        _code = code;
        _cloudinaryConfig = cloudinaryConfig;

        Account acc = new Account(
            _cloudinaryConfig.Value.CloudName,
            _cloudinaryConfig.Value.ApiKey,
            _cloudinaryConfig.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(acc);
    }

    #region <!-- soa end points -->

    [HttpGet("getValveCodesInHospital/{type}/{position}/{hospitalId}")]
    public async Task<IActionResult> getValveCodes(string type, string position, int hospitalId) // returns Full Class_Valve
    {
        var result = await _code.GetValveCodesInHospital(position, type, hospitalId);
        return Ok(result);
    }

    [HttpGet("products")]
    public async Task<IActionResult> getAllCodes()
    {
        var result = await _code.getAllProducts();
        return Ok(result);
    }

    [HttpGet("getAllValve_CodeAsItems/{type}/{position}")] // returns List<Class_Item> from all ValveCodes
    public async Task<IActionResult> getValveCodesItems(string type, string position)
    {
        var result = await _code.getAllTPProducts(position, type);
        return Ok(result);
    }

    [HttpGet("getValve_CodeAsItemsNotInHospital/{type}/{position}/{hospitalId}")] // returns List<Class_Item>
    public async Task<IActionResult> getExtraValveCodes(
        string type,
        string position,
        int hospitalId
    )
    {
        var result = await _code.GetValveCodesNOTinHospital(position, type, hospitalId);
        return Ok(result);
    }

    [HttpGet("writeHospitalIdToValveCode/{id}/{hospitalId}")]
    public async Task<IActionResult> getExtraValveCodes(int id, int hospitalId)
    {
        var result = await _code.addHospitalIdToValveCode(id, hospitalId);
        return Ok(result);
    }

    [HttpGet("detailsByValveId/{id}", Name = "getValveCode")]
    public async Task<IActionResult> GetSpecificValveCode(int id)
    {
        var result = await _code.getDetailsByValveTypeId(id);
        return Ok(result);
    }

    [HttpGet("detailsByProductCode/{id}")]
    public async Task<IActionResult> GetSpecificValveCodeId(string id)
    {
        var result = await _code.getDetailsByProductCode(id);
        return Ok(result);
    }

    [HttpGet("detailsByValveNo/{id}")]
    public async Task<IActionResult> GetSpecificValveCodeByNo(int id)
    {
        var result = await _code.getDetailsByNo(id);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateValveCodes([FromBody] Valve_Code vc)
    {
        var result = await _code.updateValveCode(vc);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteValveCodes(int id)
    {
        var result = await _code.deleteValveCode(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddValveCodes([FromBody] Valve_Code vc)
    {
        var result = await _code.addValveCode(vc);
        return Ok(result);
    }

    [HttpPost("addPhoto/{id}")]
    public async Task<IActionResult> AddPhoto(int id, [FromForm] PhotoForCreationDto photoDto)
    {
        var h = await _code.getDetailsByValveTypeId(id);
        if (h == null)
        {
            return NotFound();
        }
        var file = photoDto.File;
        if (file != null)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation()
                            .Width(500)
                            .Height(500)
                            .Crop("fill")
                            .Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
                h.image = uploadResult?.SecureUrl?.AbsoluteUri;
                // automap it to class-hospital before save
                var no = await _code.updateValveCode(h);
                return CreatedAtRoute("getValveCode", new { id = h.ValveTypeId }, h);
            }
        }
        return BadRequest("Could not add the photo ...");
    }
    #endregion
    #region <!-- online inventory end points -->


    #endregion
}
