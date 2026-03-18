using Microsoft.Maui.Controls.Shapes;

namespace TTT;

public partial class RulesPage : ContentPage
{
    public RulesPage()
    {
        BackgroundColor = Color.FromArgb("#1a1a2e");
        Title = "Reeglid";

        var title = new Label
        {
            Text = "📜 Mängureeglid",
            FontSize = 28,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var rulesFrame = new Border
        {
            BackgroundColor = Color.FromArgb("#16213e"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Stroke = Color.FromArgb("#e94560"),
            Padding = new Thickness(20),
            Content = new VerticalStackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label { Text = "🎯 Eesmärk", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#e94560") },
                    new Label { Text = "Pane kolm oma märki ritta — horisontaalselt, vertikaalselt või diagonaalselt.", FontSize = 15, TextColor = Colors.White },

                    new Label { Text = "🕹️ Kuidas mängida", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#e94560"), Margin = new Thickness(0, 10, 0, 0) },
                    new Label { Text = "1. Kaks mängijat vaheldumisi klõpsavad tühjale ruudule.", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "2. X alustab esimesena (või kasuta 'Kes alustab?' nuppu).", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "3. Esimene mängija, kes saab 3 märki ritta, võidab.", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "4. Kui kõik ruudud on täis ja keegi ei võitnud — viik!", FontSize = 15, TextColor = Colors.White },

                    new Label { Text = "💡 Nõuanne", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#e94560"), Margin = new Thickness(0, 10, 0, 0) },
                    new Label { Text = "Püüa hõivata keskmine ruut — see annab kõige rohkem võiduvõimalusi!", FontSize = 15, TextColor = Colors.White },
                }
            }
        };

        var btnBack = new Button
        {
            Text = "← Tagasi",
            FontSize = 16,
            BackgroundColor = Color.FromArgb("#e94560"),
            TextColor = Colors.White,
            CornerRadius = 10,
            HeightRequest = 48
        };
        btnBack.Clicked += OnBackClicked;

        Content = new ScrollView
        {
            Padding = new Thickness(24),
            Content = new VerticalStackLayout
            {
                Spacing = 16,
                Children = { title, rulesFrame, btnBack }
            }
        };
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}