using System;
using System.Collections.Generic;
using System.Linq;  
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Diplom.models;
using static Diplom.config;

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
                        await DisplayAlert("Ошибка", $"Ошибка: {response.StatusCode}", "OK");
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Ошибка", $"Ошибка: {e.Message}", "OK");
            }
        }
    }
}