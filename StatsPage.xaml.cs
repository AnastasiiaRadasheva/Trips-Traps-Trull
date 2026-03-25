using Microsoft.Maui.Controls.Shapes;

namespace TTT;

public partial class StatsPage : ContentPage
{
    private Label _lblWinsXPvp;
    private Label _lblWinsOPvp;
    private Label _lblDrawsPvp;

    private VerticalStackLayout _historyLayout;

    private Label _lblWinsXBot;
    private Label _lblWinsOBot;
    private Label _lblDrawsBot;

    private Label _lblBotGames;

    public StatsPage()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");
        Title = "Statistika";

        var title = new Label
        {
            Text = " Statistika",
            FontSize = 30,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var pvpTitle = new Label
        {
            Text = "Mängija vs Mängija",
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

        var botStatsTitle = new Label
        {
            Text = "Mängija vs Bot",
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

        var botLevelTitle = new Label
        {
            Text = "Boti tase",
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
        var historyTitle = new Label
        {
            Text = "Viimased mängud",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        _historyLayout = new VerticalStackLayout
        {
            Spacing = 6
        };

        var historyScroll = new ScrollView
        {
            HeightRequest = 150, 
            Content = _historyLayout
        };

        var historyFrame = new Border
        {
            BackgroundColor = Color.FromArgb("#16213e"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Color.FromArgb("#e94560"),
            Padding = new Thickness(20),
            Content = historyScroll 
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
                Spacing = 16,
                Padding = new Thickness(24),
                Children =
                {
                    title,
                    pvpTitle, pvpFrame, 
                    botStatsTitle, botStatsFrame,
                    botLevelTitle, botLevelFrame,
                    historyTitle,
historyFrame,
                    btnBack
                }
            }
        };
    }
    private void LoadHistory()
    {
        _historyLayout.Children.Clear();

        string history = Preferences.Get("game_history", "");

        var list = history.Split('|', StringSplitOptions.RemoveEmptyEntries);

        if (list.Length == 0)
        {
            _historyLayout.Children.Add(new Label
            {
                Text = "Pole veel mänge",
                TextColor = Colors.Gray,
                HorizontalOptions = LayoutOptions.Center
            });
            return;
        }
        foreach (var item in list)
        {
            _historyLayout.Children.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#0f3460"),
                StrokeShape = new RoundRectangle { CornerRadius = 10 },
                Padding = new Thickness(10),
                Content = new Label
                {
                    Text = item,
                    TextColor = Colors.White,
                    FontSize = 14
                }
            });
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadHistory();
        LoadStats();
    }

    private void LoadStats()
    {
        _lblWinsXPvp.Text = $"X võitis: {Preferences.Get("wins_x_pvp", 0)}";
        _lblWinsOPvp.Text = $"O võitis: {Preferences.Get("wins_o_pvp", 0)}";
        _lblDrawsPvp.Text = $"Viik: {Preferences.Get("draws_pvp", 0)}";

        _lblWinsXBot.Text = $"Mängija võitis: {Preferences.Get("wins_x_bot", 0)}";
        _lblWinsOBot.Text = $"Bot võitis: {Preferences.Get("wins_o_bot", 0)}";
        _lblDrawsBot.Text = $"Viik: {Preferences.Get("draws_bot", 0)}";

        int gamesWithBot = Preferences.Get("bot_games_played", 0);
        _lblBotGames.Text = $"Mängud botiga: {gamesWithBot}\n{GetBotLevelText(gamesWithBot)}";
    }

    private string GetBotLevelText(int games)
    {
        if (games <= 2) return "Tase: Algaja 🟢";
        if (games <= 6) return "Tase: Kesktase 🟡";
        if (games <= 10) return "Tase: Kogenud 🟠";
        return "Tase: Meister 🔴";
    }


    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}