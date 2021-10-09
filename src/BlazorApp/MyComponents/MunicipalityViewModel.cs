using PensionCoach.Tools.CommonTypes;
using PensionCoach.Tools.CommonTypes.Municipality;

namespace BlazorApp.MyComponents
{
    public class MunicipalityViewModel : MunicipalityModel
    {
        public string NameAndCanton => $"{Name} ({Canton})";
    }
}
