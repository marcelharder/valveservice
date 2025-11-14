namespace ValveService.helpers;
public class CalculationResult
{
    public double MeanAAD { get; set; }
    public double StdDev { get; set; }
    public double LowerBound { get; set; }
    public double UpperBound { get; set; }
    public double? ZScore { get; set; }
}