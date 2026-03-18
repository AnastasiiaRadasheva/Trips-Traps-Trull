namespace TTT;

public partial class main : ContentPage
{
    private readonly GameLogic _game = new GameLogic();
    private readonly Button[] _cells = new Button[9];
    private Label _lblCurrentPlayer;

    public main()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");
        Title = "Trips-Traps-Trull";

        _lblCurrentPlayer = new Label
        {
            Text = "Käib: X",
            FontSize = 22,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var gameGrid = BuildGrid();

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

        var bottomButtons = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 12,
            Margin = new Thickness(0, 10, 0, 0),
            Children = { btnNewGame, btnRandomStart }
        };

        Content = new VerticalStackLayout
        {
            Padding = new Thickness(20),
            Spacing = 16,
            VerticalOptions = LayoutOptions.Center,
            Children = { _lblCurrentPlayer, gameGrid, bottomButtons }
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

        if (!_game.MakeMove(index))
            return;

        btn.Text = _game.CurrentPlayer;
        btn.TextColor = _game.CurrentPlayer == "X"
            ? Color.FromArgb("#e94560")
            : Color.FromArgb("#0f9b58");

        string result = _game.CheckResult();

        if (result != null)
        {
            await HandleResult(result);
            return;
        }

        _game.SwitchPlayer();
        _lblCurrentPlayer.Text = $"Käib: {_game.CurrentPlayer}";
    }

    private async Task HandleResult(string result)
    {
        string message;

        if (result == "Draw")
        {
            message = "Viik! 🤝";
            SaveStats("draw");
        }
        else
        {
            message = $"{result} võitis! 🎉";
            SaveStats(result == "X" ? "x" : "o");
        }

        bool playAgain = await DisplayAlertAsync("Mäng läbi!", message, "Uus mäng", "Välju");

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
        await DisplayAlertAsync("🎲 Loosimine!", $"Alustab mängija {starter}!", "OK");
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