using Microsoft.Maui.Controls.Shapes;

namespace TTT;

public partial class StatsPage : ContentPage
{
    // Лейблы для статистики двух игроков
    private Label _lblWinsXPvp;
    private Label _lblWinsOPvp;
    private Label _lblDrawsPvp;

    // Лейблы для статистики против бота
    private Label _lblWinsXBot;
    private Label _lblWinsOBot;
    private Label _lblDrawsBot;

    // Лейбл уровня бота
    private Label _lblBotGames;

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

        // --- БЛОК: ДВА ИГРОКА ---
        var pvpTitle = new Label
        {
            Text = "👥 Mängija vs Mängija",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        _lblWinsXPvp = new Label
        {
            FontSize = 18,
            TextColor = Color.FromArgb("#e94560"),
            HorizontalOptions = LayoutOptions.Center
        };

        _lblWinsOPvp = new Label
        {
            FontSize = 18,
            TextColor = Color.FromArgb("#0f9b58"),
            HorizontalOptions = LayoutOptions.Center
        };

        _lblDrawsPvp = new Label
        {
            FontSize = 18,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var pvpFrame = new Border
        {
            BackgroundColor = Color.FromArgb("#16213e"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Color.FromArgb("#e94560"),
            Padding = new Thickness(20),
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children = { _lblWinsXPvp, _lblWinsOPvp, _lblDrawsPvp }
            }
        };

        // --- БЛОК: ПРОТИВ БОТА ---
        var botStatsTitle = new Label
        {
            Text = "🤖 Mängija vs Bot",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        _lblWinsXBot = new Label
        {
            FontSize = 18,
            TextColor = Color.FromArgb("#e94560"),
            HorizontalOptions = LayoutOptions.Center
        };

        _lblWinsOBot = new Label
        {
            FontSize = 18,
            TextColor = Color.FromArgb("#0f9b58"),
            HorizontalOptions = LayoutOptions.Center
        };

        _lblDrawsBot = new Label
        {
            FontSize = 18,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var botStatsFrame = new Border
        {
            BackgroundColor = Color.FromArgb("#16213e"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Color.FromArgb("#e94560"),
            Padding = new Thickness(20),
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children = { _lblWinsXBot, _lblWinsOBot, _lblDrawsBot }
            }
        };

        // --- БЛОК: УРОВЕНЬ БОТА ---
        var botLevelTitle = new Label
        {
            Text = "🤖 Boti tase",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        _lblBotGames = new Label
        {
            FontSize = 18,
            TextColor = Color.FromArgb("#e94560"),
            HorizontalOptions = LayoutOptions.Center
        };

        var botLevelFrame = new Border
        {
            BackgroundColor = Color.FromArgb("#16213e"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Color.FromArgb("#e94560"),
            Padding = new Thickness(20),
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children = { _lblBotGames }
            }
        };

        // --- КНОПКИ СБРОСА ---

        // Сбрасывает только статистику побед — бот не трогается
        var btnResetStats = new Button
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
        btnResetStats.Clicked += OnResetStatsClicked;

        // Сбрасывает только память и уровень бота — статистика не трогается
        var btnResetBot = new Button
        {
            Text = "🔄  Lähtesta boti tase",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnResetBot.Clicked += OnResetBotClicked;

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

        // ScrollView — чтобы всё влезло на маленький экран
        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Padding = new Thickness(24),
                Children =
                {
                    title,
                    pvpTitle, pvpFrame, btnResetStats,
                    botStatsTitle, botStatsFrame,
                    botLevelTitle, botLevelFrame, btnResetBot,
                    btnBack
                }
            }
        };
    }

    // Обновляем все данные каждый раз когда открываем страницу
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadStats();
    }

    private void LoadStats()
    {
        // Статистика двух игроков — ключи с суффиксом "_pvp"
        _lblWinsXPvp.Text = $"X võitis: {Preferences.Get("wins_x_pvp", 0)}";
        _lblWinsOPvp.Text = $"O võitis: {Preferences.Get("wins_o_pvp", 0)}";
        _lblDrawsPvp.Text = $"Viik: {Preferences.Get("draws_pvp", 0)}";

        // Статистика против бота — ключи с суффиксом "_bot"
        _lblWinsXBot.Text = $"Mängija võitis: {Preferences.Get("wins_x_bot", 0)}";
        _lblWinsOBot.Text = $"Bot võitis: {Preferences.Get("wins_o_bot", 0)}";
        _lblDrawsBot.Text = $"Viik: {Preferences.Get("draws_bot", 0)}";

        // Уровень бота
        int gamesWithBot = Preferences.Get("bot_games_played", 0);
        _lblBotGames.Text = $"Mängud botiga: {gamesWithBot}\n{GetBotLevelText(gamesWithBot)}";
    }

    // Переводим число игр в понятный текст уровня
    private string GetBotLevelText(int games)
    {
        if (games <= 2) return "Tase: Algaja 🟢";
        if (games <= 6) return "Tase: Kesktase 🟡";
        if (games <= 10) return "Tase: Kogenud 🟠";
        return "Tase: Meister 🔴";
    }

    // Сбрасываем только статистику побед (pvp и bot отдельно)
    private async void OnResetStatsClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Kinnita", "Kas soovid statistika lähtestada?", "Jah", "Ei");

        if (confirm)
        {
            // Удаляем все 6 ключей статистики
            Preferences.Remove("wins_x_pvp");
            Preferences.Remove("wins_o_pvp");
            Preferences.Remove("draws_pvp");
            Preferences.Remove("wins_x_bot");
            Preferences.Remove("wins_o_bot");
            Preferences.Remove("draws_bot");
            LoadStats();
        }
    }

    // Сбрасываем только память и уровень бота
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

            LoadStats();
            await DisplayAlertAsync("✅", "Bot on lähtestatud! Ta on jälle nõrk.", "OK");
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}