using Microsoft.Maui.Controls.Shapes;

namespace TTT;

public partial class StatsPage : ContentPage
{
    private Label _lblWinsX;
    private Label _lblWinsO;
    private Label _lblDraws;

    public StatsPage()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");
        Title = "Statistika";

        var title = new Label
        {
            Text = "📊 Statistika",
            FontSize = 30,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        _lblWinsX = new Label
        {
            FontSize = 20,
            TextColor = Color.FromArgb("#e94560"),
            HorizontalOptions = LayoutOptions.Center
        };

        _lblWinsO = new Label
        {
            FontSize = 20,
            TextColor = Color.FromArgb("#0f9b58"),
            HorizontalOptions = LayoutOptions.Center
        };

        _lblDraws = new Label
        {
            FontSize = 20,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var statsFrame = new Border
        {
            BackgroundColor = Color.FromArgb("#16213e"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Color.FromArgb("#e94560"),
            Padding = new Thickness(20),
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Children = { _lblWinsX, _lblWinsO, _lblDraws }
            }
        };

        var btnReset = new Button
        {
            Text = "🗑️  Lähtesta statistika",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnReset.Clicked += OnResetStatsClicked;

        var btnBack = new Button
        {
            Text = "← Tagasi",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#e94560"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48
        };
        btnBack.Clicked += OnBackClicked;

        Content = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 20,
            Padding = new Thickness(40),
            Children = { title, statsFrame, btnReset, btnBack }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadStats();
    }

    private void LoadStats()
    {
        _lblWinsX.Text = $"X võitis: {Preferences.Get("wins_x", 0)}";
        _lblWinsO.Text = $"O võitis: {Preferences.Get("wins_o", 0)}";
        _lblDraws.Text = $"Viik: {Preferences.Get("draws", 0)}";
    }

    private async void OnResetStatsClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Kinnita", "Kas soovid statistika lähtestada?", "Jah", "Ei");

        if (confirm)
        {
            Preferences.Remove("wins_x");
            Preferences.Remove("wins_o");
            Preferences.Remove("draws");
            LoadStats();
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}