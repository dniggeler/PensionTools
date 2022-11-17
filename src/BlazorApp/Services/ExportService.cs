using System.IO;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace BlazorApp.Services;

public class ExportService : IExportService
{
    private readonly NavigationManager navigationManager;

    public ExportService(NavigationManager navigationManager)
    {
        this.navigationManager = navigationManager;
    }

    public void Export(string table, string type, Query query = null)
    {
        string urlPath = Path.Combine("api/export/comparison/excel");

        navigationManager.NavigateTo(urlPath, true);
    }
}
