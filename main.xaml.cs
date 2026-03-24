namespace TTT;

public partial class main : ContentPage
{
private int _turnNumber = 0;
    private readonly GameLogic _game = new GameLogic();
    private readonly BotLogic _bot = new BotLogic();
    private readonly Button[] _cells = new Button[9];
    private Label _lblCurrentPlayer = null!;
    private Label _lblBotLevel = null!;
    private Picker _pickerDifficulty = null!;
    private bool _isBotMode = false;

    private const string PlayerSymbol = "X";
    private const string BotSymbol = "O";

    public main()
    {
        BuildUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _bot.ReloadMemory();
        if (_isBotMode)
            UpdateBotLevelLabel();
    }

    private void BuildUI()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");
        Title = "Trips-Traps-Trull";

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
            Spacing = 8
        };
        topButtons.Children.Add(btnStats);
        topButtons.Children.Add(btnRules);
        topButtons.Children.Add(btnSettings);

        _lblCurrentPlayer = new Label
        {
            Text = "Käib: X",
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        _lblBotLevel = new Label
        {
            FontSize = 14,
            TextColor = Color.FromArgb("#aaaaaa"),
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false
        };

        _pickerDifficulty = new Picker
        {
            Title = "Vali raskusaste",
            FontSize = 15,
            TextColor = Colors.White,
            BackgroundColor = Color.FromArgb("#16213e"),
            TitleColor = Color.FromArgb("#aaaaaa"),
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest = 220,
            IsVisible = false
        };
        _pickerDifficulty.Items.Add("🟢 Algaja");
        _pickerDifficulty.Items.Add("🟡 Kesktase");
        _pickerDifficulty.Items.Add("🟠 Kogenud");
        _pickerDifficulty.Items.Add("🔴 Meister");
        _pickerDifficulty.Items.Add("📈 Automaatne (kasvab)");
        _pickerDifficulty.SelectedIndex = 4;
        _pickerDifficulty.SelectedIndexChanged += OnDifficultyChanged;

        var gameGrid = BuildGrid();

        var btnNewGame = new Button
        {
            Text = "  Uus mäng",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#e94560"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            WidthRequest = 150
        };
        btnNewGame.Clicked += OnNewGameClicked;

        var btnRandomStart = new Button
        {
            Text = "  Kes alustab?",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            WidthRequest = 150,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnRandomStart.Clicked += OnRandomStartClicked;

        var btnToggleBot = new Button
        {
            Text = "Mängija  vs Bot",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            WidthRequest = 150,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1
        };
        btnToggleBot.Clicked += OnToggleBotClicked;

        var bottomButtons = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 12,
            Margin = new Thickness(0, 10, 0, 0),
            Children = { btnNewGame, btnRandomStart }
        };

        var bottomButtons2 = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 12,
            Children = { btnToggleBot }
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(20),
                Spacing = 16,
                VerticalOptions = LayoutOptions.Center,
                Children = { topButtons, _lblCurrentPlayer, _lblBotLevel, _pickerDifficulty, gameGrid, bottomButtons, bottomButtons2 }
            }
        };
    }

    private Grid BuildGrid()
    {
        var grid = new Grid
        {
            HeightRequest = 320,
            WidthRequest = 320,
            HorizontalOptions = LayoutOptions.Center,
            RowSpacing = 8,
            ColumnSpacing = 8
        };

        for (int i = 0; i < 3; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        for (int i = 0; i < 9; i++)
        {
            var btn = new Button
            {
                FontSize = 36,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.FromArgb("#16213e"),
                TextColor = Colors.White,
                CornerRadius = 10,
                CommandParameter = i
            };
            btn.Clicked += OnCellClicked;
            _cells[i] = btn;
            grid.Add(btn, i % 3, i / 3);
        }

        return grid;
    }

    private async void OnCellClicked(object? sender, EventArgs e)
    {
        var btn = (Button)sender!;
        int index = (int)btn.CommandParameter;

        if (_isBotMode && _game.CurrentPlayer != PlayerSymbol)
            return;

        if (!_game.MakeMove(index))
            return;
_turnNumber++;
        SetButtonSymbol(btn, _game.CurrentPlayer);

        if (_isBotMode)
        {
            int playerTurn = CountPlayerMoves() - 1;
            _bot.RecordPlayerMove(index, playerTurn);
        }

        string? result = _game.CheckResult();
        if (result != null)
        {
            await HandleResult(result);
            return;
        }

        _game.SwitchPlayer();
        _lblCurrentPlayer.Text = $"Käib: {_game.CurrentPlayer}";

        if (_isBotMode && _game.CurrentPlayer == BotSymbol)
            await MakeBotMove();
    }

    // Считаем сколько ходов уже сделал игрок (X) на доске
    private int CountPlayerMoves()
    {
        int count = 0;
        foreach (var cell in _game.Board)
            if (cell == PlayerSymbol) count++;
        return count;
    }

    private async Task MakeBotMove()
    {
        await Task.Delay(400);

        int botIndex = _bot.GetBestMove(_game.Board, BotSymbol);

        if (botIndex == -1 || !_game.MakeMove(botIndex))
            return;

        SetButtonSymbol(_cells[botIndex], BotSymbol);

        string? result = _game.CheckResult();
        if (result != null)
        {
            await HandleResult(result);
            return;
        }

        _game.SwitchPlayer();
        _lblCurrentPlayer.Text = $"Käib: {_game.CurrentPlayer}";
    }

    private void SetButtonSymbol(Button btn, string symbol)
    {
        btn.Text = symbol;
        btn.TextColor = symbol == "X"
            ? Color.FromArgb("#e94560")
            : Color.FromArgb("#0f9b58");
    }

    private async Task HandleResult(string result)
    {
        string message;

        if (result == "Draw")
        {
            message = "Viik! ";
            SaveStats("draw");
        }
        else if (_isBotMode)
        {
            message = result == PlayerSymbol
                ? "Sa võitsid boti! "
                : "Bot võitis! ";
            SaveStats(result == "X" ? "x" : "o");
        }
        else
        {
            message = $"{result} võitis! ";
            SaveStats(result == "X" ? "x" : "o");
        }

        if (_isBotMode)
        {
            _bot.RecordGameFinished();
            UpdateBotLevelLabel();
        }

        bool playAgain = await DisplayAlertAsync("Mäng läbi!", message, "Uus mäng", "Välju");

        if (playAgain)
            await ResetBoard();
        else
            await Shell.Current.GoToAsync("..");
    }

    private async void OnNewGameClicked(object? sender, EventArgs e) => await ResetBoard();

    private async void OnRandomStartClicked(object? sender, EventArgs e)
    {
        string starter = _game.RandomStartPlayer();
        await ResetBoard(starter);

        string msg = _isBotMode && starter == BotSymbol
            ? "Bot alustab! ⚡"
            : $"Alustab mängija {starter}!";

        await DisplayAlertAsync("🎲 Loosimine!", msg, "OK");
    }

    private async void OnToggleBotClicked(object? sender, EventArgs e)
    {
        _isBotMode = !_isBotMode;

        var btn = (Button)sender!;
        if (_isBotMode)
        {
            btn.BackgroundColor = Color.FromArgb("#e94560");
            btn.Text = "Mängija  vs Bot ON";
            _lblBotLevel.IsVisible = true;
            _pickerDifficulty.IsVisible = true;
            UpdateBotLevelLabel();
            await DisplayAlertAsync(" Bot", "Bot on sisse lülitatud! Sina mängid X-ga.", "OK");
        }
        else
        {
            btn.BackgroundColor = Color.FromArgb("#16213e");
            btn.Text = "Mängija  vs Bot";
            _lblBotLevel.IsVisible = false;
            _pickerDifficulty.IsVisible = false;
        }

        await ResetBoard();
    }

    private void OnDifficultyChanged(object? sender, EventArgs e)
    {
        int selected = _pickerDifficulty.SelectedIndex;
        _bot.ManualDifficulty = selected == 4 ? -1 : selected;
        UpdateBotLevelLabel();
    }

    private void UpdateBotLevelLabel()
    {
        if (_bot.ManualDifficulty >= 0)
        {
            string[] levels = { "🟢 Algaja", "🟡 Kesktase", "🟠 Kogenud", "🔴 Meister" };
            _lblBotLevel.Text = $"Boti tase: {levels[_bot.ManualDifficulty]}";
        }
        else
        {
            int games = _bot.GamesPlayed;
            string level;
            if (games <= 2) level = "🟢 Algaja";
            else if (games <= 6) level = "🟡 Kesktase";
            else if (games <= 10) level = "🟠 Kogenud";
            else level = "🔴 Meister";
            _lblBotLevel.Text = $"Boti tase: {level}  ({games} mängu)";
        }
    }

    private async void OnStatsClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("StatsPage");

    private async void OnRulesClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("RulesPage");

    private async void OnSettingsClicked(object? sender, EventArgs e) =>
        await Shell.Current.GoToAsync("SettingsPage");

    private async Task ResetBoard(string? startingPlayer = null)
    {
        // В бот-режиме без явного указания — случайно выбираем кто начинает
        if (_isBotMode && startingPlayer == null)
        {
            var rnd = new Random();
            startingPlayer = rnd.Next(2) == 0 ? PlayerSymbol : BotSymbol;
        }
        else
        {
            startingPlayer ??= "X";
        }

        _game.Reset(startingPlayer);

        foreach (var cell in _cells)
        {
            cell.Text = string.Empty;
            cell.BackgroundColor = Color.FromArgb("#16213e");
        }

        _lblCurrentPlayer.Text = $"Käib: {startingPlayer}";

        // Если первым ходит бот — сразу делаем его ход
        if (_isBotMode && startingPlayer == BotSymbol)
            await MakeBotMove();
    }

    private void SaveStats(string winner)
    {
        string suffix = _isBotMode ? "bot" : "pvp";

        if (winner == "x")
            Preferences.Set($"wins_x_{suffix}", Preferences.Get($"wins_x_{suffix}", 0) + 1);
        else if (winner == "o")
            Preferences.Set($"wins_o_{suffix}", Preferences.Get($"wins_o_{suffix}", 0) + 1);
        else
            Preferences.Set($"draws_{suffix}", Preferences.Get($"draws_{suffix}", 0) + 1);
    }
}
