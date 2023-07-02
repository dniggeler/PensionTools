using PensionCoach.Tools.CommonTypes;

namespace BlazorApp.ViewModels;

internal record GenderTypeViewModel
{
    public Gender Value { get; set; }

    public string Name { get; set; }
}
