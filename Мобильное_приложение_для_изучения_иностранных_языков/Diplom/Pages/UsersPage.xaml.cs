using System;
using System.Collections.Generic;
using System.Linq;  
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Diplom.models;
using static Diplom.config;
using System.Text;
using System.Threading.Tasks;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersPage : ContentPage
    {
        private readonly string apiUrl = $"{Our_addres}/api/users/";

        public UsersPage()
        {
            InitializeComponent();
            GetDataFromAPI();
        }

        private async Task GetDataFromAPI()
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
                        List<MyUser> users = JsonConvert.DeserializeObject<List<MyUser>>(jsonResponse);

                        foreach (var user in users)
                        {
                            if (!string.IsNullOrEmpty(user.Image))
                            {
                                user.Image = $"{Our_addres}" + user.Image;
                            }
                        }

                        // Сортировка пользователей по правильным и неправильным ответам
                        users.Sort((u1, u2) =>
                        {
                            int rightAnswerComparison = u2.RightAnswers.CompareTo(u1.RightAnswers);
                            if (rightAnswerComparison == 0)
                            {
                                return u1.WrongAnswers.CompareTo(u2.WrongAnswers);
                            }
                            return rightAnswerComparison;
                        });

                        // Только топ 100 пользователей выводится
                        var topUsers = users.Take(100).ToList();

                        listView.ItemsSource = topUsers;
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
    }
}