namespace Yum_Now.Controls;

public partial class TagCardView : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(TagCardView),
        string.Empty);

    public static readonly BindableProperty TagValueProperty = BindableProperty.Create(
        nameof(TagValue),
        typeof(string),
        typeof(TagCardView),
        string.Empty);

    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
        nameof(IsSelected),
        typeof(bool),
        typeof(TagCardView),
        false,
        propertyChanged: OnIsSelectedChanged);

    public event EventHandler? Clicked;

    public TagCardView()
    {
        InitializeComponent();
        UpdateVisualState();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string TagValue
    {
        get => (string)GetValue(TagValueProperty);
        set => SetValue(TagValueProperty, value);
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private static void OnIsSelectedChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TagCardView card)
        {
            card.UpdateVisualState();
        }
    }

    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, IsSelected ? "Selected" : "Normal");
    }

    private void OnButtonClicked(object? sender, EventArgs e)
    {
        Clicked?.Invoke(this, e);
    }
}
