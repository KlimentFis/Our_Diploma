using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Diplom.models;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DictionaryWords : ContentPage
    {
        readonly string apiUrl = "http://test.bipchik.keenetic.pro/api/words/";
        List<Words> wordsList = new List<Words>();
        List<Words> filteredWordsList = new List<Words>();

        public DictionaryWords()
        {
            InitializeComponent();
            GetDataFromAPI();
        }

        private async void GetDataFromAPI()
        {
            try
            {
                string accessToken = Application.Current.Properties["AccessToken"].ToString();

                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        wordsList = JsonConvert.DeserializeObject<List<Words>>(jsonResponse);
                        filteredWordsList = wordsList;
                        listView.ItemsSource = filteredWordsList;
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", $"Ошибка: {response.StatusCode}", "OK");
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Ошибка", $"Ошибка: {e.Message}", "OK");
            }
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = e.NewTextValue.ToLower();
            if (string.IsNullOrEmpty(filter))
            {
                filteredWordsList = wordsList;
            }
            else
            {
                filteredWordsList = wordsList
                    .Where(word => word.Name.ToLower().Contains(filter) || word.Translate.ToLower().Contains(filter))
                    .ToList();
            }

            listView.ItemsSource = filteredWordsList;
        }
    }
}
