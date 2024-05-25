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
                        await DisplayAlert("Ошибка", $"Ошибка: {response.StatusCode}", "OK");
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
