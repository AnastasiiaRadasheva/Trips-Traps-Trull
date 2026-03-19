namespace TTT;

public class BotLogic
{
    private int _gamesPlayed;

    public int ManualDifficulty { get; set; } = -1;

    private int[] _playerMoveFrequency = new int[9];
    private int[][] _playerMoveByTurn = new int[9][];

    // История последних игр: список ходов игрока в каждой игре
    private List<int[]> _lostGamePatterns = new List<int[]>();
    private List<int> _currentGameMoves = new List<int>();
    private const int MaxPatterns = 10;

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

        // 1. Всегда: выиграть если можно
        int winMove = FindWinningMove(board, botSymbol);
        if (winMove != -1)
            return winMove;

        // 2. Всегда: заблокировать немедленную победу игрока
        int blockMove = FindWinningMove(board, playerSymbol);
        if (blockMove != -1)
        {
            double ignoreChance = GetIgnoreChance();
            if (_random.NextDouble() > ignoreChance)
                return blockMove;
        }

        // 3. Анализ паттернов прошлых проигрышей — приоритет на мастере
        int patternMove = GetAntiPatternMove(board, playerSymbol);
        if (patternMove != -1)
            return patternMove;

        // 4. Тактическое предупреждение по памяти
        int tacticalMove = GetTacticalCounterMove(board, playerSymbol);
        if (tacticalMove != -1)
            return tacticalMove;

        // 5. Центр (с вероятностью по уровню)
        if (string.IsNullOrEmpty(board[4]))
        {
            double centerChance = GetCenterChance();
            if (_random.NextDouble() < centerChance)
                return 4;
        }

        // 6. Ход по памяти частот
        int memoryMove = GetMoveFromMemory(board);
        if memoryMove != -1)
            return memoryMove;

        return GetRandomMove(board);
    }

    /// <summary>
    /// Анализирует паттерны игр где бот проиграл.
    /// Если текущая игра похожа на проигранную — ищет другой ход.
    /// </summary>
    private int GetAntiPatternMove(string[] board, string playerSymbol)
    {
        if (_lostGamePatterns.Count == 0)
            return -1;

        // Смотрим только последние 3+ игры (или сколько есть)
        int lookback = Math.Min(_lostGamePatterns.Count, Math.Max(3, _lostGamePatterns.Count));
        int playerMovesNow = _currentGameMoves.Count;

        if (playerMovesNow == 0)
            return -1;

        // Находим паттерны похожие на текущую игру
        var matchingPatterns = new List<int[]>();

        foreach (var pattern in _lostGamePatterns.TakeLast(lookback))
        {
            if (pattern.Length <= playerMovesNow)
                continue;

            // Проверяем совпадение первых N ходов
            bool matches = true;
            for (int i = 0; i < playerMovesNow; i++)
            {
                if (i >= pattern.Length || pattern[i] != _currentGameMoves[i])
                {
                    matches = false;
                    break;
                }
            }

            if (matches)
                matchingPatterns.Add(pattern);
        }

        if (matchingPatterns.Count == 0)
            return -1;

        // Собираем "опасные" следующие ходы из паттернов
        var dangerousCells = new Dictionary<int, int>(); // клетка -> сколько раз встречается
        foreach (var pattern in matchingPatterns)
        {
            if (playerMovesNow < pattern.Length)
            {
                int nextCell = pattern[playerMovesNow];
                if (string.IsNullOrEmpty(board[nextCell]))
                {
                    dangerousCells.TryGetValue(nextCell, out int cnt);
                    dangerousCells[nextCell] = cnt + 1;
                }
            }
        }

        if (dangerousCells.Count == 0)
            return -1;

        // Самая опасная клетка — занимаем её
        int bestCell = dangerousCells.OrderByDescending(x => x.Value).First().Key;

        // На высоком уровне — всегда блокируем, на низком — иногда пропускаем
        double useChance = GetPatternUseChance();
        if (_random.NextDouble() < useChance)
            return bestCell;

        return -1;
    }

    private double GetPatternUseChance()
    {
        int level = ManualDifficulty >= 0 ? ManualDifficulty * 3 : _gamesPlayed;
        if (level <= 3) return 0.0;   // Новичок — не использует
        if (level <= 6) return 0.4;   // Средний — иногда
        if (level <= 10) return 0.75; // Опытный — часто
        return 1.0;                   // Мастер — всегда
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
        _currentGameMoves.Add(index);

        if (turnNumber >= 0 && turnNumber < 9)
            _playerMoveByTurn[turnNumber][index]++;

        SaveMemory();
    }

    /// <summary>
    /// Вызывать в конце игры с результатом: true = бот проиграл
    /// </summary>
    public void RecordGameFinished(bool botLost = false)
    {
        _gamesPlayed++;
        Preferences.Set("bot_games_played", _gamesPlayed);

        if (botLost && _currentGameMoves.Count >= 2)
        {
            _lostGamePatterns.Add(_currentGameMoves.ToArray());

            // Храним только последние MaxPatterns
            if (_lostGamePatterns.Count > MaxPatterns)
                _lostGamePatterns.RemoveAt(0);

            SavePatterns();
        }

        _currentGameMoves.Clear();
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

    private void SavePatterns()
    {
        Preferences.Set("bot_pattern_count", _lostGamePatterns.Count);
        for (int p = 0; p < _lostGamePatterns.Count; p++)
        {
            var pattern = _lostGamePatterns[p];
            Preferences.Set($"bot_pattern_{p}_len", pattern.Length);
            for (int m = 0; m < pattern.Length; m++)
                Preferences.Set($"bot_pattern_{p}_{m}", pattern[m]);
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

        // Загружаем паттерны проигрышей
        _lostGamePatterns.Clear();
        int patternCount = Preferences.Get("bot_pattern_count", 0);
        for (int p = 0; p < patternCount; p++)
        {
            int len = Preferences.Get($"bot_pattern_{p}_len", 0);
            var pattern = new int[len];
            for (int m = 0; m < len; m++)
                pattern[m] = Preferences.Get($"bot_pattern_{p}_{m}", 0);
            _lostGamePatterns.Add(pattern);
        }
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

        int patternCount = Preferences.Get("bot_pattern_count", 0);
        for (int p = 0; p < patternCount; p++)
        {
            int len = Preferences.Get($"bot_pattern_{p}_len", 0);
            for (int m = 0; m < len; m++)
                Preferences.Remove($"bot_pattern_{p}_{m}");
            Preferences.Remove($"bot_pattern_{p}_len");
        }
        Preferences.Remove("bot_pattern_count");

        _lostGamePatterns.Clear();
        _currentGameMoves.Clear();
        _gamesPlayed = 0;
        Preferences.Remove("bot_games_played");
    }

    public int[] GetMemoryStats() => _playerMoveFrequency;
}