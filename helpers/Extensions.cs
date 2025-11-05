namespace ValveService.helpers;

    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message)
        {

            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
       
        public static int CalculateAge(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            if (theDateTime.AddYears(age) > DateTime.Today) age--;
            return age;
        }
        public static string UppercaseFirst(this String inputString)
        {
            if (string.IsNullOrEmpty(inputString)) { return string.Empty; }
            return char.ToUpper(inputString[0]) + inputString.Substring(1).ToLower();
        }
        public static string makeSureTwoChar(this String inputString)
        {
            var help = "";
            if(inputString.Length == 1){help = "0" + inputString;} else {help = inputString;}
            return help;
        }
    }
