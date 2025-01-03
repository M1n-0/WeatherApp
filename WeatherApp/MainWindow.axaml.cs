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

        // Charger la clé API
        public class Config
        {
            public string ApiKey { get; set; }
        }

        public static Config LoadConfig()
        {
            string configPath = "config.json";
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<Config>(json);
            }
            else
            {
                throw new FileNotFoundException("Le fichier config.json est introuvable !");
            }
        }

        // Appel à l'API météo actuelle
        public async Task<JObject> GetCurrentWeatherAsync(string cityName)
        {
            string apiKey = LoadConfig().ApiKey;
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

        // Appel à l'API des prévisions météo
        public async Task<JObject> GetWeatherForecastAsync(string cityName)
        {
            string apiKey = LoadConfig().ApiKey;
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

        // Recherche météo
        private async void OnSearchClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                string cityName = CityInput.Text;
                JObject weatherData = await GetCurrentWeatherAsync(cityName);

                // Extraire les données
                string city = weatherData["name"].ToString();
                string description = weatherData["weather"][0]["description"].ToString();
                string temperature = weatherData["main"]["temp"].ToString();
                string humidity = weatherData["main"]["humidity"].ToString();

                // Afficher les résultats
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

        // Prévisions météo
        private async void OnForecastClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                string cityName = ForecastCityInput.Text;
                JObject forecastData = await GetWeatherForecastAsync(cityName);

                ForecastResults.Children.Clear();
                foreach (var item in forecastData["list"])
                {
                    string date = item["dt_txt"].ToString();
                    string description = item["weather"][0]["description"].ToString();
                    string temperature = item["main"]["temp"].ToString();

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

        // Sauvegarder les paramètres
        private void OnSaveSettingsClicked(object sender, RoutedEventArgs e)
        {
            string defaultCity = DefaultCityInput.Text;
            var options = new { DefaultCity = defaultCity };
            File.WriteAllText("options.json", JsonConvert.SerializeObject(options));
        }
    }
}
