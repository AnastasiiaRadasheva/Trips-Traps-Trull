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
            Text = "⚙️ Seaded",
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
            Text = "Kustutab kõik võidud, kaotused ja viigid — nii PvP kui ka boti vastu.",
            FontSize = 14,
            TextColor = Color.FromArgb("#aaaaaa"),
            HorizontalOptions = LayoutOptions.Start
        };

        var btnResetStats = new Button
        {
            Text = " Lähtesta statistika",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            HorizontalOptions = LayoutOptions.Fill,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnResetStats.Clicked += OnResetStatsClicked;

        var statsFrame = new Border
        {
            BackgroundColor = Color.FromArgb("#16213e"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Color.FromArgb("#e94560"),
            Padding = new Thickness(16),
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Children = { statsTitle, statsDesc, btnResetStats }
            }
        };

        var botTitle = new Label
        {
            Text = " Boti tase",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Start
        };

        var botDesc = new Label
        {
            Text = "Bot unustab sinu mänguharjumused ja muutub jälle nõrgaks algajaks.",
            FontSize = 14,
            TextColor = Color.FromArgb("#aaaaaa"),
            HorizontalOptions = LayoutOptions.Start
        };

        var btnResetBot = new Button
        {
            Text = " Lähtesta boti tase",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            HorizontalOptions = LayoutOptions.Fill,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnResetBot.Clicked += OnResetBotClicked;

        var botFrame = new Border
        {
            BackgroundColor = Color.FromArgb("#16213e"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Color.FromArgb("#e94560"),
            Padding = new Thickness(16),
            Content = new VerticalStackLayout
            {
                Spacing = 12,
                Children = { botTitle, botDesc, btnResetBot }
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
                Children = { title, statsFrame, botFrame, btnBack }
            }
        };
    }

    private async void OnResetStatsClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Kinnita",
            "Kas soovid kõik tulemused kustutada?",
            "Jah",
            "Ei");

        if (confirm)
        {
            Preferences.Remove("wins_x_pvp");
            Preferences.Remove("wins_o_pvp");
            Preferences.Remove("draws_pvp");
            Preferences.Remove("wins_x_bot");
            Preferences.Remove("wins_o_bot");
            Preferences.Remove("draws_bot");

            await DisplayAlertAsync("✅", "Statistika on kustutatud!", "OK");
        }
    }

    private async void OnResetBotClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync(
            "Kinnita",
            "Bot unustab kõik ja muutub jälle nõrgaks. Kas oled kindel?",
            "Jah",
            "Ei");

        if (confirm)
        {
            Preferences.Remove("bot_games_played");

            for (int i = 0; i < 9; i++)
                Preferences.Remove($"bot_memory_{i}");

            await DisplayAlertAsync("✅", "Bot on lähtestatud! Ta on jälle nõrk.", "OK");
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}