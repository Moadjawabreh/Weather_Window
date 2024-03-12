using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Weather
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
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
			using (var client = new HttpClient())
			{
				var request = new HttpRequestMessage
				{
					Method = HttpMethod.Get,
					RequestUri = new Uri($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longitude}&appid={apiKey}&units=metric")
				};

				HttpResponseMessage response = await client.SendAsync(request);

				if (response.IsSuccessStatusCode)
				{
					string responseBody = await response.Content.ReadAsStringAsync();
					dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);

					double windSpeed = data.wind.speed;
					double windDirectionDegrees = data.wind.deg;
					string weatherDescription = data.weather[0].description;
					string windDirection = ConvertDegreesToCardinal(windDirectionDegrees);

					return $"Weather at latitude {latitude}, longitude {longitude}:\nWeather Description: {weatherDescription}\nWind Speed: {windSpeed} m/s\nWind Direction: {windDirection}";
				}
				else
				{
					return $"Failed to retrieve weather data. Status code: {response.StatusCode}";
				}
			}
		}

		private string ConvertDegreesToCardinal(double degrees)
		{
			string[] cardinalDirections = {
	"North",
	"North-Northeast",
	"Northeast",
	"East-Northeast",
	"East",
	"East-Southeast",
	"Southeast",
	"South-Southeast",
	"South",
	"South-Southwest",
	"Southwest",
	"West-Southwest",
	"West",
	"West-Northwest",
	"Northwest",
	"North-Northwest"
};
			int index = (int)Math.Round((degrees % 360) / 22.5);
			return cardinalDirections[index % 16];
		}
	}

}