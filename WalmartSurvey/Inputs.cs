namespace WalmartSurvey
{
    static internal class Inputs
    {
        // Data to be updated each use
        public const string TCNumber = "1234 5678 9012 3456 78901"; // Unique 21 digit can include spaces for readability
        public const string VisitTime = "11:00 AM"; // Times in "11:00 AM" format, whole hour times only.
        public const string VisitDateOverride = @"11/18/2019"; // mm/dd/yyyy Provide actual value if visit was not today; otherwise leave empty
        

        // Connection/driver configuration
        public const string ChromeDriverDirectory = @"\\MAIN\Portal\Kevin\Selenium";
        //public const string ChromeDriverDirectory = @"C:\Users\admin\Desktop\Portal\Kevin\Selenium";
        public const string Url = @"http://survey.walmart.ca/";
        

        // Data that does not usually change
        public const int BirthYear = 1980;
        public const string Language = "English";
        public const string PostalCode = "V8P4A5";
        public const string Province = "British Columbia";
        public const int SkillTestAnswer = 5;
        public const int StoreNumber = 3109;
        public const int TCNumberLength = 21;
        
    }

}
