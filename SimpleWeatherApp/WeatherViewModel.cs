using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SimpleWeatherApp
{
    class WeatherViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        static string APIKey = "[PLACE YOUR API KEY HERE]";
        static string cityURL = "http://dataservice.accuweather.com/locations/v1/cities/";
        static string conditionsURL = "http://dataservice.accuweather.com/currentconditions/v1/";

        public string DisplayMessage
        {
            get
            {
                if (string.IsNullOrEmpty(Key))
                {
                    return "-";
                }

                else
                {
                    return $"Location Key: {Key}" + Environment.NewLine + CityName + $" is located at {Region}, {Country}";
                }
                
            }
        }

        string keyWord;
        public string Keyword
        {
            get
            {
                return keyWord;
            }

            set
            {
                keyWord = value;
                OnPropertyChanged("Keyword");
            }
        }

        string key;
        public string Key
        {
            get
            {
                return key;
            }

            set
            {
                key = value;
                OnPropertyChanged("Key");
            }
        }

        string cityName;
        public string CityName
        {
            get
            {
                return cityName;
            }

            set
            {
                cityName = value;
                OnPropertyChanged("CityName");
            }
        }

        string region;
        public string Region
        {
            get
            {
                return region;
            }

            set
            {
                region = value;
                OnPropertyChanged("Region");
            }
        }

        string country;
        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
                OnPropertyChanged("Country");
            }
        }

        public string DisplayWeatherMessage
        {
            get
            {
                if (string.IsNullOrEmpty(WeatherText))
                {
                    return "-";
                }

                else
                {
                    return WeatherText + " " + Temperature;
                }

            }
        }

        string weatherText;
        public string WeatherText
        {
            get
            {
                return weatherText;
            }

            set
            {
                weatherText = value;
                OnPropertyChanged("WeatherText");
            }
        }

        string temperature;
        public string Temperature
        {
            get
            {
                return temperature;
            }

            set
            {
                temperature = value;
                OnPropertyChanged("Temperature");
            }
        }

        bool isBusy;
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }

            set
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public ICommand CitySearchCommand { get; set; }

        public WeatherViewModel()
        {
            CitySearchCommand = new Command(CitySearch);
        }

        private async void CitySearch()
        {
            IsBusy = true;

            var apiUri = cityURL + "search?apikey=" + APIKey + "&q=" + keyWord;

            var httpClient = new System.Net.Http.HttpClient();
            var httpResponse = await httpClient.GetAsync(apiUri);
            var httpResult = httpResponse.Content.ReadAsStringAsync().Result;

            var httpData = JArray.Parse(httpResult);

            foreach(var data in httpData)
            {
                Key = (string)data["Key"];
                CityName = (string)data["EnglishName"];

                var region = (JObject)data["Region"];
                Region = (string)region["EnglishName"];

                var country = (JObject)data["Country"];
                Country = (string)country["EnglishName"];
                OnPropertyChanged(nameof(DisplayMessage));

                break;
            }

            IsBusy = false;

            WeatherCondition();
        }

        private async void WeatherCondition()
        {
            IsBusy = true;

            var apiUri = conditionsURL + Key + "?apikey=" + APIKey;

            var httpClient = new System.Net.Http.HttpClient();
            var httpResponse = await httpClient.GetAsync(apiUri);
            var httpResult = httpResponse.Content.ReadAsStringAsync().Result;

            var httpData = JArray.Parse(httpResult);

            foreach (var data in httpData)
            {
                WeatherText = (string)data["WeatherText"];

                var temperature = (JObject)data["Temperature"];
                var metric = (JObject)temperature["Metric"];

                Temperature = (string)metric["Value"];
                OnPropertyChanged(nameof(DisplayWeatherMessage));
                break;
            }

            IsBusy = false;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new 
                PropertyChangedEventArgs(propertyName));
        }
    }
}
