namespace BTGDesktop;

public partial class SimulatorPage : ContentPage
{
    public SimulatorPage(SimulatorViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}