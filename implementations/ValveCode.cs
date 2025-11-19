namespace ValveService.implementations;

public class ValveCode : IValveCode
{
    private DapperContext _context;

    public ValveCode(DapperContext context)
    {
        _context = context;
    }

    #region <!-- ValveCode-->
    public async Task<string> addHospitalIdToValveCode(int ValveTypeId, int hospitalId)
    {
        var query = "SELECT * FROM ValveCodes WHERE ValveTypeId = @ValveTypeId";
        using (var connection = _context.CreateConnection())
        {
            var vc = await connection.QueryFirstOrDefaultAsync<Valve_Code>(
                query,
                new { ValveTypeId }
            );
            if (vc != null)
            {
                if (vc.hospitalId == null || vc.hospitalId == "")
                {
                    vc.hospitalId = vc.hospitalId + hospitalId.ToString().makeSureTwoChar();
                }
                else
                {
                    vc.hospitalId = vc.hospitalId + "," + hospitalId.ToString().makeSureTwoChar();
                }
                try
                {
                    await updateValveCode(vc);
                }
                catch (Exception e)
                {
                    Console.Write(e.InnerException);
                }
            }

            return "1";
        }
    }

    public async Task<List<Class_Item>> GetValveCodesNOTinHospital(
        string location,
        string type,
        int selectedHospital
    )
    {
        var vctr = new List<Class_Item>();
        var query = "SELECT * FROM ValveCodes WHERE Type = @type AND Implant_position = @location";
        using (var connection = _context.CreateConnection())
        {
            var documents = await connection.QueryAsync<Valve_Code>(query, new { type, location });
            foreach (Valve_Code vc in documents)
            {
                if (vc.hospitalId != null)
                {
                    var hos = selectedHospital.ToString().makeSureTwoChar();
                    var help_array = vc.hospitalId.Split(',');
                    if (!help_array.Contains(hos))
                    {
                        var ci = new Class_Item();
                        ci.value = vc.ValveTypeId;
                        ci.description = vc.uk_code;
                        vctr.Add(ci);
                    }
                }
                else { }
            }
            return vctr;
        }
    }

    public async Task<List<Class_Item>> getAllTPProducts(string location, string type)
    {
        var vctr = new List<Class_Item>();

        var query = "SELECT * FROM ValveCodes WHERE Type = @type AND Implant_position = @location";
        using (var connection = _context.CreateConnection())
        {
            var documents = await connection.QueryAsync<Valve_Code>(query, new { type, location });
            foreach (Valve_Code el in documents)
            {
                var cl = new Class_Item();
                cl.description = el.Description;
                cl.value = el.ValveTypeId;
                vctr.Add(cl);
            }
            return vctr;
        }
    }

    public async Task<List<Valve_Code>> GetValveCodesInHospital(
        string location,
        string type,
        int hospitalId
    )
    {
        var vctr = new List<Valve_Code>();
        var query = "SELECT * FROM ValveCodes WHERE Type = @type AND Implant_position = @location";
        using (var connection = _context.CreateConnection())
        {
            var documents = await connection.QueryAsync<Valve_Code>(query, new { type, location });
            foreach (Valve_Code vc in documents)
            {
                if (hospitalId != 0 && vc.hospitalId != null)
                {
                    var hospital = hospitalId.ToString().makeSureTwoChar();
                    var help = vc.hospitalId.Split(',');
                    if (help.Contains(hospital))
                    {
                        vctr.Add(vc);
                    }
                }
            }
            return vctr;
        }
    }

