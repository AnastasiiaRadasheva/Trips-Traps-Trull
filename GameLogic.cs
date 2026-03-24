namespace TTT;

public class GameLogic
{
    public string[] Board { get; private set; } = new string[9];
    public string CurrentPlayer { get; private set; } = "X";
    public bool GameOver { get; private set; } = false;

    private static readonly int[][] WinCombinations =
    {
        new[] { 0, 1, 2 },
        new[] { 3, 4, 5 },
        new[] { 6, 7, 8 },
        new[] { 0, 3, 6 },
        new[] { 1, 4, 7 },
        new[] { 2, 5, 8 },
        new[] { 0, 4, 8 },
        new[] { 2, 4, 6 }
    };

    public bool MakeMove(int index)
    {
        if (GameOver || !string.IsNullOrEmpty(Board[index]))
            return false;

        Board[index] = CurrentPlayer;
        return true;
    }

    public string? CheckResult()
    {
        foreach (var combo in WinCombinations)
        {
            if (!string.IsNullOrEmpty(Board[combo[0]]) &&
                Board[combo[0]] == Board[combo[1]] &&
                Board[combo[1]] == Board[combo[2]])
            {
                GameOver = true;
                return Board[combo[0]];
            }
        }

        if (Array.TrueForAll(Board, cell => !string.IsNullOrEmpty(cell)))
        {
            GameOver = true;
            return "Draw";
        }

        return null;
    }

    public void SwitchPlayer()
    {
        CurrentPlayer = CurrentPlayer == "X" ? "O" : "X";
    }

    public void Reset(string startingPlayer = "X")
    {
        _turnNumber = 0;
        Board = new string[9];
        CurrentPlayer = startingPlayer;
        GameOver = false;
    }

    public string RandomStartPlayer()
    {
        var random = new Random();
        CurrentPlayer = random.Next(2) == 0 ? "X" : "O";
        return CurrentPlayer;
    }
}
