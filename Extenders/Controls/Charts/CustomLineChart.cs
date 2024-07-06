namespace BTGDesktop;

public partial class CustomLineChart : SKCanvasView
{
    #region Variables
    /// <summary>
    /// Gets or sets the minimum value from entries. If not defined, it will be the minimum between zero and the 
    /// minimal entry value.
    /// </summary>
    /// <value>The minimum value.</value>
    public float MinValue
    {
        get
        {
            if (!this.Definition.VerticalAxisPointsDefinition.Any())
            {
                return 0;
            }

            if (this.InternalMinValue == null)
            {
                return Math.Min(0, (float)this.Definition.VerticalAxisPointsDefinition.SelectMany(x => x.Y).Min());
            }

            return Math.Min(this.InternalMinValue.Value, (float)this.Definition.VerticalAxisPointsDefinition.SelectMany(x => x.Y).Min());
        }

        set => this.InternalMinValue = value;
    }

    /// <summary>
    /// Gets or sets the maximum value from entries. If not defined, it will be the maximum between zero and the 
    /// maximum entry value.
    /// </summary>
    /// <value>The minimum value.</value>
    public float MaxValue
    {
        get
        {
            if (!this.Definition.VerticalAxisPointsDefinition.Any())
            {
                return 0;
            }

            if (this.InternalMaxValue == null)
            {
                return Math.Max(0, (float)this.Definition.VerticalAxisPointsDefinition.SelectMany(x => x.Y).Max());
            }

            return Math.Max(this.InternalMaxValue.Value, (float)this.Definition.VerticalAxisPointsDefinition.SelectMany(x => x.Y).Max());
        }

        set => this.InternalMaxValue = value;
    }

    /// <summary>
    /// Gets or sets the internal minimum value (that can be null).
    /// </summary>
    /// <value>The internal minimum value.</value>
    protected float? InternalMinValue { get; set; }

    /// <summary>
    /// Gets or sets the internal max value (that can be null).
    /// </summary>
    /// <value>The internal max value.</value>
    protected float? InternalMaxValue { get; set; }

    private float ValueRange => this.MaxValue - this.MinValue;

    bool _initialized;

    float _scaleResolution = 1.0f;
    int _scaleGraphic = 10;
    #endregion
    #region Constructor
    public CustomLineChart() => PaintSurface += OnPaint;

    CustomLineChartDefinition _customLineChartDefinition;
    public CustomLineChartDefinition Definition
    {
        get => _customLineChartDefinition;
        set
        {
            _customLineChartDefinition = value;
            _initialized = false;
            InvalidateSurface();
        }
    }
    #endregion
    #region Properties
    #region LineChartDefinition
    public static readonly BindableProperty LineChartDefinitionProperty = BindableProperty.Create(
        propertyName: nameof(LineChartDefinition),
        returnType: typeof(CustomLineChartDefinition),
        declaringType: typeof(CustomLineChart),
        defaultValue: null,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: LineChartDefinitionPropertyChanged);
    public CustomLineChartDefinition LineChartDefinition
    {
        get => (CustomLineChartDefinition)GetValue(LineChartDefinitionProperty);
        set => SetValue(LineChartDefinitionProperty, value);
    }

