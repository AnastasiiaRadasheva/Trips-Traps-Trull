namespace TTT;

public class BotLogic
{
    private int _gamesPlayed;

    public int ManualDifficulty { get; set; } = -1;

    private int[] _playerMoveFrequency = new int[9];

    private int[][] _playerMoveByTurn = new int[9][];

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
        for (int i = 0; i < 9; i++)
            _playerMoveByTurn[i] = new int[9];

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

        int tacticalMove = GetTacticalCounterMove(board, playerSymbol);
        if (tacticalMove != -1)
            return tacticalMove;

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

    private int GetTacticalCounterMove(string[] board, string playerSymbol)
    {
        double memoryWeight = GetMemoryWeight();
        if (memoryWeight <= 0)
            return -1;

        int playerTurn = CountSymbols(board, playerSymbol);
        int nextTurn = playerTurn; 

        if (nextTurn >= 9)
            return -1;

        var freeCells = new List<int>();
        for (int i = 0; i < 9; i++)
            if (string.IsNullOrEmpty(board[i]))
                freeCells.Add(i);

        if (freeCells.Count == 0)
            return -1;

        int bestCell = -1;
        double bestScore = -1;

        foreach (int cell in freeCells)
        {
            double score = 0;

            score += _playerMoveFrequency[cell] * 1.0;

            score += _playerMoveByTurn[nextTurn][cell] * 2.0;

            score += CountForkPotential(board, playerSymbol, cell) * 3.0;

            score *= memoryWeight;

            if (score > bestScore)
            {
                bestScore = score;
                bestCell = cell;
            }
        }

        double threshold = GetMemoryThreshold();
        if (bestScore >= threshold)
            return bestCell;

        return -1;
    }

    private int CountForkPotential(string[] board, string playerSymbol, int cell)
    {
        string botSymbol = playerSymbol == "X" ? "O" : "X";
        int count = 0;

        foreach (var combo in WinCombinations)
        {
            if (!combo.Contains(cell)) continue;

            bool botBlocks = false;
            int playerInLine = 0;

            foreach (int i in combo)
            {
                if (board[i] == botSymbol) { botBlocks = true; break; }
                if (board[i] == playerSymbol) playerInLine++;
            }

            if (!botBlocks && playerInLine >= 1)
                count++;
        }

        return count;
    }

    private int CountSymbols(string[] board, string symbol)
    {
        int count = 0;
        foreach (var cell in board)
            if (cell == symbol) count++;
        return count;
    }

    // Насколько бот доверяет памяти (0 = игнорирует, 1 = полностью доверяет)
    private double GetMemoryWeight()
    {
        int level = ManualDifficulty >= 0 ? ManualDifficulty * 3 : _gamesPlayed;
        if (level <= 3) return 0.0;
        if (level <= 6) return 0.3;
        if (level <= 10) return 0.7;
        return 1.0;
    }

    private double GetMemoryThreshold()
    {
        int level = ManualDifficulty >= 0 ? ManualDifficulty * 3 : _gamesPlayed;
        if (level <= 6) return 4.0;
        if (level <= 10) return 2.0;
        return 1.0; 
    }

    private double GetIgnoreChance()
    {
        int level = ManualDifficulty >= 0 ? ManualDifficulty * 3 : _gamesPlayed;
        if (level <= 2) return 0.80;
        if (level <= 6) return 0.40;
        if (level <= 10) return 0.20;
        return 0.05;
    }

    private double GetCenterChance()
    {
        int level = ManualDifficulty >= 0 ? ManualDifficulty * 3 : _gamesPlayed;
        if (level <= 5) return 0.30;
        if (level <= 10) return 0.60;
        return 1.0;
    }

    public void RecordPlayerMove(int index, int turnNumber = -1)
    {
        _playerMoveFrequency[index]++;

        if (turnNumber >= 0 && turnNumber < 9)
            _playerMoveByTurn[turnNumber][index]++;

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
                if (board[i] == symbol) symbolCount++;
                else if (string.IsNullOrEmpty(board[i])) emptyIndex = i;
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
            if (string.IsNullOrEmpty(board[i]))
                freeCells.Add(i);

        if (freeCells.Count == 0) return -1;

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
            if (string.IsNullOrEmpty(board[i]))
                freeCells.Add(i);

        if (freeCells.Count == 0) return -1;
        return freeCells[_random.Next(freeCells.Count)];
    }

    private void SaveMemory()
    {
        for (int i = 0; i < 9; i++)
        {
            Preferences.Set($"bot_memory_{i}", _playerMoveFrequency[i]);
            for (int t = 0; t < 9; t++)
                Preferences.Set($"bot_memory_turn_{t}_{i}", _playerMoveByTurn[t][i]);
        }
    }

    public void ReloadMemory() => LoadMemory();

    private void LoadMemory()
    {
        for (int i = 0; i < 9; i++)
        {
            _playerMoveFrequency[i] = Preferences.Get($"bot_memory_{i}", 0);
            for (int t = 0; t < 9; t++)
                _playerMoveByTurn[t][i] = Preferences.Get($"bot_memory_turn_{t}_{i}", 0);
        }
        _gamesPlayed = Preferences.Get("bot_games_played", 0);
    }

    public void ResetMemory()
    {
        for (int i = 0; i < 9; i++)
        {
            _playerMoveFrequency[i] = 0;
            Preferences.Remove($"bot_memory_{i}");
            for (int t = 0; t < 9; t++)
            {
                _playerMoveByTurn[t][i] = 0;
                Preferences.Remove($"bot_memory_turn_{t}_{i}");
            }
        }
        _gamesPlayed = 0;
        Preferences.Remove("bot_games_played");
    }

    public int[] GetMemoryStats() => _playerMoveFrequency;
}