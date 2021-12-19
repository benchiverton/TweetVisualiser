using System.Collections.Generic;

namespace TweetVisualiser.UI.BlazoriseHelpers;

public class HorizontalLineChartOptions
{
    public HorizontalLineChartOptions(string title, string yAxisLabel)
    {
        Title = new LineChartTitle(title);
        Scales = new LineChartScales(yAxisLabel);
    }

    public LineChartTitle Title { get; set; }
    public LineChartScales Scales { get; set; }
    public LineChartTooltips Tooltips { get; set; } = new LineChartTooltips();
    public LineChartHover Hover { get; set; } = new LineChartHover();
}

public class LineChartTitle
{
    public LineChartTitle(string title) => Text = title;

    public bool Display { get; set; } = true;
    public string Text { get; set; }
}

public class LineChartScales
{
    public LineChartScales(string yAxisLabel) => YAxes = new List<object>
        {
            new LineChartAxis(yAxisLabel)
        };

    public List<object> YAxes { get; set; }
}

public class LineChartAxis
{
    public LineChartAxis(string yAxisLabel) => ScaleLabel = new LineChartScaleLabel(yAxisLabel);

    public bool Display { get; set; } = true;
    public LineChartScaleLabel ScaleLabel { get; set; }
    public LineChartTicks Ticks { get; set; } = new LineChartTicks();
}

public class LineChartScaleLabel
{
    public LineChartScaleLabel(string yAxisLabel) => LabelString = yAxisLabel;

    public bool Display { get; set; } = true;
    public string LabelString { get; set; }
}

public class LineChartTicks
{
    public bool BeginAtZero { get; set; } = true;
}

public class LineChartTooltips
{
    public string Mode { get; set; } = "nearest";
    public bool Intersect { get; set; } = false;
}

public class LineChartHover
{
    public string Mode { get; set; } = "nearest";
    public bool Intersect { get; set; } = false;
}
