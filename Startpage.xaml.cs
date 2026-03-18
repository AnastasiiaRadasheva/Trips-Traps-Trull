namespace TTT;

public partial class StartPage : ContentPage
{
    public StartPage()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");

        var title = new Label
        {
            Text = "Trips-Traps-Trull",
            FontSize = 36,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var subtitle = new Label
        {
            Text = "X  O  X",
            FontSize = 28,
            TextColor = Color.FromArgb("#e94560"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 20)
        };

        var btnNewGame = new Button
        {
            Text = "🎮  Uus mäng",
            FontSize = 18,
            BackgroundColor = Color.FromArgb("#e94560"),
            TextColor = Colors.White,
            CornerRadius = 12,
            HeightRequest = 55,
            WidthRequest = 220
        };
        btnNewGame.Clicked += OnNewGameClicked;

        var btnStats = new Button
        {
            Text = "📊  Statistika",
            FontSize = 18,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 12,
            HeightRequest = 55,
            WidthRequest = 220,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnStats.Clicked += OnStatsClicked;

        var btnRules = new Button
        {
            Text = "📜  Reeglid",
            FontSize = 18,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 12,
            HeightRequest = 55,
            WidthRequest = 220,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnRules.Clicked += OnRulesClicked;

        Content = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 20,
            Padding = new Thickness(40),
            Children = { title, subtitle, btnNewGame, btnStats, btnRules }
        };
    }

    private async void OnNewGameClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("main");
    }

    private async void OnStatsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("StatsPage");
    }

    private async void OnRulesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("RulesPage");
    }
}