    public async Task<Valve_Code?> addValveCode(Valve_Code vc)
    {
        vc.No = await getNewHospitalNo();

        var query =
            "INSERT INTO ValveCodes (hospitalId,No,Vendor_description, Vendor_code, Model_code, Implant_position, uk_code,us_code, image, description, type, countries)"
            + "VALUES (@hospitalId,@No,@Vendor_description, @Vendor_code, @Model_code, @Implant_position, @uk_code,@us_code, @image, @description, @type, @countries);"
            + " SELECT LAST_INSERT_ID() FROM ValveCodes";

        var parameters = new DynamicParameters();

        parameters.Add("hospitalId", vc.hospitalId, DbType.String);
        parameters.Add("No", vc.No, DbType.Int32);
        parameters.Add("Vendor_description", vc.Vendor_description, DbType.String);
        parameters.Add("Vendor_code", vc.Vendor_code, DbType.Int32);
        parameters.Add("Model_code", vc.Model_code, DbType.Int32);
        parameters.Add("Implant_position", vc.Implant_position, DbType.String);
        parameters.Add("uk_code", vc.uk_code, DbType.String);
        parameters.Add("us_code", vc.us_code, DbType.String);
        parameters.Add("image", vc.image, DbType.String);
        parameters.Add("description", vc.Description, DbType.String);
        parameters.Add("type", vc.Type, DbType.String);
        parameters.Add("countries", vc.countries, DbType.String);

        using (var connection = _context.CreateConnection())
        {
            var id = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);
            if (id != 0)
            {
                var createdValveCode = new Valve_Code
                {
                    ValveTypeId = id,
                    hospitalId = vc.hospitalId,
                    No = vc.No,
                    Vendor_description = vc.Vendor_description,
                    Vendor_code = vc.Vendor_code,
                    Valve_size = vc.Valve_size,
                    Model_code = vc.Model_code,
                    Implant_position = vc.Implant_position,
                    uk_code = vc.uk_code,
                    us_code = vc.us_code,
                    image = vc.image,
                    Description = vc.Description,
                    Type = vc.Type,
                    countries = vc.countries,
                };
                return createdValveCode;
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<Valve_Code> updateValveCode(Valve_Code pv)
    {
        var query =
            @"UPDATE ValveCodes SET No=@No,hospitalId=@hospitalId,Vendor_description=@Vendor_description,"
            + "Vendor_code=@Vendor_code,Model_code=@Model_code,Implant_position=@Implant_position,"
            + "uk_code=@uk_code,us_code=@us_code,image=@image,Description=@Description,Type=@Type,countries=@countries "
            + "WHERE ValveTypeId=@ValveTypeId";

        var parameters = new DynamicParameters();

        parameters.Add("ValveTypeId", pv.ValveTypeId);
        parameters.Add("No", pv.No);
        parameters.Add("hospitalId", pv.hospitalId);
        parameters.Add("Vendor_description", pv.Vendor_description);
        parameters.Add("Vendor_code", pv.Vendor_code);
        parameters.Add("Model_code", pv.Model_code);
        parameters.Add("Implant_position", pv.Implant_position);
        parameters.Add("Type", pv.Type);
        parameters.Add("countries", pv.countries);
        parameters.Add("uk_code", pv.uk_code);
        parameters.Add("us_code", pv.us_code);
        parameters.Add("image", pv.image);
        parameters.Add("Type", pv.Type);
        parameters.Add("Description", pv.Description);

        using (var connection = _context.CreateConnection())
        {
            try
            {
                await connection.ExecuteAsync(query, parameters);
            }
            catch (System.Exception e)
            {
                Console.Write(e.InnerException);
            }
        }
        return pv;
    }

    public async Task<int> deleteValveCode(int id)
    {
        var query = @"Delete FROM ValveCodes WHERE ValveTypeId = @id";
        using (var connection = _context.CreateConnection())
        {
            try
            {
                await connection.ExecuteAsync(query, new { id });
                return 1;
            }
            catch (System.Exception e)
            {
                Console.Write(e.InnerException);
                return 0;
            }
        }
    }

    public async Task<List<ValveCodeSizesDTO>?> getValveCodeSizes(int ValveTypeId)
    {
        var vcs_list = new List<ValveCodeSizesDTO>();
        var query = "SELECT * FROM ValveSizes WHERE VTValveTypeId = @ValveTypeId";
        using (var connection = _context.CreateConnection())
        {
            var documents = await connection.QueryAsync<Valve_Size>(query, new { ValveTypeId });
            foreach (Valve_Size vs in documents.ToList())
            {
                var vcs = new ValveCodeSizesDTO();
                vcs.eoa = vs.EOA;
                vcs.size = vs.Size;
                vcs_list.Add(vcs);
            }
            return vcs_list;
        }
    }

    public async Task<List<Class_Item>> getValveCodesPerCountry(string currentCountry)
    {
        var list = new List<Class_Item>();
        var query = "SELECT * FROM ValveCodes";
        using (var connection = _context.CreateConnection())
        {
            var documents = await connection.QueryAsync<Valve_Code>(query);
            foreach (Valve_Code vc in documents)
            {
                if (vc.countries != null)
                {
                    var countryArray = vc.countries.Split(',');
                    if (countryArray.Contains(currentCountry))
                    {
                        var item = new Class_Item();
                        item.value = vc.ValveTypeId;
                        item.description = vc.Description;
                        list.Add(item);
                    }
                }
                else { }
            }
            return list;
        }
    }

    public async Task<List<Valve_Code>?> getAllProducts()
    {
        var query = "SELECT * FROM ValveCodes";
        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryAsync<Valve_Code>(query);
            if (result != null)
            {
                return result.ToList();
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<Valve_Code?> getDetailsByProductCode(string code)
    {
        var query = "SELECT * FROM ValveCodes WHERE uk_code = @code";
        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryFirstOrDefaultAsync<Valve_Code>(query, new { code });
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<Valve_Code?> getDetailsByNo(int code)
    {
        var query = "SELECT * FROM ValveCodes WHERE No = @code";
        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryFirstOrDefaultAsync<Valve_Code>(query, new { code });
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<Valve_Code?> getDetailsByValveTypeId(int ValveTypeId)
    {
        var query = "SELECT * FROM ValveCodes WHERE ValveTypeId = @ValveTypeId";
        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryFirstOrDefaultAsync<Valve_Code>(
                query,
                new { ValveTypeId }
            );
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }

    private async Task<int> getNewHospitalNo()
    {
        var help = 0;
        var allHospitalNumbers = new List<int>();
        // get AllValveCodes
        var allValveCodes = await getAllProducts();
        if (allValveCodes != null)
        {
            foreach (Valve_Code vc in allValveCodes)
            {
                allHospitalNumbers.Add(vc.No);
            }
            help = allHospitalNumbers.Count();
            while (allHospitalNumbers.Contains(help))
            {
                help++;
            }
        }
        return help;
    }

    #endregion

    #region <!-- ValveSize-->

    public async Task<List<Valve_Size>?> getSizesForValve(int vid)
    {
        var query = "SELECT * FROM ValveSizes WHERE VTValveTypeId= @vid";
        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryAsync<Valve_Size>(query, new { vid });
            if (result != null)
            {
                return result.ToList();
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<Valve_Size?> getSize(int cid)
    {
        var query = "SELECT * FROM ValveSizes WHERE SizeId= @cid";
        using (var connection = _context.CreateConnection())
        {
            var result = await connection.QueryFirstOrDefaultAsync<Valve_Size>(query, new { cid });
            if (result != null)
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }

    public async Task<int> deleteValveSize(int sid)
    {
        var query = @"Delete FROM ValveSizes WHERE SizeId = @sid";
        using (var connection = _context.CreateConnection())
        {
            try
            {
                await connection.ExecuteAsync(query, new { sid });
                return 1;
            }
            catch (System.Exception e)
            {
                Console.Write(e.InnerException);
                return 0;
            }
        }
    }

    public async Task<Valve_Size> updateValveSize(Valve_Size vs)
    {
        var query =
            @"UPDATE ValveSizes SET VTValveTypeId=@VTValveTypeId,Size=@Size,EOA=@EOA,ValveTypeId=@ValveTypeId WHERE SizeId=@SizeId";

        var parameters = new DynamicParameters();
        parameters.Add("SizeId", vs.SizeId);
        parameters.Add("VTValveTypeId", vs.VTValveTypeId);
        parameters.Add("Size", vs.Size);
        parameters.Add("EOA", vs.EOA);
        parameters.Add("ValveTypeId", vs.ValveTypeId);

        using (var connection = _context.CreateConnection())
        {
            try
            {
                await connection.ExecuteAsync(query, parameters);
            }
            catch (System.Exception e)
            {
                Console.Write(e.InnerException);
            }
        }
        return vs;
    }

    public async Task<Valve_Size?> addValveSize(Valve_Size vs)
    {
        var query =
            "INSERT INTO ValveSizes (VTValveTypeId,Size,EOA,ValveTypeId)"
            + "VALUES (@VTValveTypeId,@Size,@EOA,@ValveTypeId);"
            + " SELECT LAST_INSERT_ID() FROM ValveSizes";

        var parameters = new DynamicParameters();

        parameters.Add("VTValveTypeId", vs.VTValveTypeId);
        parameters.Add("Size", vs.Size);
        parameters.Add("EOA", vs.EOA);
        parameters.Add("ValveTypeId", vs.ValveTypeId);

        using (var connection = _context.CreateConnection())
        {
            var id = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);
            if (id != 0)
            {
                var createdValveSize = new Valve_Size
                {
                    SizeId = id,
                    Size = vs.Size,
                    EOA = vs.EOA,
                    VTValveTypeId = vs.VTValveTypeId
                };
                return createdValveSize;
            }
            else
            {
                return null;
            }
        }
    }

    #endregion

    #region <!-- Vendor Stuff-->
    public async Task<List<Valve_Code>?> getAllProductsByVTP(
        int vendor,
        string type,
        string position
    )
    {
        var vctr = new List<Valve_Code>();
        var query =
            "SELECT * FROM ValveCodes WHERE Vendor_code = @vendor AND Implant_position = @position AND Type = @type";
        using (var connection = _context.CreateConnection())
        {
            var documents = await connection.QueryAsync<Valve_Code>(
                query,
                new
                {
                    vendor,
                    type,
                    position
                }
            );
            foreach (Valve_Code vc in documents)
            {
                vctr.Add(vc);
            }
            return vctr;
        }
    }

    public async Task<List<Valve_Code>?> getAllProductsByVC(int vendor, string isoCountryCode)
    {
        var vctr = new List<Valve_Code>();
        var query = "SELECT * FROM ValveCodes WHERE Vendor_code = @vendor";
        using (var connection = _context.CreateConnection())
        {
            var documents = await connection.QueryAsync<Valve_Code>(query, new { vendor });
            foreach (Valve_Code vc in documents)
            {
                if (vc.countries != null)
                {
                    var help = vc.countries.Split(',');
                    var countryList = help.ToList();
                    if (isoCountryCode == "All")
                    {
                        vctr.Add(vc);
                    }
                    else
                    {
                        if (countryList.Contains(isoCountryCode))
                        {
                            vctr.Add(vc);
                        }
                    }
                }
            }
            return vctr;
        }
    }

    public async Task<List<Class_Item>?> getAllProductsItemsByVC(int vendor, string isoCountryCode)
    {
        var vctr = new List<Class_Item>();
        var query = "SELECT * FROM ValveCodes WHERE Vendor_code = @vendor";
        using (var connection = _context.CreateConnection())
        {
            var documents = await connection.QueryAsync<Valve_Code>(query, new { vendor });
            foreach (Valve_Code vc in documents)
            {
                if (isoCountryCode == "All")
                {
                    var it = new Class_Item();
                    it.value = vc.No;
                    it.description = vc.Description;
                    vctr.Add(it);
                }
                else
                {
                    if (vc.countries != null)
                    {
                        var help = vc.countries.Split(',');
                        var countryList = help.ToList();

                        if (countryList.Contains(isoCountryCode))
                        {
                            var it = new Class_Item();
                            it.value = vc.No;
                            it.description = vc.Description;
                            vctr.Add(it);
                        }
                    }
                }
            }
            return vctr;
        }
    }

    public async Task<List<ValveSizeForReturnDTO>?> getSizesForSuggestedValves(
        float minid,
        float maxid,
        string soort
    )
    {
        List<ValveSizeForReturnDTO> result = new();
        if (soort == "Mechanical")
        {
            var mech = new MechanicalValves(_context);
            result = await mech.GetMechanical(minid, maxid, soort);
        }
        else if (soort == "Biological")
        {
            var bio = new BiologicalValves(_context);
            result = await bio.GetBiological(minid, maxid, soort);
        }
        foreach (ValveSizeForReturnDTO vr in result)
        {
            var valveCodeDetails = await getDetailsByValveTypeId(vr.VTValveTypeId);
            vr.VendorName = valveCodeDetails?.Vendor_description ?? string.Empty;
            vr.ModelName = valveCodeDetails?.Description ?? string.Empty;
        }
        return result;
    }

    private async Task<List<string?>> getVendorNames()
    {
        var query = "SELECT Vendor_description FROM ValveCodes";
        using (var connection = _context.CreateConnection())
        {
            var vendor_names = await connection.QueryAsync<Valve_Code>(query);
            return vendor_names.Select(vn => vn.Vendor_description).Distinct().ToList();
        }
    }

    public async Task<List<ValveSizeForReturnDTO>?> getFilteredValveSizes(
        List<ValveSizeForReturnDTO> sizesList
    )
    {
        List<ValveSizeForReturnDTO> filteredList = new();
        foreach (ValveSizeForReturnDTO size in sizesList)
        {
            if (size.Size == sizesList[0].Size)
            {
                filteredList.Add(size);
            }
        }
        return filteredList;
    }

    #endregion
}
