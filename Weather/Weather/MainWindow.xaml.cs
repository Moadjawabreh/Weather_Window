using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Weather
{
	public partial class MainWindow : Window
	{
		private const string apiKey = "4b8f491d04a1db4cebb5b9bc5ce693bb";

		public MainWindow()
		{
			InitializeComponent();
		}

		private async void GetWeather_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				double longitude = Convert.ToDouble(longitudeTextBox.Text);
				double latitude = Convert.ToDouble(latitudeTextBox.Text);

				string weatherInfo = await GetWeatherData(longitude, latitude);
				weatherInfoTextBlock.Text = weatherInfo;
			}
			catch (Exception ex)
			{
				weatherInfoTextBlock.Text = $"Error: {ex.Message}";
			}
		}

		private async Task<string> GetWeatherData(double longitude, double latitude)
		{
			string weatherInfo = "";

			for (int height = 0; height <= 200; height += 20)
			{
				using (var client = new HttpClient())
				{
					var request = new HttpRequestMessage
					{
						Method = HttpMethod.Get,
						RequestUri = new Uri($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric&height={height}")
					};

					HttpResponseMessage response = await client.SendAsync(request);

					if (response.IsSuccessStatusCode)
					{
						string responseBody = await response.Content.ReadAsStringAsync();
						dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

						double temperature = data.main.temp;
						double windSpeed = data.wind.speed;
						double windDirectionDegrees = (double)data.wind.deg; // Ensure wind direction is parsed as double

						string weatherDescription = data.weather[0].description;
						string windDirection = ConvertDegreesToCardinal(windDirectionDegrees);

						weatherInfo += $"Height: {height} meters\nTemperature: {temperature}°C\nWind Speed: {windSpeed} m/s\nWind Direction: {windDirection}\nWeather Description: {weatherDescription}\n\n";
					}
					else
					{
						return $"Failed to retrieve weather data. Status code: {response.StatusCode}";
					}
				}
			}

			return weatherInfo;
		}

		private string ConvertDegreesToCardinal(double degrees)
		{
			string[] cardinalDirections = {
				"North", "North-Northeast", "Northeast", "East-Northeast",
				"East", "East-Southeast", "Southeast", "South-Southeast",
				"South", "South-Southwest", "Southwest", "West-Southwest",
				"West", "West-Northwest", "Northwest", "North-Northwest"
			};

			int index = (int)Math.Round((degrees % 360) / 22.5);
			return cardinalDirections[index % 16];
		}
	}
}
