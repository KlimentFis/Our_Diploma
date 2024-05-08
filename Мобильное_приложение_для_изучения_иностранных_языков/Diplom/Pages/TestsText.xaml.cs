using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Diplom.Pages
{
    public partial class TestsText : ContentPage
    {
        public TestsText()
        {
            InitializeComponent();
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            string textToCheck = EditorTxt.Text;
            List<string> errors = await CheckGrammarAsync(textToCheck);
            if (errors.Count > 0)
            {
                // Вывод ошибок
                _=DisplayAlert("Грамматические ошибки", string.Join("\n", errors), "OK");
            }
            else
            {
                // В случае отсутствия ошибок
                await DisplayAlert("Проверка завершена", "Грамматических ошибок не обнаружено", "OK");
            }
        }

        private async Task<List<string>> CheckGrammarAsync(string text)
        {
            List<string> errors = new List<string>();

            // API URL для проверки грамматики
            string apiUrl = "https://languagetool.org/api/v2/check";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Параметры запроса
                    var parameters = new Dictionary<string, string>
                    {
                        { "language", "en-US" }, // Язык текста
                        { "text", text } // Текст для проверки
                    };

                    // Отправка POST-запроса к API LanguageTool
                    var response = await client.PostAsync(apiUrl, new FormUrlEncodedContent(parameters));

                    // Обработка ответа
                    if (response.IsSuccessStatusCode)
                    {
                        // Преобразование ответа в JSON
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(jsonResponse);

                        // Извлечение списка ошибок из JSON
                        var matches = jsonObject["matches"];
                        foreach (var match in matches)
                        {
                            errors.Add(match["message"].ToString());
                        }
                    }
                    else
                    {
                        errors.Add("Ошибка при запросе к API LanguageTool");
                    }
                }
            }
            catch (Exception ex)
            {
                errors.Add("Ошибка: " + ex.Message);
            }

            return errors;
        }
    }
}
