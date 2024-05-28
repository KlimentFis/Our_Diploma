using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Diplom.models;
using static Diplom.config;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestsTranslation : ContentPage
    {
        private readonly string apiUrl = $"{Our_addres}/api/words/";
        private string accessToken;
        private List<Words> wordsList;
        private Words currentWord;
        private RadioButton correctRadioButton;

        public TestsTranslation()
        {
            InitializeComponent();
            GetDataFromAPI();
            SendBtn.Clicked += SendBtn_Clicked;

            // Добавляем обработчики событий CheckedChanged для каждого RadioButton
            RadioBtn1.CheckedChanged += RadioButton_CheckedChanged;
            RadioBtn2.CheckedChanged += RadioButton_CheckedChanged;
            RadioBtn3.CheckedChanged += RadioButton_CheckedChanged;
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
                        // Отображение случайного слова и переводов
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
                                await GetDataFromAPI();
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


        private void DisplayRandomWord()
        {
            Random rand = new Random();
            currentWord = wordsList[rand.Next(0, wordsList.Count)];
            wordLabel.Text = currentWord.Name;

            // Собираем все переводы в один список
            List<string> translations = wordsList.Select(word => word.Translate).ToList();
            // Перемешиваем переводы
            translations = translations.OrderBy(a => rand.Next()).ToList();

            // Индекс правильного перевода
            int correctTranslationIndex = translations.IndexOf(currentWord.Translate);

            // Отображение переводов на RadioButton'ах
            RadioBtn1.Content = translations[0];
            RadioBtn2.Content = translations[1];
            RadioBtn3.Content = translations[2];

            // Установка правильного ответа на случайный RadioButton
            switch (rand.Next(1, 4))
            {
                case 1:
                    RadioBtn1.Content = currentWord.Translate;
                    correctTranslationIndex = 0;
                    break;
                case 2:
                    RadioBtn2.Content = currentWord.Translate;
                    correctTranslationIndex = 1;
                    break;
                case 3:
                    RadioBtn3.Content = currentWord.Translate;
                    correctTranslationIndex = 2;
                    break;
            }

            // Устанавливаем индекс правильного перевода
            correctRadioButton = correctTranslationIndex switch
            {
                0 => RadioBtn1,
                1 => RadioBtn2,
                _ => RadioBtn3
            };
        }


        private async void SendBtn_Clicked(object sender, EventArgs e)
        {
            MyUser user = null; // Инициализируем переменную, но не присваиваем значение Password сразу
            bool isAnswerCorrect = correctRadioButton.IsChecked;

            // Обновление UI в зависимости от правильного или неправильного ответа
            if (isAnswerCorrect)
            {
                ResultLabel.Text = "Правильно!";
                ResultFrame.BackgroundColor = Color.FromHex("#72c255");
            }
            else
            {
                string correctTranslation = correctRadioButton.Content.ToString();
                ResultLabel.Text = $"Неправильно! Правильное слово: {correctTranslation}";
                ResultFrame.BackgroundColor = Color.FromHex("#ff6161");
            }
            ResultFrame.IsVisible = true;

            string apiUrl = $"{Our_addres}/api/users/{Application.Current.Properties["Username"]}/";

            using (HttpClient client = new HttpClient())
            {
                string accessToken = Application.Current.Properties["AccessToken"].ToString();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                try
                {
                    // Получение данных пользователя
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        user = JsonConvert.DeserializeObject<MyUser>(content);

                        // Обновление количества правильных и неправильных ответов
                        if (isAnswerCorrect)
                        {
                            user.RightAnswers++;
                        }
                        else
                        {
                            user.WrongAnswers++;
                        }

                        // Присваиваем значение Password после получения данных пользователя
                        user.Password = Application.Current.Properties["Password"].ToString();

                        // Подготовка данных для отправки
                        string json = JsonConvert.SerializeObject(user);
                        HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                        // Отправка обновленных данных пользователя с использованием PUT запроса
                        HttpResponseMessage newResponse = await client.PutAsync(apiUrl, httpContent);
                        if (newResponse.IsSuccessStatusCode)
                        {
                            
                        }
                        else
                        {
                            string errorContent = await newResponse.Content.ReadAsStringAsync();
                            await DisplayAlert("Ошибка", $"Ошибка при отправке данных: {newResponse.StatusCode}\n{errorContent}", "OK");
                            Console.WriteLine($"Ошибка при отправке данных: {newResponse.StatusCode}\n{errorContent}");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось загрузить данные профиля.", "OK");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
                    return;
                }
            }

            DisplayRandomWord();
        }




        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // Получаем RadioButton, который вызвал событие
            var radioButton = (RadioButton)sender;

            // Если флажок установлен (RadioButton выбран), сбрасываем флажок для остальных RadioButton
            if (radioButton.IsChecked)
            {
                if (radioButton != RadioBtn1)
                    RadioBtn1.IsChecked = false;
                if (radioButton != RadioBtn2)
                    RadioBtn2.IsChecked = false;
                if (radioButton != RadioBtn3)
                    RadioBtn3.IsChecked = false;
            }
        }
    }
}
