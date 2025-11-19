namespace ValveService.Controllers;

[ApiController]
[Route("[controller]")]
public class PredictedSizeController : ControllerBase
{
    private IPrediction _prediction;

    public PredictedSizeController(IPrediction prediction)
    {
        _prediction = prediction;
    }

    [HttpGet("calculatePrediction/{age}/{weight}/{height}/{sex}")]
    public async Task<IActionResult> predictSize(int age, double weight, double height, string sex)
    {
        var result =  _prediction.CalculatePrediction(age, weight, height, sex);
        return Ok(result);
    }

    [HttpGet("calculateResults/{age}/{weight}/{height}/{sex}/{measuredAAD?}")]
    public async Task<IActionResult> calculateResults(
        int age,
        double weight,
        double height,
        string sex,
        double? measuredAAD = null
    )
    {
        //var result = _prediction.CalculateResults(age, weight, height, sex, measuredAAD);
        return Ok();
    }
}
