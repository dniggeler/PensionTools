namespace PensionCoach.Tools.PostOpenApi.Models
{
    public class ZipModel
    {
        public int BfsCode { get; set; }

        public string ZipCode { get; set; }

        public string ZipCodeAddOn { get; set; }

        public string MunicipalityName { get; set; }

        public string Canton { get; set; }
        
        public DateTime DateOfValidity { get; set; }

        public int LanguageCode { get; set; }
    }
}
