﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Diplom.config;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private readonly string usersUrl = $"{Our_addres}/api/all_users/";
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string username = LoginEntry.Text;
                string password = PasswordEntry.Text;

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("Ошибка", "Введите логин и пароль", "OK");
                    return;
                }

 
                Application.Current.Properties["Username"] = username;
                Application.Current.Properties["Password"] = password;

                using (HttpClient client = new HttpClient())
                {
                    // Извлекаем список всех пользователей
                    HttpResponseMessage usersResponse = await client.GetAsync(usersUrl);
                    if (usersResponse.IsSuccessStatusCode)
                    {
                        string usersContent = await usersResponse.Content.ReadAsStringAsync();
                        var usersList = JsonConvert.DeserializeObject<string[]>(usersContent);

                        // Проверка на существующего пользователя
                        if (Array.Exists(usersList, user => user == username))
                        {
                            string authUrl = $"{Our_addres}/api/token/";

                            string jsonData = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";
                            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                            HttpResponseMessage authResponse = await client.PostAsync(authUrl, content);

                            if (authResponse.IsSuccessStatusCode)
                            {
                                string responseContent = await authResponse.Content.ReadAsStringAsync();
                                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

                                string refreshToken = responseObject.refresh;
                                string accessToken = responseObject.access;

                                Application.Current.Properties["RefreshToken"] = refreshToken;
                                Application.Current.Properties["AccessToken"] = accessToken;
                                await Application.Current.SavePropertiesAsync();


                                // Создаем новую главную страницу, чтобы открыть страницу авторизации
                                MainPage mainPage = new MainPage();
                                mainPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(ProfilePage)));
                                Application.Current.MainPage = mainPage;
                            }
                            else
                            {
                                await DisplayAlert("Ошибка", $"Ошибка при выполнении запроса: {authResponse.StatusCode}", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", "Неправильный логин или пароль", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Неправильный логин или пароль", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка: {ex.Message}", "OK");
            }
        }


        private async void SignUpPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }
    }
}
