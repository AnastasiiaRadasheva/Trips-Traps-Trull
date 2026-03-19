namespace TTT;

public partial class AppShell : Shell
{
    public AppShell()
    {
        Routing.RegisterRoute("main", typeof(main));
        Routing.RegisterRoute("StatsPage", typeof(StatsPage));
        Routing.RegisterRoute("RulesPage", typeof(RulesPage));
        Routing.RegisterRoute("SettingsPage", typeof(SettingsPage));

        CurrentItem = new ShellContent
        {
            Content = new StartPage()
        };
    }
}