    static void LineChartDefinitionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomLineChart)bindable;
        var lines = (CustomLineChartDefinition)newValue;
        control.Definition = lines;
    }
    #endregion

    #region ShowYValues
    public static readonly BindableProperty ShowYValuesProperty = BindableProperty.Create(
        propertyName: nameof(ShowYValues),
        returnType: typeof(bool),
        declaringType: typeof(CustomLineChart),
        defaultValue: true,
        defaultBindingMode: BindingMode.TwoWay);
    public bool ShowYValues
    {
        get => (bool)GetValue(ShowYValuesProperty);
        set => SetValue(ShowYValuesProperty, value);
    }
    #endregion
    #endregion
    #region Methods
    private void OnPaint(object sender, SKPaintSurfaceEventArgs e)
    {
        if (Definition != null && !_initialized)
        {
            _scaleResolution = (float)(e.Info.Width / ((SKCanvasView)sender).Width);
            DrawChart(e);

            _initialized = true;
        }
    }

    private void DrawChart(SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        var marginTop = 30.0f * _scaleResolution;
        var marginBottom = 55.0f * _scaleResolution;
        var marginX = 100.0f * _scaleResolution;
        var height = e.Info.Height - (marginTop + marginBottom);
        var width = e.Info.Width - (marginX * 2);


        if (Definition.HorizontalAxisPointsDefiniton != null && Definition.HorizontalAxisPointsDefiniton.Labels != null && Definition.HorizontalAxisPointsDefiniton.Labels.Any())
        {
            if (Definition.VerticalAxisPointsDefinition != null && Definition.VerticalAxisPointsDefinition != null && Definition.VerticalAxisPointsDefinition.Any())
            {
                var diferenca = Math.Abs(MaxValue - MinValue);
                diferenca = diferenca > 0 ? diferenca * -1 : diferenca;

                this.DrawArea(canvas, width, marginX, marginTop, height, diferenca);
                this.DrawCaption(canvas, width, height, marginX, marginTop, marginBottom, diferenca);
                this.DrawLines(canvas, width, height, marginX, marginTop);
            }
        }
    }

    private void DrawLines(SKCanvas canvas, float width, float height, float marginLeft, float marginTop)
    {
        var espacamentoPontos = width / (Definition.HorizontalAxisPointsDefiniton.Labels.Length - 1);

        foreach (var verticalAxisPointsDefinition in Definition.VerticalAxisPointsDefinition)
        {
            var paint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = verticalAxisPointsDefinition.LineColor,
                StrokeWidth = verticalAxisPointsDefinition.StrokeWidth * _scaleResolution,
                IsAntialias = true
            };

            var path = new SKPath();

            float y = marginTop + ((MaxValue - (float)verticalAxisPointsDefinition.Y[0]) / this.ValueRange) * height;
            path.MoveTo(marginLeft, y);

            for (var i = 0; i < Definition.HorizontalAxisPointsDefiniton.Labels.Length; i++)
            {
                if (verticalAxisPointsDefinition.Y != null && verticalAxisPointsDefinition.Y.Length > i)
                {
                    var x = marginLeft + (i * espacamentoPontos);
                    y = marginTop + (((MaxValue - (float)verticalAxisPointsDefinition.Y[i]) / this.ValueRange) * height);
                    path.LineTo(x, y);
                }
            }

            canvas.DrawPath(path, paint);
        }
    }

    private void DrawCaption(SKCanvas canvas, float width, float height, float marginLeft, float marginTop, float marginBottom, float variacao)
    {
        var espacamentoPontos = width / (Definition.HorizontalAxisPointsDefiniton.Labels.Length - 1);

        var legendaPaint = new SKPaint()
        {
            TextSize = 12 * _scaleResolution,
            IsAntialias = true,
            Color = new SKColor(255, 128, 128),
            IsStroke = false
        };

        var textBounds = new SKRect();
        legendaPaint.MeasureText("0", ref textBounds);

        var intervaloValoresLegenda = variacao / (_scaleGraphic - 2);
        var valorLegenda = MaxValue;

        if (ShowYValues)
        {
            for (var i = 1; i <= _scaleGraphic; i++)
            {
                var v = valorLegenda.FloatParaReal();
                var y = marginTop + (((MaxValue - valorLegenda) / this.ValueRange) * height);
                canvas.DrawText(v, 0, y, legendaPaint);
                valorLegenda += intervaloValoresLegenda;
            }
        }

        for (var i = 0; i < Definition.HorizontalAxisPointsDefiniton.Labels.Length; i++)
        {
            var textX = marginLeft + (i * espacamentoPontos);
            legendaPaint.TextAlign = SKTextAlign.Center;

            var textY = marginTop + marginBottom + height;

            canvas.DrawText(Definition.HorizontalAxisPointsDefiniton.Labels[i], textX, textY, legendaPaint);
        }
    }

    private void DrawArea(SKCanvas canvas, float width, float marginLeft, float marginTop, float height, float variacao)
    {
        var linhaPontilhadaPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = new SKColor(164, 164, 164),
            StrokeWidth = 1.0f * _scaleResolution,
            PathEffect = SKPathEffect.CreateDash(new float[] { 4 * _scaleResolution, 4 * _scaleResolution }, 4 * _scaleResolution),
            IsAntialias = false
        };

        var intervaloValoresLegenda = variacao / (_scaleGraphic - 2);
        var valorLegenda = MaxValue;

        for (int i = 1; i <= _scaleGraphic; i++)
        {
            var positionY = marginTop + (((MaxValue - valorLegenda) / this.ValueRange) * height);
            canvas.DrawLine(marginLeft, positionY, marginLeft + width, positionY, linhaPontilhadaPaint);
            valorLegenda += intervaloValoresLegenda;
        }
    }
    #endregion
}