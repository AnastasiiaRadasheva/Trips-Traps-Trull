namespace TTT;

public partial class TournamentPage : ContentPage
{
    private readonly TournamentManager _tournament = new();
    private readonly GameLogic _game = new(4);

    private Button[] _cells = Array.Empty<Button>();
    private Grid _gameGrid = null!;

    private Label _lblPhase = null!;
    private Label _lblCurrentPlayer = null!;
    private Label _lblBye = null!;

    private Entry _entryX = null!;
    private Entry _entryO = null!;
    private Entry _entryZ = null!;

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
    }

    // ───────── UI BUILD ─────────
    private void BuildUI()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");
        Title = "3-mängija turniir";

        _lblPhase = CreateLabel("", 20, true, "#e94560");
        _lblCurrentPlayer = CreateLabel("", 18);
        _lblBye = CreateLabel("", 14, false, "#aaaaaa");

        _entryX = CreateEntry("Mängija X nimi");
        _entryO = CreateEntry("Mängija O nimi");
        _entryZ = CreateEntry("Mängija Z nimi");

        var btnStart = CreateButton("🏆 Alusta turniiri", "#e94560", OnStartClicked);
        var btnRestart = CreateButton("🔄 Uus turniir", "#16213e", OnRestartClicked, true);

        _gameGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Center
        };

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 12,
                Children =
                {
                    _lblPhase,
                    _lblCurrentPlayer,
                    _lblBye,
                    CreateForm(),
                    btnStart,
                    _gameGrid,
                    btnRestart
                }
            }
        };
    }

    private VerticalStackLayout CreateForm()
    {
        return new VerticalStackLayout
        {
            Spacing = 8,
            Children = { _entryX, _entryO, _entryZ }
        };
    }

    private static Entry CreateEntry(string placeholder) => new()
    {
        Placeholder = placeholder,
        BackgroundColor = Color.FromArgb("#16213e"),
        TextColor = Colors.White
    };

    private static Label CreateLabel(string text, int size, bool bold = false, string? color = null)
    {
        return new Label
        {
            Text = text,
            FontSize = size,
            FontAttributes = bold ? FontAttributes.Bold : FontAttributes.None,
            TextColor = color != null ? Color.FromArgb(color) : Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };
    }

    private static Button CreateButton(string text, string bg, EventHandler handler, bool bordered = false)
    {
        var btn = new Button
        {
            Text = text,
            BackgroundColor = Color.FromArgb(bg),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48,
            WidthRequest = 230
        };

        btn.Clicked += handler;

        if (bordered)
        {
            btn.BorderWidth = 1;
            btn.BorderColor = Color.FromArgb("#e94560");
        }

        return btn;
    }

    // ───────── GAME GRID ─────────
    private void RebuildGrid(int size)
    {
        _gameGrid.Children.Clear();
        _gameGrid.RowDefinitions.Clear();
        _gameGrid.ColumnDefinitions.Clear();

        for (int i = 0; i < size; i++)
        {
            _gameGrid.RowDefinitions.Add(new RowDefinition());
            _gameGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        _cells = new Button[size * size];

        for (int i = 0; i < _cells.Length; i++)
        {
            var btn = new Button
            {
                FontSize = size == 4 ? 28 : 36,
                BackgroundColor = Color.FromArgb("#16213e"),
                TextColor = Colors.White,
                CornerRadius = 8,
                CommandParameter = i
            };

            btn.Clicked += OnCellClicked;

            _cells[i] = btn;
            _gameGrid.Add(btn, i % size, i / size);
        }
    }

    // ───────── ACTIONS ─────────
    private async void OnStartClicked(object? sender, EventArgs e)
    {
        _playerNames["X"] = NormalizeName(_entryX.Text, "Mängija X");
        _playerNames["O"] = NormalizeName(_entryO.Text, "Mängija O");
        _playerNames["Z"] = NormalizeName(_entryZ.Text, "Mängija Z");

        _tournament.Start(new List<string> { "X", "O", "Z" });

        await StartPhase();
    }

    private async void OnRestartClicked(object? sender, EventArgs e)
    {
        _tournament.Reset();
        _gameGrid.Children.Clear();

        _lblPhase.Text = "TURNIIR";
        _lblCurrentPlayer.Text = "";
        _lblBye.Text = "";

        await Task.CompletedTask;
    }

    private string NormalizeName(string? input, string fallback)
        => string.IsNullOrWhiteSpace(input) ? fallback : input.Trim();

    // ───────── PHASE FLOW ─────────
    private async Task StartPhase()
    {
        var (pA, pB) = _tournament.GetCurrentMatchPlayers();

        _game.Reset(pA, _tournament.BoardSize);
        RebuildGrid(_tournament.BoardSize);

        UpdateStatus(pA);

        var phase = _tournament.CurrentPhase;

        if (phase == TournamentManager.Phase.Round1)
        {
            _lblBye.Text = $"Ootab: {NameOf(_tournament.ByePlayer)}";

            await DisplayAlert("Voor 1",
                $"{NameOf(pA)} vs {NameOf(pB)}",
                "OK");
        }
        else if (phase == TournamentManager.Phase.Round2)
        {
            _lblBye.Text = "";

            await DisplayAlert("Voor 2",
                $"{NameOf(pA)} vs {NameOf(pB)}",
                "OK");
        }
        else if (phase == TournamentManager.Phase.Final)
        {
            await DisplayAlert("Finaal",
                $"{NameOf(pA)} vs {NameOf(pB)}",
                "OK");
        }
    }

    // ───────── CLICK HANDLER ─────────
    private async void OnCellClicked(object? sender, EventArgs e)
    {
        if (_tournament.CurrentPhase is TournamentManager.Phase.NotStarted
            or TournamentManager.Phase.Finished)
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

        _game.SwitchPlayer();
        UpdateStatus(_game.CurrentPlayer);
    }

    // ───────── RESULT ─────────
    private async Task HandleResult(string result)
    {
        if (result == "Draw")
        {
            await DisplayAlert("Viik", "Mängitakse uuesti", "OK");
            await StartPhase();
            return;
        }

        _tournament.RegisterResult(result);

        if (_tournament.CurrentPhase == TournamentManager.Phase.Finished)
        {
            await ShowResults();
            return;
        }

        await DisplayAlert("Voor lõppenud", $"{NameOf(result)} võitis", "OK");
        await StartPhase();
    }

    private async Task ShowResults()
    {
        string text =
            $"🥇 {NameOf(_tournament.First)}\n" +
            $"🥈 {NameOf(_tournament.Second)}\n" +
            $"🥉 {NameOf(_tournament.Third)}";

        _lblPhase.Text = "LÕPP";

        await DisplayAlert("Tulemused", text, "OK");
    }

    // ───────── UI HELPERS ─────────
    private void UpdateStatus(string currentPlayer)
    {
        _lblPhase.Text = _tournament.PhaseLabel().ToUpper();
        _lblCurrentPlayer.Text = $"Käib: {NameOf(currentPlayer)} ({currentPlayer})";
    }

    private void SetCell(Button btn, string symbol)
    {
        btn.Text = symbol;
        btn.TextColor = SymbolColors.TryGetValue(symbol, out var c) ? c : Colors.White;
    }

    private string NameOf(string symbol)
        => _playerNames.TryGetValue(symbol, out var name) ? name : symbol;
}