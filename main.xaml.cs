namespace TTT;

public partial class main : ContentPage
{
    private readonly GameLogic _game = new GameLogic();
    private readonly BotLogic _bot = new BotLogic();
    private readonly Button[] _cells = new Button[9];
    private Label _lblCurrentPlayer;

    // Флаг — играем против бота или против человека
    private bool _isBotMode = false;

    // Игрок всегда X, бот всегда O
    private const string PlayerSymbol = "X";
    private const string BotSymbol = "O";

    public main()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");
        Title = "Trips-Traps-Trull";

        // --- МАЛЕНЬКИЕ КНОПКИ ВВЕРХУ ---
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

        var topButtons = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.End,
            Spacing = 8
        };
        topButtons.Children.Add(btnStats);
        topButtons.Children.Add(btnRules);

        // --- ЛЕЙБЛ "ЧЕЙ ХОД" ---
        _lblCurrentPlayer = new Label
        {
            Text = "Käib: X",
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        // --- СТРОИМ ИГРОВУЮ СЕТКУ 3x3 ---
        var gameGrid = BuildGrid();

        // --- КНОПКИ ВНИЗУ ---
        var btnNewGame = new Button
        {
            Text = "🔄  Uus mäng",
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
            Text = "🎲  Kes alustab?",
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

        // --- КНОПКА ПЕРЕКЛЮЧЕНИЯ РЕЖИМА ---
        var btnToggleBot = new Button
        {
            Text = "🤖  vs Bot",
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

        Content = new VerticalStackLayout
        {
            Padding = new Thickness(20),
            Spacing = 16,
            VerticalOptions = LayoutOptions.Center,
            Children = { topButtons, _lblCurrentPlayer, gameGrid, bottomButtons, bottomButtons2 }
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

        // В режиме бота — принимаем ход только от игрока (X)
        if (_isBotMode && _game.CurrentPlayer != PlayerSymbol)
            return;

        if (!_game.MakeMove(index))
            return;

        // Показываем символ на кнопке
        SetButtonSymbol(btn, _game.CurrentPlayer);

        // Если режим бота — записываем ход игрока в память бота
        if (_isBotMode)
            _bot.RecordPlayerMove(index);

        // Проверяем результат
        string? result = _game.CheckResult();
        if (result != null)
        {
            await HandleResult(result);
            return;
        }

        _game.SwitchPlayer();
        _lblCurrentPlayer.Text = $"Käib: {_game.CurrentPlayer}";

        // Если режим бота и сейчас ход бота — делаем ход бота
        if (_isBotMode && _game.CurrentPlayer == BotSymbol)
            await MakeBotMove();
    }

    private async Task MakeBotMove()
    {
        // Небольшая задержка чтобы ход бота не был мгновенным
        await Task.Delay(400);

        // Бот выбирает лучший ход
        int botIndex = _bot.GetBestMove(_game.Board, BotSymbol);

        if (botIndex == -1 || !_game.MakeMove(botIndex))
            return;

        // Показываем символ бота на кнопке
        SetButtonSymbol(_cells[botIndex], BotSymbol);

        // Проверяем результат после хода бота
        string? result = _game.CheckResult();
        if (result != null)
        {
            await HandleResult(result);
            return;
        }

        _game.SwitchPlayer();
        _lblCurrentPlayer.Text = $"Käib: {_game.CurrentPlayer}";
    }

    // Устанавливаем текст и цвет символа на кнопке
    private void SetButtonSymbol(Button btn, string symbol)
    {
        btn.Text = symbol;
        btn.TextColor = symbol == "X"
            ? Color.FromArgb("#e94560")  // X — красный
            : Color.FromArgb("#0f9b58"); // O — зелёный
    }

    private async Task HandleResult(string result)
    {
        string message;

        if (result == "Draw")
        {
            message = "Viik! 🤝";
            SaveStats("draw");
        }
        else if (_isBotMode)
        {
            // В режиме бота показываем кто победил — игрок или бот
            message = result == PlayerSymbol
                ? "Sa võitsid boti! 🎉"
                : "Bot võitis! 🤖";
            SaveStats(result == "X" ? "x" : "o");
        }
        else
        {
            message = $"{result} võitis! 🎉";
            SaveStats(result == "X" ? "x" : "o");
        }

        if (_isBotMode)
            _bot.RecordGameFinished();

        bool playAgain = await DisplayAlert("Mäng läbi!", message, "Uus mäng", "Välju");

        if (playAgain)
            ResetBoard();
        else
            await Shell.Current.GoToAsync("..");
    }

    private void OnNewGameClicked(object? sender, EventArgs e)
    {
        ResetBoard();
    }

    private async void OnRandomStartClicked(object? sender, EventArgs e)
    {
        string starter = _game.RandomStartPlayer();
        ResetBoard(starter);
        await DisplayAlert("🎲 Loosimine!", $"Alustab mängija {starter}!", "OK");
    }

    private async void OnToggleBotClicked(object? sender, EventArgs e)
    {
        // Переключаем режим
        _isBotMode = !_isBotMode;

        var btn = (Button)sender!;
        if (_isBotMode)
        {
            // Включаем бота — подсвечиваем кнопку
            btn.BackgroundColor = Color.FromArgb("#e94560");
            btn.Text = "🤖  vs Bot ON";
            await DisplayAlert("🤖 Bot", "Bot on sisse lülitatud! Sina mängid X-ga.", "OK");
        }
        else
        {
            // Выключаем бота
            btn.BackgroundColor = Color.FromArgb("#16213e");
            btn.Text = "🤖  vs Bot";
        }

        ResetBoard();
    }

    private async void OnStatsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("StatsPage");
    }

    private async void OnRulesClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("RulesPage");
    }

    private void ResetBoard(string startingPlayer = "X")
    {
        _game.Reset(startingPlayer);

        foreach (var cell in _cells)
        {
            cell.Text = string.Empty;
            cell.BackgroundColor = Color.FromArgb("#16213e");
        }

        _lblCurrentPlayer.Text = $"Käib: {startingPlayer}";
    }

    private void SaveStats(string winner)
    {
        if (winner == "x")
            Preferences.Set("wins_x", Preferences.Get("wins_x", 0) + 1);
        else if (winner == "o")
            Preferences.Set("wins_o", Preferences.Get("wins_o", 0) + 1);
        else
            Preferences.Set("draws", Preferences.Get("draws", 0) + 1);
    }
}