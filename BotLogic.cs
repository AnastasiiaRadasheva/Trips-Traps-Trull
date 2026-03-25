namespace TTT;

public class BotLogic
{
    private int _gamesPlayed;

    public int ManualDifficulty { get; set; } = -1;

    private int[] _playerMoveFrequency = new int[9];
    private int[][] _playerMoveByTurn = new int[9][];

    private readonly int[] _currentGameMoves = new int[9];
    private int _currentGameMoveCount = 0;

    private const int MaxMemoryGames = 20;

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

    // ──────────────────────────────────────────────
    // Основная логика хода
    // ──────────────────────────────────────────────

    public int GetBestMove(string[] board, string botSymbol)
    {
        string playerSymbol = botSymbol == "X" ? "O" : "X";

        int winMove = FindWinningMove(board, botSymbol);
        if (winMove != -1)
            return winMove;

        int blockMove = FindWinningMove(board, playerSymbol);
        if (blockMove != -1)
        {
            if (_random.NextDouble() > GetIgnoreChance())
                return blockMove;
        }

        int tacticalMove = GetTacticalCounterMove(board, playerSymbol);
        if (tacticalMove != -1)
            return tacticalMove;

        if (string.IsNullOrEmpty(board[4]) && _random.NextDouble() < GetCenterChance())
            return 4;

        if (GetMemoryWeight() > 0)
        {
            int memoryMove = GetMoveFromMemory(board);
            if (memoryMove != -1)
                return memoryMove;
        }

        return GetRandomMove(board);
    }

    private int GetTacticalCounterMove(string[] board, string playerSymbol)
    {
        double memoryWeight = GetMemoryWeight();
        if (memoryWeight <= 0)
            return -1;

        int playerTurn = CountSymbols(board, playerSymbol);
        if (playerTurn >= 9)
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
            score += _playerMoveByTurn[playerTurn][cell] * 2.0;
            score += CountForkPotential(board, playerSymbol, cell) * 3.0;
            score *= memoryWeight;

            if (score > bestScore)
            {
                bestScore = score;
                bestCell = cell;
            }
        }

        return bestScore >= GetMemoryThreshold() ? bestCell : -1;
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

    // ──────────────────────────────────────────────
    // Уровни сложности
    // ──────────────────────────────────────────────

    private int EffectiveLevel => ManualDifficulty >= 0 ? ManualDifficulty * 3 : _gamesPlayed;

    private double GetMemoryWeight()
    {
        int level = EffectiveLevel;
        if (level <= 3) return 0.0;
        if (level <= 6) return 0.3;
        if (level <= 10) return 0.7;
        return 1.0;
    }

    private double GetMemoryThreshold()
    {
        int level = EffectiveLevel;
        if (level <= 6) return 4.0;
        if (level <= 10) return 2.0;
        return 1.0;
    }

    private double GetIgnoreChance()
    {
        int level = EffectiveLevel;
        if (level <= 2) return 0.80;
        if (level <= 6) return 0.40;
        if (level <= 10) return 0.20;
        return 0.05;
    }

    private double GetCenterChance()
    {
        int level = EffectiveLevel;
        if (level <= 5) return 0.30;
        if (level <= 10) return 0.60;
        return 1.0;
    }

    // ──────────────────────────────────────────────
    // Запись ходов и результата игры
    // ──────────────────────────────────────────────

    public void RecordPlayerMove(int index, int turnNumber = -1)
    {
        // Просто буферизуем ход текущей игры — в Preferences не пишем
        if (_currentGameMoveCount < 9)
            _currentGameMoves[_currentGameMoveCount++] = index;
    }

    public void RecordGameFinished()
    {
        _gamesPlayed++;
        Preferences.Set("bot_games_played", _gamesPlayed);

        ApplyCurrentGameToMemory();
        _currentGameMoveCount = 0;
    }

    private void ApplyCurrentGameToMemory()
    {
        // Загружаем историю последних игр
        string history = Preferences.Get("bot_move_history", "");
        var games = history.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();

        // Добавляем текущую игру как строку "0,4,2,6,..."
        string newGame = string.Join(",", _currentGameMoves.Take(_currentGameMoveCount));
        if (!string.IsNullOrEmpty(newGame))
            games.Add(newGame);

        // Оставляем только последние MaxMemoryGames игр
        if (games.Count > MaxMemoryGames)
            games = games.Skip(games.Count - MaxMemoryGames).ToList();

        Preferences.Set("bot_move_history", string.Join("|", games));

        // Пересчитываем частоты с нуля по актуальной истории
        RecalculateFrequencies(games);
    }

    private void RecalculateFrequencies(List<string> games)
    {
        _playerMoveFrequency = new int[9];
        _playerMoveByTurn = new int[9][];
        for (int i = 0; i < 9; i++)
            _playerMoveByTurn[i] = new int[9];

        foreach (var game in games)
        {
            var moves = game.Split(',', StringSplitOptions.RemoveEmptyEntries);
            for (int t = 0; t < moves.Length; t++)
            {
                if (int.TryParse(moves[t], out int cell) && cell >= 0 && cell < 9)
                {
                    _playerMoveFrequency[cell]++;
                    if (t < 9) _playerMoveByTurn[t][cell]++;
                }
            }
        }
    }

    // ──────────────────────────────────────────────
    // Вспомогательные методы выбора хода
    // ──────────────────────────────────────────────

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

        return highestFrequency > 1 ? bestCell : -1;
    }

    private int GetRandomMove(string[] board)
    {
        var freeCells = new List<int>();
        for (int i = 0; i < 9; i++)
            if (string.IsNullOrEmpty(board[i]))
                freeCells.Add(i);

        return freeCells.Count == 0 ? -1 : freeCells[_random.Next(freeCells.Count)];
    }

    // ──────────────────────────────────────────────
    // Сохранение / загрузка
    // ──────────────────────────────────────────────

    public void ReloadMemory() => LoadMemory();

    private void LoadMemory()
    {
        _playerMoveFrequency = new int[9];
        _playerMoveByTurn = new int[9][];
        for (int i = 0; i < 9; i++)
            _playerMoveByTurn[i] = new int[9];

        string history = Preferences.Get("bot_move_history", "");
        var games = history.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
        RecalculateFrequencies(games);

        _gamesPlayed = Preferences.Get("bot_games_played", 0);
    }

    public void ResetMemory()
    {
        _playerMoveFrequency = new int[9];
        _playerMoveByTurn = new int[9][];
        for (int i = 0; i < 9; i++)
            _playerMoveByTurn[i] = new int[9];

        _currentGameMoveCount = 0;
        _gamesPlayed = 0;

        Preferences.Remove("bot_move_history");
        Preferences.Remove("bot_games_played");

        // Чистим старые ключи на случай если они остались от предыдущей версии
        for (int i = 0; i < 9; i++)
        {
            Preferences.Remove($"bot_memory_{i}");
            for (int t = 0; t < 9; t++)
                Preferences.Remove($"bot_memory_turn_{t}_{i}");
        }
    }

    public int GamesPlayed => _gamesPlayed;
    public int[] GetMemoryStats() => _playerMoveFrequency;
}