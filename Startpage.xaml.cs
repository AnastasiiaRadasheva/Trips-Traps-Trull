namespace TTT;

public partial class StartPage : ContentPage
{
    VerticalStackLayout vst;
    ScrollView sv;

    public List<ContentPage> Lehed = new List<ContentPage>()
    {
        new main(),
        new StatsPage(),
        new RulesPage(),
        new SettingsPage()
    };

    public List<string> LeheNimed = new List<string>()
    {
        "Uus mäng",
        "Statistika",
        "Reeglid",
        "Seaded"
    };

    public StartPage()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");

        var title = new Label
        {
            Text = "Trips-Traps-Trull",
            FontSize = 36,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var subtitle = new Label
        {
            Text = "X  O  X",
            FontSize = 28,
            TextColor = Color.FromArgb("#e94560"),
            HorizontalOptions = LayoutOptions.Center,
            Margin = new Thickness(0, 0, 0, 20)
        };

        vst = new VerticalStackLayout
        {
            Padding = 20,
            Spacing = 15,
            VerticalOptions = LayoutOptions.Center
        };

        vst.Children.Add(title);
        vst.Children.Add(subtitle);
        for (int i = 0; i < Lehed.Count; i++)
        {
            int index = i;

            Button nupp = new Button
            {
                Text = LeheNimed[i],
                FontSize = 18,
                BackgroundColor = index == 0
                    ? Color.FromArgb("#e94560")
                    : Color.FromArgb("#16213e"),
                TextColor = Colors.White,
                CornerRadius = 12,
                HeightRequest = 55,
                WidthRequest = 220,
                BorderColor = Color.FromArgb("#e94560"),
                BorderWidth = 1
            };

            nupp.Clicked += async (sender, e) =>
            {
                if (index == 0)
                {
                    await Navigation.PushAsync(new ModeSelectPage());
                }
                else
                {
                    var valik = Lehed[index];
                    await Navigation.PushAsync(valik);
                }
            };

            vst.Children.Add(nupp);
        }

        sv = new ScrollView { Content = vst };
        Content = sv;
    }
}