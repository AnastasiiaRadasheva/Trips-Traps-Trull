namespace TTT;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute("main", typeof(main));
        Routing.RegisterRoute("StatsPage", typeof(StatsPage));
        Routing.RegisterRoute("RulesPage", typeof(RulesPage));
        Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));
        Routing.RegisterRoute("TournamentPage", typeof(TournamentPage));

        CurrentItem = new ShellContent
        {
            Content = new StartPage()
        };
    }
}