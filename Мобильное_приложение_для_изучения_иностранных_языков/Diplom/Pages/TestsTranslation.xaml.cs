using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestsTranslation : ContentPage
    {
        private readonly string apiUrl = "http://test.bipchik.keenetic.pro/api/words/";
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

        private async void GetDataFromAPI()
        {
            try
            {
                // Получение access токена из хранилища
                string accessToken = Application.Current.Properties["AccessToken"].ToString();

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
            // Проверка выбранного ответа
            if (correctRadioButton.IsChecked)
            {
                await DisplayAlert("Результат", "Правильно!", "OK");
            }
            else
            {
                await DisplayAlert("Результат", "Неправильно!", "OK");
            }
            // Повторное отображение следующего случайного слова
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

        public class Words
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("translate")]
            public string Translate { get; set; }
        }
    }
}
