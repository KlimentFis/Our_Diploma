using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using static Diplom.models;

namespace Diplom.Pages
{
    public partial class TestsWords : ContentPage
    {
        //private readonly string apiUrl = "http://192.168.1.16:8888/api/suggestions/";
        private readonly string apiUrl = config.Our_addres;
        private string accessToken;
        private List<Suggestions> suggestionsList;
        private Suggestions currentSuggestion;

        public TestsWords()
        {
            InitializeComponent();
            GetSuggestionsFromAPI();
            SendBtn.Clicked += SendBtn_Clicked;

            // Добавляем обработчики событий CheckedChanged для каждого RadioButton
            RadioBtn1.CheckedChanged += RadioButton_CheckedChanged;
            RadioBtn2.CheckedChanged += RadioButton_CheckedChanged;
            RadioBtn3.CheckedChanged += RadioButton_CheckedChanged;
        }

        private async void GetSuggestionsFromAPI()
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
                        // Десериализация JSON в список предложений
                        suggestionsList = JsonConvert.DeserializeObject<List<Suggestions>>(jsonResponse);
                        Console.WriteLine(jsonResponse);
                        // Отображение случайного предложения
                        DisplayRandomSuggestion();
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

        private void DisplayRandomSuggestion()
        {
            Random rand = new Random();
            currentSuggestion = suggestionsList[rand.Next(0, suggestionsList.Count)];
            SuggestionLabel.Text = currentSuggestion.Suggestion.Replace(currentSuggestion.RightWord, "__________");

            List<string> words = new List<string>
            {
                currentSuggestion.RightWord
            };

            // Добавляем два случайных слова из right_word
            while (words.Count < 3)
            {
                string randomWord = suggestionsList[rand.Next(0, suggestionsList.Count)].RightWord;
                if (!words.Contains(randomWord))
                    words.Add(randomWord);
            }

            // Перемешиваем список слов
            words = words.OrderBy(x => rand.Next()).ToList();

            // Устанавливаем слова в RadioButton'ы
            RadioBtn1.Content = words[0];
            RadioBtn2.Content = words[1];
            RadioBtn3.Content = words[2];
        }

        private void SendBtn_Clicked(object sender, EventArgs e)
        {
            string selectedWord = RadioBtn1.IsChecked ? RadioBtn1.Content.ToString() :
                                  RadioBtn2.IsChecked ? RadioBtn2.Content.ToString() :
                                  RadioBtn3.Content.ToString();
                
            

            if (selectedWord.Equals(currentSuggestion.RightWord))
            {
                ResultLabel.Text = "Правильно!";
                ResultFrame.BackgroundColor = Color.FromHex("#72c255");
            }
            else
            {
                ResultLabel.Text = $"Неправильно! Правильное слово: {currentSuggestion.RightWord}";
                ResultFrame.BackgroundColor = Color.FromHex("#ff6161");
            }
            ResultFrame.IsVisible = true;
            DisplayRandomSuggestion();
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
