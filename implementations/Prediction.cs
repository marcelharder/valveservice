using System.Text.Json.Serialization;

namespace ValveService.implementations;

public class Prediction : IPrediction
{
    private LookupData _lookupData = null!;
    public bool IsLoaded { get; private set; }

    public Prediction() { IsLoaded = false; }

    public async Task<bool> LoadModelAsync(string jsonFilePath)
    {
        if (string.IsNullOrWhiteSpace(jsonFilePath))
            throw new ArgumentException("Invalid path", nameof(jsonFilePath));

        try
        {
            var json = await File.ReadAllTextAsync(jsonFilePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _lookupData = JsonSerializer.Deserialize<LookupData>(json, options)
                          ?? throw new InvalidOperationException("Failed to deserialize lookup data");

            IsLoaded = true;
            return true;
        }
        catch (Exception)
        {
            IsLoaded = false;
            throw;
        }
    }

    private int FindLowerIndex(double[] vec, double q)
    {
        var n = vec.Length;
        if (n < 2) throw new ArgumentException("Vector must contain at least two elements", nameof(vec));
        if (q <= vec[0]) return 0;
        if (q >= vec[n - 1]) return n - 2;

        int left = 0, right = n - 1;
        while (left < right - 1)
        {
            int mid = (left + right) / 2;
            if (vec[mid] <= q) left = mid;
            else right = mid;
        }

        return left;
    }

    private double TrilinearInterpolation(
        double[] x, double[] y, double[] z,
        double[] flatValues,
        double xq, double yq, double zq,
        int s)
    {
        int nX = x.Length;
        int nY = y.Length;
        int nZ = z.Length;

        int sliceSize = nX * nY * nZ;
        if (flatValues.Length % sliceSize != 0)
            throw new ArgumentException("Flat values length is not compatible with grid dimensions", nameof(flatValues));

        int getIndex(int i, int j, int k, int ss)
        {
            return i + j * nX + k * nX * nY + ss * sliceSize;
        }

        int i = FindLowerIndex(x, xq);
        int j = FindLowerIndex(y, yq);
        int k = FindLowerIndex(z, zq);
        int sIndex = s;

        int i1 = Math.Min(i + 1, nX - 1);
        int j1 = Math.Min(j + 1, nY - 1);
        int k1 = Math.Min(k + 1, nZ - 1);

        double x0 = x[i], x1 = x[i1];
        double y0 = y[j], y1 = y[j1];
        double z0 = z[k], z1 = z[k1];

        double xd = (x1 - x0) != 0 ? (xq - x0) / (x1 - x0) : 0.0;
        double yd = (y1 - y0) != 0 ? (yq - y0) / (y1 - y0) : 0.0;
        double zd = (z1 - z0) != 0 ? (zq - z0) / (z1 - z0) : 0.0;

        double c000 = flatValues[getIndex(i, j, k, sIndex)];
        double c100 = flatValues[getIndex(i1, j, k, sIndex)];
        double c010 = flatValues[getIndex(i, j1, k, sIndex)];
        double c110 = flatValues[getIndex(i1, j1, k, sIndex)];
        double c001 = flatValues[getIndex(i, j, k1, sIndex)];
        double c101 = flatValues[getIndex(i1, j, k1, sIndex)];
        double c011 = flatValues[getIndex(i, j1, k1, sIndex)];
        double c111 = flatValues[getIndex(i1, j1, k1, sIndex)];

        double c00 = c000 * (1 - xd) + c100 * xd;
        double c10 = c010 * (1 - xd) + c110 * xd;
        double c01 = c001 * (1 - xd) + c101 * xd;
        double c11 = c011 * (1 - xd) + c111 * xd;

        double c0 = c00 * (1 - yd) + c10 * yd;
        double c1 = c01 * (1 - yd) + c11 * yd;

        return c0 * (1 - zd) + c1 * zd;
    }

    public PredictionResult CalculatePrediction(double age, double weight, double height, string sex)
    {
        if (!IsLoaded) throw new InvalidOperationException("Model not loaded");
        if (age <= 0 || weight <= 0 || height <= 0) throw new ArgumentException("Invalid input parameters");
        if (string.IsNullOrWhiteSpace(sex)) throw new ArgumentException("Invalid sex", nameof(sex));

        double logWeight = Math.Log(weight);
        double sqrtHeight = Math.Sqrt(height);
        double logAge = Math.Log(age + 1);

        int sexValue = string.Equals(sex, "male", StringComparison.OrdinalIgnoreCase) ? 0 : 1;

        var xGrid = _lookupData.GridAxes.LogWeight;
        var yGrid = _lookupData.GridAxes.SqrtHeight;
        var zGrid = _lookupData.GridAxes.LogAgePlus1;

        double meanAAD = TrilinearInterpolation(
            xGrid, yGrid, zGrid,
            _lookupData.Predictions.MeanAad,
            logWeight, sqrtHeight, logAge, sexValue);

        double stdDev = TrilinearInterpolation(
            xGrid, yGrid, zGrid,
            _lookupData.Predictions.StdDev,
            logWeight, sqrtHeight, logAge, sexValue);

        return new PredictionResult
        {
            MeanAAD = meanAAD,
            StdDev = stdDev
        };
    }

    public CalculationResult CalculateResults(double age, double weight, double height, string sex, double? measuredAAD = null)
    {
        var pred = CalculatePrediction(age, weight, height, sex);

        var result = new CalculationResult
        {
            MeanAAD = pred.MeanAAD,
            StdDev = pred.StdDev,
            LowerBound = pred.MeanAAD - 2 * pred.StdDev,
            UpperBound = pred.MeanAAD + 2 * pred.StdDev,
            ZScore = null
        };

        if (measuredAAD.HasValue && measuredAAD.Value > 0 && pred.StdDev != 0)
        {
            result.ZScore = (measuredAAD.Value - pred.MeanAAD) / pred.StdDev;
        }

        return result;
    }

    // Data models matching JSON structure
    private class LookupData
    {
        [JsonPropertyName("dimensions")]
        public int[] Dimensions { get; set; }

        [JsonPropertyName("grid_axes")]
        public GridAxes GridAxes { get; set; }

        [JsonPropertyName("predictions")]
        public Predictions Predictions { get; set; }
    }

    private class GridAxes
    {
        [JsonPropertyName("log_weight")]
        public double[] LogWeight { get; set; }

        [JsonPropertyName("sqrt_height")]
        public double[] SqrtHeight { get; set; }

        [JsonPropertyName("log_age_plus_1")]
        public double[] LogAgePlus1 { get; set; }
    }

    private class Predictions
    {
        [JsonPropertyName("mean_aad")]
        public double[] MeanAad { get; set; }

        [JsonPropertyName("std_dev")]
        public double[] StdDev { get; set; }
    }
}




