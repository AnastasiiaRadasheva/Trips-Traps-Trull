namespace TTT;

public class TournamentManager
{
    public enum Phase
    {
        NotStarted,
        Round1,
        Round2,
        Final,
        Finished
    }

    public Phase CurrentPhase { get; private set; } = Phase.NotStarted;

    public List<string> Players { get; private set; } = new() { "X", "O", "Z" };
    public List<string> ActivePlayers { get; private set; } = new();

    public int BoardSize =>
        CurrentPhase == Phase.Round1 ? 4 : 3;

    // результаты
    public string First { get; private set; } = "";
    public string Second { get; private set; } = "";
    public string Third { get; private set; } = "";

    private string _r1Winner = "";

    public void Start(List<string> players)
    {
        Players = new List<string>(players);
        ActivePlayers = new List<string>(Players);
        CurrentPhase = Phase.Round1;
    }

    public void RegisterResult(string winner)
    {
        if (CurrentPhase == Phase.Round1)
        {
            _r1Winner = winner;

            // в раунд 2 идут проигравшие
            ActivePlayers = Players.Where(p => p != winner).ToList();

            CurrentPhase = Phase.Round2;
        }
        else if (CurrentPhase == Phase.Round2)
        {
            // проигравший = 3 место
            Third = ActivePlayers.First(p => p != winner);

            // финал
            ActivePlayers = new List<string> { _r1Winner, winner };

            CurrentPhase = Phase.Final;
        }
        else if (CurrentPhase == Phase.Final)
        {
            First = winner;
            Second = ActivePlayers.First(p => p != winner);

            CurrentPhase = Phase.Finished;
        }
    }

    public void Reset()
    {
        CurrentPhase = Phase.NotStarted;
        ActivePlayers.Clear();
        First = Second = Third = "";
        _r1Winner = "";
    }
}