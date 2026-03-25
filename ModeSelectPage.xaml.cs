namespace TTT;

public partial class ModeSelectPage : ContentPage
{
    public ModeSelectPage()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");

        var title = new Label
        {
            Text = "Vali mängurežiim",
            FontSize = 28,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var btn2Players = new Button
        {
            Text = " 2 mängijat (või koos botiga)",
            FontSize = 18,
            BackgroundColor = Color.FromArgb("#e94560"),
            TextColor = Colors.White,
            CornerRadius = 12,
            HeightRequest = 60,
            WidthRequest = 280,
            HorizontalOptions = LayoutOptions.Fill
        };

        btn2Players.Clicked += async (s, e) =>
        {
            await Navigation.PushAsync(new main());
        };

        var btnTournament = new Button
        {
            Text = " Turniir (3 mängijat)",
            FontSize = 18,
            BackgroundColor = Color.FromArgb("#16213e"),
            TextColor = Colors.White,
            CornerRadius = 12,
            HeightRequest = 60,
            WidthRequest = 280,
            BorderColor = Color.FromArgb("#e94560"),
            BorderWidth = 1,
             HorizontalOptions = LayoutOptions.Fill
        };

        btnTournament.Clicked += async (s, e) =>
        {
            await Navigation.PushAsync(new TournamentPage());
        };

        Content = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 20,
            Padding = new Thickness(40),
            Children =
            {
                title,
                btn2Players,
                btnTournament
            }
        };
    }
}