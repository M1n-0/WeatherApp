using Avalonia.Controls;
using Avalonia.Interactivity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WeatherApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public class Config
        {
            public string? ApiKey { get; set; }
        }

        public static Config LoadConfig()
        {
            string configPath = "config.json";
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                var config = JsonConvert.DeserializeObject<Config>(json);
                if (config == null || string.IsNullOrEmpty(config.ApiKey))
                    throw new Exception("Clé API introuvable ou invalide !");
                return config;
            }
            else
            {
                throw new FileNotFoundException("Le fichier config.json est introuvable !");
            }
        }
        public async Task<JObject> GetCurrentWeatherAsync(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                throw new ArgumentException("Le nom de la ville ne peut pas être vide.");

            string apiKey = LoadConfig().ApiKey ?? throw new Exception("Clé API manquante.");
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={cityName}&appid={apiKey}&units=metric&lang=fr";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(json);
                }
                else
                {
                    throw new Exception("Erreur lors de l'appel à l'API : " + response.ReasonPhrase);
                }
            }
        }
        public async Task<JObject> GetWeatherForecastAsync(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                throw new ArgumentException("Le nom de la ville ne peut pas être vide.");

            string apiKey = LoadConfig().ApiKey ?? throw new Exception("Clé API manquante.");
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={cityName}&appid={apiKey}&units=metric&lang=fr";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    return JObject.Parse(json);
                }
                else
                {
                    throw new Exception("Erreur lors de l'appel à l'API : " + response.ReasonPhrase);
                }
            }
        }
        private async void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                string cityName = CityInput.Text ?? string.Empty;
                JObject weatherData = await GetCurrentWeatherAsync(cityName);

                string city = weatherData["name"]?.ToString() ?? "Ville inconnue";
                string description = weatherData["weather"]?[0]?["description"]?.ToString() ?? "Non disponible";
                string temperature = weatherData["main"]?["temp"]?.ToString() ?? "N/A";
                string humidity = weatherData["main"]?["humidity"]?.ToString() ?? "N/A";

                WeatherResult.Text = $"Ville : {city}\n" +
                                     $"Description : {description}\n" +
                                     $"Température : {temperature}°C\n" +
                                     $"Humidité : {humidity}%";
            }
            catch (Exception ex)
            {
                WeatherResult.Text = $"Erreur : {ex.Message}";
            }
        }

        private async void OnForecastClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                string cityName = ForecastCityInput.Text ?? string.Empty;
                JObject forecastData = await GetWeatherForecastAsync(cityName);

                ForecastResults.Children.Clear();
                foreach (var item in forecastData["list"])
                {
                    string date = item["dt_txt"]?.ToString() ?? "N/A";
                    string description = item["weather"]?[0]?["description"]?.ToString() ?? "Non disponible";
                    string temperature = item["main"]?["temp"]?.ToString() ?? "N/A";

                    ForecastResults.Children.Add(new TextBlock
                    {
                        Text = $"{date} : {description}, {temperature}°C",
                        Margin = new Avalonia.Thickness(0, 5, 0, 5)
                    });
                }
            }
            catch (Exception ex)
            {
                ForecastResults.Children.Add(new TextBlock
                {
                    Text = $"Erreur : {ex.Message}",
                    Foreground = Avalonia.Media.Brushes.Red
                });
            }
        }

        private void OnSaveSettingsClicked(object sender, RoutedEventArgs e)
        {
            string defaultCity = DefaultCityInput.Text ?? string.Empty;
            var options = new { DefaultCity = defaultCity };
            File.WriteAllText("options.json", JsonConvert.SerializeObject(options));
        }
    }
}
