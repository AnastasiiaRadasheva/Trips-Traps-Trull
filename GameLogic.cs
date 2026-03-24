namespace TTT;

/// <summary>
/// Supports both 3×3 (classic) and 4×4 (tournament) boards.
/// Win condition for 4×4: 4 in a row.
/// </summary>
public class GameLogic
{
    // ──────────────────────────────────────────────
    // State
    // ──────────────────────────────────────────────
    public int Size { get; private set; } = 3;
    public string[] Board { get; private set; } = new string[9];
    public string CurrentPlayer { get; private set; } = "X";
    public bool GameOver { get; private set; } = false;

    // Pre-computed win combinations for current board size
    private int[][] _winCombinations = Array.Empty<int[]>();

    // ──────────────────────────────────────────────
    // Constructor / init
    // ──────────────────────────────────────────────
    public GameLogic(int size = 3) => InitBoard(size);

    private void InitBoard(int size)
    {
        Size = size;
        Board = new string[size * size];
        _winCombinations = BuildWinCombinations(size);
        GameOver = false;
    }

    // ──────────────────────────────────────────────
    // Win combination builder (works for 3 and 4)
    // ──────────────────────────────────────────────
    private static int[][] BuildWinCombinations(int n)
    {
        var combos = new List<int[]>();

        // Rows
        for (int r = 0; r < n; r++)
        {
            var row = new int[n];
            for (int c = 0; c < n; c++) row[c] = r * n + c;
            combos.Add(row);
        }

        // Columns
        for (int c = 0; c < n; c++)
        {
            var col = new int[n];
            for (int r = 0; r < n; r++) col[r] = r * n + c;
            combos.Add(col);
        }

        // Main diagonal
        var diag1 = new int[n];
        for (int i = 0; i < n; i++) diag1[i] = i * n + i;
        combos.Add(diag1);

        // Anti-diagonal
        var diag2 = new int[n];
        for (int i = 0; i < n; i++) diag2[i] = i * n + (n - 1 - i);
        combos.Add(diag2);

        return combos.ToArray();
    }

    // ──────────────────────────────────────────────
    // Game actions
    // ──────────────────────────────────────────────
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

    public void SwitchPlayer() =>
        CurrentPlayer = CurrentPlayer == "X" ? "O" : "X";

    public void SwitchPlayerTo(string next) =>
        CurrentPlayer = next;

    public void Reset(string startingPlayer = "X", int? size = null)
    {
        InitBoard(size ?? Size);
        CurrentPlayer = startingPlayer;
    }

    public string RandomStartPlayer()
    {
        CurrentPlayer = new Random().Next(2) == 0 ? "X" : "O";
        return CurrentPlayer;
    }

    // Useful for multi-symbol tournaments (X / O / Z)
    public void SetCurrentPlayer(string player) => CurrentPlayer = player;
}