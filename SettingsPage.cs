using Microsoft.Maui.Controls.Shapes;

namespace TTT;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");
        Title = "Seaded";

        var title = new Label
        {
            Text = "Seaded",
            FontSize = 30,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var statsTitle = new Label
        {
            Text = " Statistika",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Start
        };

        var statsDesc = new Label
        {
            Text = "Kustutab statistika eraldi PvP ja bot režiimis.",
            FontSize = 14,
            TextColor = Color.FromArgb("#aaaaaa"),
            HorizontalOptions = LayoutOptions.Start
        };

        var btnResetPvP = new Button
        {
            Text = "Lähtesta PvP statistika",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnResetPvP.Clicked += OnResetPvPStatsClicked;

        var btnResetBotStats = new Button
        {
            Text = "Lähtesta bot statistika",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnResetBotStats.Clicked += OnResetBotStatsClicked;

        var btnResetBotLevel = new Button
        {
            Text = "Lähtesta boti tase",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnResetBotLevel.Clicked += OnResetBotLevelClicked;
        var btnResetHistory = new Button
        {
            Text = "Lähtesta turniiri ajalugu",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnResetHistory.Clicked += OnResetHistoryClicked;
        var statsFrame = new Border
        {
            BackgroundColor = Color.FromArgb("#16213e"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Color.FromArgb("#e94560"),
            Padding = new Thickness(16),
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Children =
                {
                    statsTitle,
                    statsDesc,
                    btnResetPvP,
                    btnResetBotStats,
                    btnResetBotLevel,
                    btnResetHistory
                }
            }
        };

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

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 20,
                Padding = new Thickness(24),
                Children =
                {
                    title,
                    statsFrame,
                    btnBack
                }
            }
        };
    }
    private async void OnResetHistoryClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Kinnita",
            "Kas kustutada turniiri ajalugu?",
            "Jah",
            "Ei");

        if (confirm)
        {
            Preferences.Remove("game_history");
            await DisplayAlertAsync("✅", "Turniiri ajalugu kustutatud!", "OK");
        }
    }
    private async void OnResetPvPStatsClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Kinnita",
            "Kas kustutada PvP statistika?",
            "Jah",
            "Ei");

        if (confirm)
        {
            Preferences.Remove("wins_x_pvp");
            Preferences.Remove("wins_o_pvp");
            Preferences.Remove("draws_pvp");

            await DisplayAlertAsync("✅", "PvP statistika kustutatud!", "OK");
        }
    }

    private async void OnResetBotStatsClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Kinnita",
            "Kas kustutada bot statistika?",
            "Jah",
            "Ei");

        if (confirm)
        {
            Preferences.Remove("wins_x_bot");
            Preferences.Remove("wins_o_bot");
            Preferences.Remove("draws_bot");

            await DisplayAlertAsync("✅", "Bot statistika kustutatud!", "OK");
        }
    }

    private async void OnResetBotLevelClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Kinnita",
            "Kas lähtestada boti tase ja mälu?",
            "Jah",
            "Ei");

        if (confirm)
        {
            Preferences.Remove("bot_games_played");

            for (int i = 0; i < 9; i++)
                Preferences.Remove($"bot_memory_{i}");

            await DisplayAlertAsync("✅", "Boti tase lähtestatud!", "OK");
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}