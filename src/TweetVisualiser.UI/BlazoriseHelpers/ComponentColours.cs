using Blazorise.Charts;

namespace TweetVisualiser.UI.BlazoriseHelpers;
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable CA2211 // Non-constant fields should not be visible
public static class ComponentColours
{
    public static string[] Labels = { "Red", "Blue", "Yellow", "Green", "Purple", "Orange" };

    public static string[] BorderColours =
    {
            ChartColor.FromRgba(255, 99, 132, 1f).ToJsRgba(),
            ChartColor.FromRgba(54, 162, 235, 1f).ToJsRgba(),
            ChartColor.FromRgba(255, 206, 86, 1f).ToJsRgba(),
            ChartColor.FromRgba(75, 192, 192, 1f).ToJsRgba(),
            ChartColor.FromRgba(153, 102, 255, 1f).ToJsRgba(),
            ChartColor.FromRgba(255, 159, 64, 1f).ToJsRgba(),
        };

    public static string[] BackgroundColours =
    {
            ChartColor.FromRgba(255, 99, 132, 0.5f).ToJsRgba(),
            ChartColor.FromRgba(54, 162, 235, 0.5f).ToJsRgba(),
            ChartColor.FromRgba(255, 206, 86, 0.5f).ToJsRgba(),
            ChartColor.FromRgba(75, 192, 192, 0.5f).ToJsRgba(),
            ChartColor.FromRgba(153, 102, 255, 0.5f).ToJsRgba(),
            ChartColor.FromRgba(255, 159, 64, 0.5f).ToJsRgba(),
        };
}
