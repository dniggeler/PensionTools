namespace Application.Features.CheckSettings
{
    public interface ICheckSettingsConnector
    {
        Task<Dictionary<string, string>> GetAsync();
    }
}
