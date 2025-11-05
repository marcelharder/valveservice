namespace ValveService.implementations;

public class BiologicalValves
{
    private DapperContext _context;

    public BiologicalValves(DapperContext context)
    {
        _context = context;
    }

    public async Task<List<ValveSizeForReturnDTO>> GetBiological(float minid, float maxid, string soort)
    {
        List<Valve_Size> help;
        float requiredUpperIOA = maxid;
        float requiredLowerIOA = minid;
        var query =
            "SELECT s.*, c.* FROM ValveSizes s JOIN ValveCodes c ON s.VTValveTypeId = c.ValveTypeId " +
            "WHERE c.TYPE = @soort " +
            "AND s.IOD <= @requiredUpperIOA " +
            "AND s.IOD >= @requiredLowerIOA ORDER BY s.IOD ASC";
        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryAsync<Valve_Size>(query, new { requiredUpperIOA, requiredLowerIOA, soort });
            if (result != null)
            {
                help = result.ToList();
            }
            else
            {
                return new List<ValveSizeForReturnDTO>();
            }
        }

        var listOfBioValves = new List<ValveSizeForReturnDTO>();
        if (help != null)
        {
            foreach (Valve_Size vs in help)
            {
                var test1 = new ValveSizeForReturnDTO
                {
                    SizeId = vs.SizeId,
                    Size = vs.Size,
                    VTValveTypeId = vs.VTValveTypeId,
                    EOA = vs.EOA,
                    IOD = vs.IOD,
                    OOD = vs.OOD,
                    Height = vs.Height,
                    ConeAngle = vs.ConeAngle,
                    VT = vs.VT,
                    ValveTypeId = vs.ValveTypeId,
                };
                    listOfBioValves.Add(test1);
                
            }
            return listOfBioValves;
        }
        else { }
        {
            return new List<ValveSizeForReturnDTO>();
        }
    }
}