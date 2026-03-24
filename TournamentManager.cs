namespace TTT;

/// <summary>
/// Manages a 3-player tournament:
///   Round 1  – 2 random players on 4×4 board
///   Round 2  – loser of R1 vs bye-player on 4×4 board
///   Final    – both winners on 3×3 board
///   Result   – 1st / 2nd / 3rd place determined
/// </summary>
public class TournamentManager
{
    // ──────────────────────────────────────────────
    // Phase
    // ──────────────────────────────────────────────
    public enum Phase
    {
        NotStarted,
        Round1,   // 2 players fight; 1 waits
        Round2,   // loser vs bye-player
        Final,    // both winners fight on 3×3
        Finished
    }

    // ──────────────────────────────────────────────
    // Public state
    // ──────────────────────────────────────────────
    public Phase CurrentPhase { get; private set; } = Phase.NotStarted;

    /// <summary>All three registered player names.</summary>
    public List<string> Players { get; private set; } = new() { "X", "O", "Z" };

    // Round participants (set each round)
    public string Round1PlayerA { get; private set; } = string.Empty;
    public string Round1PlayerB { get; private set; } = string.Empty;
    public string ByePlayer { get; private set; } = string.Empty; // waited in R1

    public string Round2PlayerA { get; private set; } = string.Empty; // loser R1
    public string Round2PlayerB { get; private set; } = string.Empty; // bye-player

    public string FinalPlayerA { get; private set; } = string.Empty;  // winner R1
    public string FinalPlayerB { get; private set; } = string.Empty;  // winner R2

    // Final standings (filled after Final)
    public string First { get; private set; } = string.Empty;
    public string Second { get; private set; } = string.Empty;
    public string Third { get; private set; } = string.Empty;

    // Internal tracking
    private string _winnerR1 = string.Empty;
    private string _loserR1 = string.Empty;
    private string _winnerR2 = string.Empty;
    private string _loserR2 = string.Empty;

    // ──────────────────────────────────────────────
    // Board size per phase
    // ──────────────────────────────────────────────
    public int BoardSize => CurrentPhase == Phase.Final ? 3 : 4;

    // ──────────────────────────────────────────────
    // Start the tournament
    // ──────────────────────────────────────────────
    public void Start(List<string>? players = null)
    {
        if (players != null && players.Count == 3)
            Players = new List<string>(players);

        Reset();
        BeginRound1();
    }

    // ──────────────────────────────────────────────
    // Register result for the current round
    // ──────────────────────────────────────────────
    /// <param name="winner">Name of the winner. Pass null / empty for a draw → replay (caller should re-ask).</param>
    public void RegisterResult(string? winner)
    {
        if (string.IsNullOrEmpty(winner))
            return; // draw – caller should replay the match

        switch (CurrentPhase)
        {
            case Phase.Round1:
                _winnerR1 = winner;
                _loserR1 = winner == Round1PlayerA ? Round1PlayerB : Round1PlayerA;
                BeginRound2();
                break;

            case Phase.Round2:
                _winnerR2 = winner;
                _loserR2 = winner == Round2PlayerA ? Round2PlayerB : Round2PlayerA;
                BeginFinal();
                break;

            case Phase.Final:
                string finalWinner = winner;
                string finalLoser = winner == FinalPlayerA ? FinalPlayerB : FinalPlayerA;

                First = finalWinner;
                Second = finalLoser;
                Third = _loserR2;

                CurrentPhase = Phase.Finished;
                break;
        }
    }

    // ──────────────────────────────────────────────
    // Helpers – advance phases
    // ──────────────────────────────────────────────
    private void BeginRound1()
    {
        CurrentPhase = Phase.Round1;

        // Pick two random players; third gets bye
        var shuffled = Players.OrderBy(_ => Guid.NewGuid()).ToList();
        Round1PlayerA = shuffled[0];
        Round1PlayerB = shuffled[1];
        ByePlayer = shuffled[2];
    }

    private void BeginRound2()
    {
        CurrentPhase = Phase.Round2;
        Round2PlayerA = _loserR1;
        Round2PlayerB = ByePlayer;
    }

    private void BeginFinal()
    {
        CurrentPhase = Phase.Final;
        FinalPlayerA = _winnerR1;
        FinalPlayerB = _winnerR2;
    }

    // ──────────────────────────────────────────────
    // Convenience – who plays now
    // ──────────────────────────────────────────────
    public (string playerA, string playerB) GetCurrentMatchPlayers()
    {
        return CurrentPhase switch
        {
            Phase.Round1 => (Round1PlayerA, Round1PlayerB),
            Phase.Round2 => (Round2PlayerA, Round2PlayerB),
            Phase.Final => (FinalPlayerA, FinalPlayerB),
            _ => (string.Empty, string.Empty)
        };
    }

    public string PhaseLabel() => CurrentPhase switch
    {
        Phase.Round1 => "Voor 1 (4×4)",
        Phase.Round2 => "Voor 2 (4×4)",
        Phase.Final => "Finaal (3×3)",
        Phase.Finished => "Turniiir lõppenud",
        _ => "Ei alustatud"
    };

    // ──────────────────────────────────────────────
    // Reset
    // ──────────────────────────────────────────────
    public void Reset()
    {
        CurrentPhase = Phase.NotStarted;
        Round1PlayerA = Round1PlayerB = ByePlayer = string.Empty;
        Round2PlayerA = Round2PlayerB = string.Empty;
        FinalPlayerA = FinalPlayerB = string.Empty;
        First = Second = Third = string.Empty;
        _winnerR1 = _loserR1 = _winnerR2 = _loserR2 = string.Empty;
    }
}