using PensionCoach.Tools.CommonTypes;

namespace BlazorApp.MyComponents
{
    public class MunicipalityViewModel : MunicipalityModel
    {
        public string NameAndCanton => $"{Name} ({Canton})";
    }
}
