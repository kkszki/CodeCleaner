using Microsoft.Win32;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace CodeCleaner
{
  
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TemaTextBox.Focus();
        }

        private void TallozasButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "C# fájlok (*.cs)|*.cs"
            };

            if (dialog.ShowDialog() == true)
            {
                string fajlUt = dialog.FileName;
                string kod = File.ReadAllText(fajlUt);
                KodTextBox.Text = kod;
            }
        }

       

        private async void Bekuld_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tema = TemaTextBox.Text;
                string kod = KodTextBox.Text;

                if (string.IsNullOrWhiteSpace(tema) || string.IsNullOrWhiteSpace(kod))
                {
                    MessageBox.Show("Add meg a témát és válassz ki egy fájlt!");
                    return;
                }

                string prompt = $@"
Te egy tapasztalt programozó vagy.

A program témája: {tema}

Feladatod:
Elemezd az alábbi forráskódot!
Keresd meg az értelmetlen változóneveket.
Javasolj helyettük beszédes neveket.

FONTOS SZABÁLYOK:
- Ne változtasd meg a program logikáját!
- A namespace-t se változtasd soha! Mindig csak a class-on belül módosíthatsz!
- Ne módosítsd az algoritmust!
- Ne írj át más kódrészeket!
- Csak változónevekre és függvényekre, eljárásokra adj javaslatokat!
- Ne használj a változó névben szóközt, se ékezetet.
- Ugyanazzal a sorral kezdődjön! tehát ne úgy, hogy c#
- Írd meg a komplett kódot újra, az új változó nevekkel. és csak ennyit küldj vissza!

A forráskód:

{kod}
";

                var requestData = new
                {
                    model = "qwen2.5-coder:7b",
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    },
                    stream = false
                };

                string json = JsonSerializer.Serialize(requestData);

                var content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await client.PostAsync(
                    "http://localhost:11434/api/chat",
                    content
                );

                response.EnsureSuccessStatusCode();

                string responseText = await response.Content.ReadAsStringAsync();

                using JsonDocument document = JsonDocument.Parse(responseText);

                JsonElement message = document.RootElement.GetProperty("message");

                string aiValasz = message.GetProperty("content").GetString() ?? "";

                JavitottKodTextBox.Text = aiValasz;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Hiba történt az AI kommunikáció során:\n" + ex.Message
                );
            }
        }

        private void MentesButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(JavitottKodTextBox.Text))
            {
                MessageBox.Show("Nincs menthető kód! Előbb végezd el az AI elemzést.");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "C# fájlok (*.cs)|*.cs|Minden fájl (*.*)|*.*",
                DefaultExt = "cs",
                FileName = "JavitottKod.cs"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, JavitottKodTextBox.Text, Encoding.UTF8);
                MessageBox.Show("A fájl sikeresen elmentve!");
            }
        }
    }
}