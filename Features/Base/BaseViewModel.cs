namespace BTGDesktop;

public abstract partial class BaseViewModel() : ObservableObject, IDisposable, IQueryAttributable
{
    public event BusyChange? BusyChanged;

    [ObservableProperty]
    bool _isLoading;

    bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (value == _isBusy)
                return;

            SetLoading(value);
            BusyChanged?.Invoke(value);

            SetProperty(ref _isBusy, value);
        }
    }

    public virtual void ApplyQueryAttributes(IDictionary<string, object> query) { }

    public void SetLoading(bool value)
        => IsLoading = value;

    public virtual Task InitializeAsync(object navigationData)
            => InitializeAsync();
    protected virtual Task InitializeAsync()
        => Task.FromResult(true);

    public virtual void Dispose() { }       
}
public delegate void BusyChange(bool isBusy);