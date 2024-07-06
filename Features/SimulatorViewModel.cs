namespace BTGDesktop;

public partial class SimulatorViewModel(ISimulatorService simulatorService) : BaseViewModel
{
    #region Services
    readonly ISimulatorService _simulatorService = simulatorService;
    #endregion

    #region Properties
    [ObservableProperty]
    CustomLineChartDefinition definition;

    [ObservableProperty]
    double _initialPrice = 10;

    [ObservableProperty]
    double _volatility = 2;

    [ObservableProperty]
    double _mean = 5;

    [ObservableProperty]
    int _duration = 30;

    #endregion

    #region Methods
    [RelayCommand]
    void GenerateSimulator()
    {
        var prices = _simulatorService.GenerateBrownianMotion(Volatility.ToPercent(), Mean.ToPercent(), InitialPrice, Duration);

        Definition = new CustomLineChartDefinition
        {
            ChartType = 1,
            HorizontalAxisPointsDefiniton = new HorizontalAxisPointsDefiniton
            {
                Labels = GetLabels(prices)
            },
            VerticalAxisPointsDefinition = new List<VerticalAxisPointsDefinition>
            {
                new VerticalAxisPointsDefinition
                {
                    PointColor = new SKColor(12,120,216),
                    LineColor = new SKColor(12,120,216),
                    StrokeWidth = 2.5f,
                    Y = prices
                }
            }
        };
    }
    


    #endregion
    #region helpers
    string[] GetLabels(double[] prices)
    {
        List<string> labels = [];

        foreach(var price in prices) 
        {
            labels.Add(" ");
        }
        return [.. labels];
    }
    #endregion
}