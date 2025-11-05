namespace ValveService.helpers;

public class AreaConverter
{
    /// <summary>
    /// Converts an area in square centimeters (cm²) to diameter in millimeters (mm).
    /// </summary>
    /// <param name="areaCm2">The area in square centimeters.</param>
    /// <returns>The diameter in millimeters.</returns>
    public double ConvertAreaCm2ToDiameterMm(double areaCm2)
    {
        if (areaCm2 < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(areaCm2), "Area cannot be negative.");
        }

        // Area A = pi * r^2
        // r = sqrt(A / pi)
        // d = 2 * r = 2 * sqrt(A / pi)

        // Convert cm2 to mm2 for calculation, or convert cm to mm at the end.
        // Let's calculate radius in cm first, then convert to mm.
        double radiusCm = Math.Sqrt(areaCm2 / Math.PI);
        double diameterCm = 2 * radiusCm;
        double diameterMm = diameterCm * 10; // 1 cm = 10 mm

        return diameterMm;
    }
   
}