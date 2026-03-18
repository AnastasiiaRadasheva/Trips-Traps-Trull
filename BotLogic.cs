namespace TTT;

public class BotLogic
{
    private int _gamesPlayed;

    private int[] _playerMoveFrequency = new int[9];

    private readonly Random _random = new Random();

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

    public BotLogic()
    {
        LoadMemory();
    }

    public int GetBestMove(string[] board, string botSymbol)
    {
        string playerSymbol = botSymbol == "X" ? "O" : "X";

        int winMove = FindWinningMove(board, botSymbol);
        if (winMove != -1)
            return winMove;

        int blockMove = FindWinningMove(board, playerSymbol);
        if (blockMove != -1)
        {
            double ignoreChance = GetIgnoreChance();

            if (_random.NextDouble() > ignoreChance)
                return blockMove; 
        }

        if (string.IsNullOrEmpty(board[4]))
        {
            double centerChance = GetCenterChance();
            if (_random.NextDouble() < centerChance)
                return 4;
        }

        int memoryMove = GetMoveFromMemory(board);
        if (memoryMove != -1)
            return memoryMove;

        return GetRandomMove(board);
    }

    private double GetIgnoreChance()
    {
        if (_gamesPlayed <= 2) return 0.80;
        if (_gamesPlayed <= 6) return 0.40;
        if (_gamesPlayed <= 10) return 0.20;
        return 0.05;
    }

    private double GetCenterChance()
    {
        if (_gamesPlayed <= 5) return 0.30;
        if (_gamesPlayed <= 10) return 0.60;
        return 1.0;
    }

    public void RecordPlayerMove(int index)
    {
        _playerMoveFrequency[index]++;
        SaveMemory();
    }

    public void RecordGameFinished()
    {
        _gamesPlayed++;
        Preferences.Set("bot_games_played", _gamesPlayed);
    }

    public int GamesPlayed => _gamesPlayed;

    private int FindWinningMove(string[] board, string symbol)
    {
        foreach (var combo in WinCombinations)
        {
            int symbolCount = 0;
            int emptyIndex = -1;

            foreach (int i in combo)
            {
                if (board[i] == symbol)
                    symbolCount++;
                else if (string.IsNullOrEmpty(board[i]))
                    emptyIndex = i;
            }

            if (symbolCount == 2 && emptyIndex != -1)
                return emptyIndex;
        }

        return -1;
    }

    private int GetMoveFromMemory(string[] board)
    {
        var freeCells = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
                freeCells.Add(i);
        }

        if (freeCells.Count == 0)
            return -1;

        int bestCell = -1;
        int highestFrequency = 0;

        foreach (int cell in freeCells)
        {
            if (_playerMoveFrequency[cell] > highestFrequency)
            {
                highestFrequency = _playerMoveFrequency[cell];
                bestCell = cell;
            }
        }
        if (highestFrequency > 1)
            return bestCell;

        return -1;
    }

    private int GetRandomMove(string[] board)
    {
        var freeCells = new List<int>();
        for (int i = 0; i < 9; i++)
        {
            if (string.IsNullOrEmpty(board[i]))
                freeCells.Add(i);
        }

        if (freeCells.Count == 0)
            return -1;

        return freeCells[_random.Next(freeCells.Count)];
    }

    private void SaveMemory()
    {
        for (int i = 0; i < 9; i++)
            Preferences.Set($"bot_memory_{i}", _playerMoveFrequency[i]);
    }

    private void LoadMemory()
    {
        for (int i = 0; i < 9; i++)
            _playerMoveFrequency[i] = Preferences.Get($"bot_memory_{i}", 0);

        _gamesPlayed = Preferences.Get("bot_games_played", 0);
    }

    public void ResetMemory()
    {
        for (int i = 0; i < 9; i++)
        {
            _playerMoveFrequency[i] = 0;
            Preferences.Remove($"bot_memory_{i}");
        }
        _gamesPlayed = 0;
        Preferences.Remove("bot_games_played");
    }

    public int[] GetMemoryStats() => _playerMoveFrequency;
}