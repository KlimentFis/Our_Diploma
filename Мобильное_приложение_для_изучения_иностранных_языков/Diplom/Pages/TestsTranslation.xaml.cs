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
            // Выбор случайного слова из списка
            Random rand = new Random();
            currentWord = wordsList[rand.Next(0, wordsList.Count)];
            // Отображение английского слова
            wordLabel.Text = currentWord.Name;
            // Случайное перемешивание списка переводов
            List<string> translations = new List<string> { currentWord.Translate };
            translations.AddRange(wordsList.Where(word => word != currentWord).Select(word => word.Translate));
            translations = translations.OrderBy(a => rand.Next()).ToList();
            // Отображение переводов на RadioButton'ах
            RadioBtn1.Content = translations[0];
            RadioBtn2.Content = translations[1];
            RadioBtn3.Content = translations[2];
            RadioBtn1.CheckedChanged += RadioButton_CheckedChanged;
            RadioBtn2.CheckedChanged += RadioButton_CheckedChanged;
            RadioBtn3.CheckedChanged += RadioButton_CheckedChanged;
            // Пометка правильного перевода
            correctRadioButton = RadioBtn1;
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
                await DisplayAlert("Результат", "Неправильно, попробуйте еще раз.", "OK");
            }
            // Повторное отображение следующего случайного слова
            DisplayRandomWord();
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (e.Value)
            {
                // Снимаем флажки с других RadioButton
                if (sender != RadioBtn1)
                    RadioBtn1.IsChecked = false;
                if (sender != RadioBtn2)
                    RadioBtn2.IsChecked = false;
                if (sender != RadioBtn3)
                    RadioBtn3.IsChecked = false;

                // Устанавливаем правильный RadioButton
                correctRadioButton = (RadioButton)sender;
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
