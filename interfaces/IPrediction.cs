using Microsoft.AspNetCore.Mvc;
using ValveService.implementations;

namespace ValveService.interfaces;

public interface IPrediction
{
    PredictionResult CalculatePrediction(double age, double weight, double height, string sex);

    //CalculationResult CalculateResults(double age, double weight, double height, string sex, double? measuredAAD = null);

    //Task<bool> LoadModelAsync(string jsonFilePath);
}
