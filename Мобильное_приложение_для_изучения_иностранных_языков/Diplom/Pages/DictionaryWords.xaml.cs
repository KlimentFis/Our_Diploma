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
using static Diplom.config;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DictionaryWords : ContentPage
    {
        private readonly string apiUrl = $"{Our_addres}/api/words/";
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
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            // Токен недействителен, обновляем его
                            bool refreshed = await RefreshToken();
                            if (refreshed)
                            {
                                // Повторяем запрос после обновления токена
                                GetDataFromAPI();
                            }
                            else
                            {
                                await DisplayAlert("Ошибка", "Не удалось обновить токен.", "OK");
                                // Отправляем пользователя на страницу авторизации
                                await Navigation.PushAsync(new LoginPage());
                            }
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", $"Ошибка: {response.StatusCode}", "OK");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Ошибка", $"Ошибка: {e.Message}", "OK");
            }
        }


        private async Task<bool> RefreshToken()
        {
            try
            {
                string apiUrl = $"{Our_addres}/api/token/";

                string username = Application.Current.Properties["Username"].ToString();
                string password = Application.Current.Properties["Password"].ToString();

                var requestData = new
                {
                    username = username,
                    password = password
                };

                using (HttpClient client = new HttpClient())
                {
                    string jsonData = JsonConvert.SerializeObject(requestData);
                    HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string tokenContent = await response.Content.ReadAsStringAsync();
                        var tokenData = JsonConvert.DeserializeObject<dynamic>(tokenContent);

                        Application.Current.Properties["AccessToken"] = tokenData.access;
                        await Application.Current.SavePropertiesAsync();
                        return true;
                    }
                    else
                    {
                        // Обработка ошибки при запросе нового токена
                        string errorContent = await response.Content.ReadAsStringAsync();
                        // Можно добавить логику для повторных попыток или обработать ошибку по своему усмотрению
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения при запросе нового токена
                Console.WriteLine($"Произошла ошибка при обновлении токена: {ex.Message}");
                return false;
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
