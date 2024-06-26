﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;
using System.Text;
using static Diplom.models;
using static Diplom.config;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.IO;

namespace Diplom.Pages
{
    public partial class TestsWords : ContentPage
    {
        private readonly string apiUrl = $"{Our_addres}/api/suggestions/";
        private string accessToken;
        private List<Suggestions> suggestionsList;
        private Suggestions currentSuggestion;
        private RadioButton correctRadioButton;

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

        private async Task GetSuggestionsFromAPI()
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
                        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        {
                            // Токен недействителен, обновляем его
                            bool refreshed = await RefreshToken();
                            if (refreshed)
                            {
                                // Повторяем запрос после обновления токена
                                await GetSuggestionsFromAPI();
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

            // Устанавливаем правильный ответ в correctRadioButton
            if (words[0] == currentSuggestion.RightWord) correctRadioButton = RadioBtn1;
            else if (words[1] == currentSuggestion.RightWord) correctRadioButton = RadioBtn2;
            else correctRadioButton = RadioBtn3;
        }

        private async void SendBtn_Clicked(object sender, EventArgs e)
        {
            string selectedWord = RadioBtn1.IsChecked ? RadioBtn1.Content.ToString() :
                                  RadioBtn2.IsChecked ? RadioBtn2.Content.ToString() :
                                  RadioBtn3.Content.ToString();

            bool isAnswerCorrect = selectedWord.Equals(currentSuggestion.RightWord);

            if (isAnswerCorrect)
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

            await UpdateUserStatistics(isAnswerCorrect);
            DisplayRandomSuggestion();
        }

        private async System.Threading.Tasks.Task UpdateUserStatistics(bool isAnswerCorrect)
        {
            MyUser user = null; // Инициализируем переменную, но не присваиваем значение Password сразу
            string apiUrl = $"{Our_addres}/api/users/{Application.Current.Properties["Username"]}/";

            using (HttpClient client = new HttpClient())
            {
                string accessToken = Application.Current.Properties["AccessToken"].ToString();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        user = JsonConvert.DeserializeObject<MyUser>(content);

                        if (isAnswerCorrect)
                        {
                            user.RightAnswers++;
                        }
                        else
                        {
                            user.WrongAnswers++;
                        }

                        user.Password = Application.Current.Properties["Password"].ToString();

                        using (var multipartContent = new MultipartFormDataContent())
                        {
                            multipartContent.Add(new StringContent(user.Username), "username");
                            multipartContent.Add(new StringContent(user.Password), "password");
                            multipartContent.Add(new StringContent(user.FirstName), "first_name");
                            multipartContent.Add(new StringContent(user.LastName), "last_name");
                            multipartContent.Add(new StringContent(user.Patronymic), "patronymic");
                            multipartContent.Add(new StringContent(user.Anonymous.ToString()), "anonymous");
                            multipartContent.Add(new StringContent(user.UseEnglish.ToString()), "use_english");
                            multipartContent.Add(new StringContent(user.RightAnswers.ToString()), "right_answers");
                            multipartContent.Add(new StringContent(user.WrongAnswers.ToString()), "wrong_answers");

                            if (!string.IsNullOrEmpty(user.Image) && File.Exists(user.Image))
                            {
                                var photoContent = new StreamContent(File.OpenRead(user.Image));
                                photoContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                                multipartContent.Add(photoContent, "image", Path.GetFileName(user.Image));
                            }
                            else
                            {
                                //Console.WriteLine("Image path is invalid or file does not exist.");
                            }

                            HttpResponseMessage updateResponse = await client.PutAsync(apiUrl, multipartContent);
                            if (updateResponse.IsSuccessStatusCode)
                            {
                                //Console.WriteLine("User data updated successfully.");
                            }
                            else
                            {
                                string errorContent = await updateResponse.Content.ReadAsStringAsync();
                                await DisplayAlert("Ошибка", $"Ошибка при отправке данных: {updateResponse.StatusCode}\n{errorContent}", "OK");
                                //Console.WriteLine($"Ошибка при отправке данных: {updateResponse.StatusCode}\n{errorContent}");
                            }
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
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            var radioButton = (RadioButton)sender;

            if (radioButton.IsChecked)
            {
                if (radioButton != RadioBtn1) RadioBtn1.IsChecked = false;
                if (radioButton != RadioBtn2) RadioBtn2.IsChecked = false;
                if (radioButton != RadioBtn3) RadioBtn3.IsChecked = false;
            }
        }
    }
}
