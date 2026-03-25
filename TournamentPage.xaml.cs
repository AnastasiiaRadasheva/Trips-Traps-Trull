namespace TTT;

public partial class TournamentPage : ContentPage
{
    private readonly TournamentManager _tournament = new();
    private readonly GameLogic _game = new();

    private Button[] _cells = Array.Empty<Button>();
    private Grid _gameGrid = null!;

    private Label _lblPhase = null!;
    private Label _lblCurrentPlayer = null!;
    private Button _btnRestart = null!;

    private readonly Dictionary<string, string> _playerNames = new()
    {
        { "X", "Mängija X" },
        { "O", "Mängija O" },
        { "Z", "Mängija Z" }
    };

    private static readonly Dictionary<string, Color> SymbolColors = new()
    {
        { "X", Color.FromArgb("#e94560") },
        { "O", Color.FromArgb("#0f9b58") },
        { "Z", Color.FromArgb("#4e8ef7") }
    };

    public TournamentPage()
    {
        BuildUI();

        Loaded += async (_, __) =>
        {
            _tournament.Start(new List<string> { "X", "O", "Z" });
            await StartPhase();
        };
    }

    private void BuildUI()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");

        var btnStats = new Button
        {
            Text = "📊",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 8,
            HeightRequest = 36,
            WidthRequest = 46,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnStats.Clicked += OnStatsClicked;

        var btnRules = new Button
        {
            Text = "📜",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 8,
            HeightRequest = 36,
            WidthRequest = 46,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnRules.Clicked += OnRulesClicked;

        var btnSettings = new Button
        {
            Text = "⚙️",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 8,
            HeightRequest = 36,
            WidthRequest = 46,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnSettings.Clicked += OnSettingsClicked;

        var topButtons = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.End,
            Spacing = 8,
            Children = { btnStats, btnRules, btnSettings }
        };

        _lblPhase = new Label
        {
            Text = "TURNIIR",
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#e94560"),
            HorizontalOptions = LayoutOptions.Center
        };

        _lblCurrentPlayer = new Label
        {
            Text = "",
            FontSize = 20,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        _btnRestart = CreateButton("🔄 Uus turniir", "#16213e", OnRestartClicked);

        _gameGrid = new Grid
        {
            HeightRequest = 320,
            WidthRequest = 320,
            HorizontalOptions = LayoutOptions.Center,
            RowSpacing = 8,
            ColumnSpacing = 8
        };

        Content = new ScrollView
        {
            Content = new Grid
            {
                Children =
                {
                    new VerticalStackLayout
                    {
                        Spacing = 16,
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,
                        Children = { topButtons, _lblPhase, _lblCurrentPlayer, _gameGrid, _btnRestart }
                    }
                }
            }
        };
    }

    private static Button CreateButton(string text, string bg, EventHandler handler)
    {
        var btn = new Button
        {
            Text = text,
            BackgroundColor = Color.FromArgb(bg),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48
        };
        btn.Clicked += handler;
        return btn;
    }

    private void RebuildGrid(int size)
    {
        _gameGrid.Children.Clear();
        _gameGrid.RowDefinitions.Clear();
        _gameGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < size; i++)
        {
            _gameGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            _gameGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        _cells = new Button[size * size];

        for (int i = 0; i < _cells.Length; i++)
        {
            var btn = new Button
            {
                FontSize = size == 4 ? 26 : 34,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromArgb("#16213e"),
                TextColor = Colors.White,
                CornerRadius = 10,
                CommandParameter = i
            };
            btn.Clicked += OnCellClicked;
            _cells[i] = btn;
            _gameGrid.Add(btn, i % size, i / size);
        }
    }

    private async void OnRestartClicked(object? sender, EventArgs e)
    {
        _tournament.Reset();
        _gameGrid.Children.Clear();
        _lblPhase.Text = "TURNIIR";
        _lblCurrentPlayer.Text = "";
        _tournament.Start(new List<string> { "X", "O", "Z" });
        await StartPhase();
    }

    private async Task StartPhase()
    {
        _game.Reset(_tournament.ActivePlayers, _tournament.BoardSize);
        RebuildGrid(_tournament.BoardSize);
        UpdateStatus(_game.CurrentPlayer);

        _lblPhase.Text = _tournament.CurrentPhase switch
        {
            TournamentManager.Phase.Round1 => "VOOR 1 (4×4)",
            TournamentManager.Phase.Round2 => "VOOR 2 (3×3)",
            TournamentManager.Phase.Final => "FINAAL (3×3)",
            _ => _lblPhase.Text
        };
    }

    private async void OnCellClicked(object? sender, EventArgs e)
    {
        if (_tournament.CurrentPhase == TournamentManager.Phase.Finished)
            return;

        var btn = (Button)sender!;
        int index = (int)btn.CommandParameter!;

        if (!_game.MakeMove(index))
            return;

        SetCell(btn, _game.CurrentPlayer);

        var result = _game.CheckResult();
        if (result != null)
        {
            await HandleResult(result);
            return;
        }

        _game.SwitchPlayer_1();
        UpdateStatus(_game.CurrentPlayer);
    }

    private void SaveTournamentResult()
    {
        string key = "game_history";
        string history = Preferences.Get(key, "");

        string entry =
            $"{DateTime.Now:dd.MM HH:mm}\n" +
            $"🥇 {NameOf(_tournament.First)}\n" +
            $"🥈 {NameOf(_tournament.Second)}\n" +
            $"🥉 {NameOf(_tournament.Third)}";

        var list = history.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
        list.Insert(0, entry);
        if (list.Count > 10) list = list.Take(10).ToList();

        Preferences.Set(key, string.Join("|", list));
    }

    private async Task HandleResult(string result)
    {
        if (result == "Draw")
        {
            await DisplayAlertAsync("Viik", "Voori kordus", "OK");
            await StartPhase();
            return;
        }

        _tournament.RegisterResult(result);

        if (_tournament.CurrentPhase == TournamentManager.Phase.Finished)
        {
            SaveTournamentResult();
            await DisplayAlertAsync("Tulemused",
                $"🥇 {NameOf(_tournament.First)}\n" +
                $"🥈 {NameOf(_tournament.Second)}\n" +
                $"🥉 {NameOf(_tournament.Third)}",
                "OK");
            await StartPhase();
            return;
        }

        await DisplayAlertAsync("Voor lõppenud", "Järgmine voor algab", "OK");
        await StartPhase();
    }

    private void UpdateStatus(string current) =>
        _lblCurrentPlayer.Text = $"Käik: {NameOf(current)} ({current})";

    private void SetCell(Button btn, string symbol)
    {
        btn.Text = symbol;
        btn.TextColor = SymbolColors.ContainsKey(symbol) ? SymbolColors[symbol] : Colors.White;
    }

    private async void OnStatsClicked(object? sender, EventArgs e) =>
        await Navigation.PushAsync(new StatsPage());

    private async void OnRulesClicked(object? sender, EventArgs e) =>
        await Navigation.PushAsync(new RulesPage());

    private async void OnSettingsClicked(object? sender, EventArgs e) =>
        await Navigation.PushAsync(new SettingsPage());

    private string NameOf(string symbol) =>
        _playerNames.ContainsKey(symbol) ? _playerNames[symbol] : symbol;
}