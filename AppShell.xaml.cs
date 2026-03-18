namespace TTT
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {

            Routing.RegisterRoute(nameof(main), typeof(main));
            Routing.RegisterRoute(nameof(StatsPage), typeof(StatsPage));
            Routing.RegisterRoute(nameof(RulesPage), typeof(RulesPage));

            CurrentItem = new ShellContent
            {
                Content = new StartPage()
            };
        }
    }
}
