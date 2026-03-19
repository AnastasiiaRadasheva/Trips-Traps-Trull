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
                    new Label { Text = " Eesmärk", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#e94560") },
                    new Label { Text = "Pane kolm oma märki ritta — horisontaalselt, vertikaalselt või diagonaalselt.", FontSize = 15, TextColor = Colors.White },

                    new Label { Text = " Kuidas mängida", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#e94560"), Margin = new Thickness(0, 10, 0, 0) },
                    new Label { Text = "1. Kaks mängijat vaheldumisi klõpsavad tühjale ruudule.", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "2. X alustab esimesena (või kasuta 'Kes alustab?' nuppu).", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "3. Esimene mängija, kes saab 3 märki ritta, võidab.", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "4. Kui kõik ruudud on täis ja keegi ei võitnud — viik!", FontSize = 15, TextColor = Colors.White },

                    new Label { Text = " Nõuanne", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#e94560"), Margin = new Thickness(0, 10, 0, 0) },
                    new Label { Text = "Püüa hõivata keskmine ruut — see annab kõige rohkem võiduvõimalusi!", FontSize = 15, TextColor = Colors.White },
                }
            }
        };

        // --- БЛОК: КАК ИГРАТЬ С БОТОМ ---
        var botTitle = new Label
        {
            Text = "Kuidas mängida botiga",
            FontSize = 24,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center
        };

        var botFrame = new Border
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
                    new Label { Text = "▶️ Kuidas alustada", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#e94560") },
                    new Label { Text = "Vajuta nuppu 'Sina vs Bot' — see lülitab boti sisse. Nupp muutub punaseks.", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "Sina mängid alati X-ga, bot mängib O-ga.", FontSize = 15, TextColor = Colors.White },


                    new Label { Text = " Boti tase kasvab", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#e94560"), Margin = new Thickness(0, 10, 0, 0) },
                    new Label { Text = "🟢 Algaja (0-2 mängu) — bot teeb palju vigu, lihtne võita.", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "🟡 Kesktase (3-6 mängu) — bot hakkab rohkem blokeerima.", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "🟠 Kogenud (7-10 mängu) — bot on juba päris tugev.", FontSize = 15, TextColor = Colors.White },
                    new Label { Text = "🔴 Meister (11+ mängu) — bot blokeerib peaaegu alati, raske võita.", FontSize = 15, TextColor = Colors.White },

                    new Label { Text = " Boti lähtestamine", FontSize = 18, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#e94560"), Margin = new Thickness(0, 10, 0, 0) },
                    new Label { Text = "Statistika lehel saad boti taseme nulli tagasi seada — bot unustab kõik ja muutub jälle nõrgaks.", FontSize = 15, TextColor = Colors.White },
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
                Children = { title, rulesFrame, botTitle, botFrame, btnBack }
            }
        };
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}