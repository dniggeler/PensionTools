using Radzen;

namespace BlazorApp.Services;

public interface IExportService
{
    public void Export(string table, string type, Query query = null);
}
