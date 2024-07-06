namespace BTGDesktop;

public class VerticalAxisPointsDefinition
{
    public double[] Y { get; set; } = [];
    public SKColor LineColor { get; set; } = new SKColor(0, 0, 0);
    public SKColor PointColor { get; set; } = new SKColor(0, 0, 0);
    public bool HasGradient { get; set; } = false;
    public float StrokeWidth { get; set; } = 1.0f;
}
