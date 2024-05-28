using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using static Diplom.models;
using static Diplom.config;
using System.Text;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestsPage : ContentPage
    {
        private readonly string apiUrl = $"{Our_addres}/api/words/";
        private string accessToken;
        private List<Words> wordsList;
        private Words currentWord;

        public TestsPage()
        {
            InitializeComponent();
            InitializePage();
        }

        private async void InitializePage()
        {
            if (!IsUserLoggedIn())
            {
                await DisplayAlert("Необходима авторизация", "Для того, чтобы увидеть слово дня, необходимо авторизироваться", "OK");
                return;
            }

            await CheckAndUpdateWordOfDay();
        }

        private async Task CheckAndUpdateWordOfDay()
        {
            if (Application.Current.Properties.ContainsKey("LastUpdateDate"))
            {
                DateTime lastUpdateDate = (DateTime)Application.Current.Properties["LastUpdateDate"];
                if (lastUpdateDate.Date == DateTime.Now.Date)
                {
                    // Загружаем сохраненное слово дня
                    currentWord = new Words
                    {
                        Name = Application.Current.Properties["WordOfDay"].ToString(),
                        Translate = Application.Current.Properties["WordTranslate"].ToString()
                    };
                    wordLabel.Text = currentWord.Name;
                    translateLabel.Text = currentWord.Translate;
                    return;
                }
            }

            await GetDataFromAPI();
        }

        private async Task GetDataFromAPI()
        {
            try
            {
                // Получение access токена из хранилища
                accessToken = Application.Current.Properties["AccessToken"].ToString();

                // Создание объекта HttpClient для отправки запросов
                using (HttpClient client = new HttpClient())
                {
                    // Создание объекта HttpRequestMessage
                    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                    // Добавление токена в заголовок запроса "Authorization"
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    // Отправка запроса и получение ответа
                    HttpResponseMessage response = await client.SendAsync(request);

                    // Проверка успешности запроса
                    if (response.IsSuccessStatusCode)
                    {
                        // Чтение ответа в формате JSON
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        // Десериализация JSON в список слов
                        wordsList = JsonConvert.DeserializeObject<List<Words>>(jsonResponse);
                        DisplayRandomWord();
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
                                await CheckAndUpdateWordOfDay();
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


        private void DisplayRandomWord()
        {
            Random rand = new Random();
            currentWord = wordsList[rand.Next(0, wordsList.Count)];
            wordLabel.Text = currentWord.Name;
            translateLabel.Text = currentWord.Translate;

            // Сохранение текущего слова и перевода в локальное хранилище
            Application.Current.Properties["WordOfDay"] = currentWord.Name;
            Application.Current.Properties["WordTranslate"] = currentWord.Translate;
            Application.Current.Properties["LastUpdateDate"] = DateTime.Now;

            Application.Current.SavePropertiesAsync();
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


        private async void OnFrameWords(object sender, EventArgs e)
        {
            if (!IsUserLoggedIn())
            {
                await Navigation.PushAsync(new LoginPage());
                return;
            }
            else
            {
                await Navigation.PushAsync(new TestsWords());
            }
        }

        private async void OnFrameTranslation(object sender, EventArgs e)
        {
            if (!IsUserLoggedIn())
            {
                await Navigation.PushAsync(new LoginPage());
                return;
            }
            else
            {
                await Navigation.PushAsync(new TestsTranslation());
                return;
            }
        }

        private async void OnFrameText(object sender, EventArgs e)
        {
            if (!IsUserLoggedIn())
            {
                await Navigation.PushAsync(new LoginPage());
                return;
            }
            else
            {
                await Navigation.PushAsync(new TestsText());
            }
        }

        private bool IsUserLoggedIn()
        {
            // Проверка наличия токена в хранилище и его не пустое значение
            if (Application.Current.Properties.ContainsKey("AccessToken") && Application.Current.Properties["AccessToken"] != null)
            {
                // Токен присутствует, пользователь вошел в систему
                return true;
            }
            else
            {
                // Токен отсутствует или его значение null, пользователь не вошел в систему
                return false;
            }
        }
    }
}
