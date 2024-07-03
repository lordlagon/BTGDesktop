namespace BTGDesktop;

public partial class EntryFloatLabel : Grid
{
    int _topMargin = -32;
    int _titleFontSize = 12;
    int _placeholderFontSize = 14;

    public event EventHandler Completed;
    public event EventHandler<TextChangedEventArgs> TextChanged;

    #region Title
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        propertyName: nameof(Title),
        returnType: typeof(string),
        declaringType: typeof(EntryFloatLabel),
        defaultValue: string.Empty,
        defaultBindingMode: BindingMode.TwoWay);
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    #endregion

    #region Text
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        propertyName: nameof(Text),
        returnType: typeof(string),
        declaringType: typeof(EntryFloatLabel),
        defaultValue: string.Empty,
        defaultBindingMode: BindingMode.TwoWay,
        propertyChanged: HandlePropertyChangedDelegate);

    static void HandlePropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as EntryFloatLabel;
        if (!control.EntryField.IsFocused)
        {
            if (!string.IsNullOrWhiteSpace((string)newValue))
                control.TranslationToTitle(false);
            else
                control.TranslationToPlaceholder(false);

            control?.TextChanged?.Invoke(control.EntryField, new TextChangedEventArgs(oldValue as string, newValue as string));
        }
    }
    async Task TranslationToTitle(bool animated)
    {
        if (animated)
        {
            var t1 = LabelTitle.TranslateTo(0, _topMargin, 100);
            var t2 = SizeTo(_titleFontSize);
            await Task.WhenAll(t1, t2);
        }
        else
        {
            LabelTitle.TranslationX = 0;
            LabelTitle.TranslationY = -30;
            LabelTitle.FontSize = 12;
        }
    }
    async Task TranslationToPlaceholder(bool animated)
    {
        if (animated)
        {
            var t1 = LabelTitle.TranslateTo(10, 0, 100);
            var t2 = SizeTo(_placeholderFontSize);
            await Task.WhenAll(t1, t2);
        }
        else
        {
            LabelTitle.TranslationX = 10;
            LabelTitle.TranslationY = 0;
            LabelTitle.FontSize = _placeholderFontSize;
        }
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    #endregion

    #region ReturnType
    public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(
        propertyName: nameof(ReturnType),
        returnType: typeof(ReturnType),
        declaringType: typeof(EntryFloatLabel),
        defaultValue: ReturnType.Default,
        defaultBindingMode: BindingMode.TwoWay);
    public ReturnType ReturnType
    {
        get => (ReturnType)GetValue(ReturnTypeProperty);
        set => SetValue(ReturnTypeProperty, value);
    }
    #endregion

    #region IsPassword
    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        propertyName: nameof(IsPassword),
        returnType: typeof(bool),
        declaringType: typeof(EntryFloatLabel),
        defaultValue: default(bool));
    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }
    #endregion

    #region IsOnlyNumber
    public static readonly BindableProperty IsOnlyNumberProperty = BindableProperty.Create(
        propertyName: nameof(IsOnlyNumber),
        returnType: typeof(bool),
        declaringType: typeof(EntryFloatLabel),
        defaultValue: false);
    public bool IsOnlyNumber
    {
        get => (bool)GetValue(IsOnlyNumberProperty);
        set => SetValue(IsOnlyNumberProperty, value);
    }
    #endregion

    #region Keyboard
    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        propertyName: nameof(Keyboard),
        returnType: typeof(Keyboard),
        declaringType: typeof(EntryFloatLabel),
        defaultValue: Keyboard.Default,
        coerceValue: (o, v) => (Keyboard)v ?? Keyboard.Default);
    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }
    #endregion


    public EntryFloatLabel()
    {
        InitializeComponent();
        LabelTitle.TranslationX = 10;
        LabelTitle.FontSize = _placeholderFontSize;
    }

    public int MaxLength { get => EntryField.MaxLength; set => EntryField.MaxLength = value; }
    public new void Focus() { if (IsEnabled) EntryField.Focus(); }

    async void Handle_Focused(object sender, FocusEventArgs e)
    {
        if (string.IsNullOrEmpty(Text))
            await TranslationToTitle(true);
    }
    async void Handle_Unfocused(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(Text))
            await TranslationToPlaceholder(true);
    }
    Task SizeTo(int fontSize)
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();

        Action<double> callback = input => { LabelTitle.FontSize = input; };
        double startingHeight = LabelTitle.FontSize;
        double endingHeight = fontSize;
        uint rate = 5;
        uint length = 100;
        Easing easing = Easing.Linear;

        LabelTitle.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (v, c) => taskCompletionSource.SetResult(c));
        return taskCompletionSource.Task;
    }

    void Handle_Completed(object sender, EventArgs e) => Completed?.Invoke(this, e);
    void Handle_Tapped(object sender, EventArgs e) { if (IsEnabled) EntryField.Focus(); }
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(IsEnabled))
        {
            EntryField.IsEnabled = IsEnabled;
        }
    }

    private void EntryField_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue != null && IsOnlyNumber)
        {
            if (e.NewTextValue.Any(char.IsLetter) || e.NewTextValue.Contains(" "))
            {
                string result = Regex.Replace(e.NewTextValue, @"[a-zA-Z\s+]", "");
                var entry = sender as Entry;
                if (e.OldTextValue == null)
                {
                    entry.Text = string.Empty;
                }
                else
                {
                    entry.Text = result.Replace(" ","");
                }
            }            
        }
    }
}