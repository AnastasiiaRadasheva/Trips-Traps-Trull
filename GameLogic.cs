namespace TTT;

public class GameLogic
{
    public int Size { get; private set; } = 3;
    public string[] Board { get; private set; } = new string[9];
    public string CurrentPlayer { get; private set; } = "X";
    public bool GameOver { get; private set; } = false;

    public List<string> Players { get; private set; } = new() { "X", "O" };
    private int _turnIndex = 0;

    private int[][] _winCombinations = Array.Empty<int[]>();

    public GameLogic(int size = 3) => InitBoard(size);

    private void InitBoard(int size)
    {
        Size = size;
        Board = new string[size * size];
        _winCombinations = BuildWinCombinations(size);
        GameOver = false;
    }

    private static int[][] BuildWinCombinations(int n)
    {
        var combos = new List<int[]>();

        for (int r = 0; r < n; r++)
        {
            var row = new int[n];
            for (int c = 0; c < n; c++) row[c] = r * n + c;
            combos.Add(row);
        }

        for (int c = 0; c < n; c++)
        {
            var col = new int[n];
            for (int r = 0; r < n; r++) col[r] = r * n + c;
            combos.Add(col);
        }

        var diag1 = new int[n];
        for (int i = 0; i < n; i++) diag1[i] = i * n + i;
        combos.Add(diag1);

        var diag2 = new int[n];
        for (int i = 0; i < n; i++) diag2[i] = i * n + (n - 1 - i);
        combos.Add(diag2);

        return combos.ToArray();
    }

    public bool MakeMove(int index)
    {
        if (GameOver || index < 0 || index >= Board.Length || !string.IsNullOrEmpty(Board[index]))
            return false;

        Board[index] = CurrentPlayer;
        return true;
    }

    public string? CheckResult()
    {
        foreach (var combo in _winCombinations)
        {
            string first = Board[combo[0]];
            if (string.IsNullOrEmpty(first)) continue;

            if (combo.All(i => Board[i] == first))
            {
                GameOver = true;
                return first;
            }
        }

        if (Array.TrueForAll(Board, cell => !string.IsNullOrEmpty(cell)))
        {
            GameOver = true;
            return "Draw";
        }

        return null;
    }

    // 2-player switch (used in main.cs)
    public void SwitchPlayer() =>
        CurrentPlayer = CurrentPlayer == "X" ? "O" : "X";

    // Multi-player switch (used in TournamentPage)
    public void SwitchPlayer_1()
    {
        _turnIndex++;
        CurrentPlayer = Players[_turnIndex % Players.Count];
    }

    public void SwitchPlayerTo(string next) =>
        CurrentPlayer = next;

    // 2-player reset (used in main.cs)
    public void Reset(string startingPlayer = "X", int? size = null)
    {
        InitBoard(size ?? Size);

        Players = new List<string> { "X", "O" };
        _turnIndex = Players.IndexOf(startingPlayer);
        if (_turnIndex < 0) _turnIndex = 0;

        CurrentPlayer = Players[_turnIndex];
    }

    // Tournament reset (used in TournamentPage)
    public void Reset(List<string> players, int size)
    {
        InitBoard(size);

        Players = new List<string>(players);
        _turnIndex = 0;
        CurrentPlayer = Players[0];
    }

    public string RandomStartPlayer()
    {
        var rnd = new Random();
        _turnIndex = rnd.Next(Players.Count);
        CurrentPlayer = Players[_turnIndex];
        return CurrentPlayer;
    }

    public void SetCurrentPlayer(string player)
    {
        CurrentPlayer = player;
        _turnIndex = Players.IndexOf(player);
    }
}