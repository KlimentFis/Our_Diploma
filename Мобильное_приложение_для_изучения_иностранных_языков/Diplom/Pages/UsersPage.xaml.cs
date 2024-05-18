using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Diplom.models;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersPage : ContentPage
    {
        readonly string apiUrl = "http://test.bipchik.keenetic.pro/api/users/";

        public UsersPage()
        {
            InitializeComponent();
            GetDataFromAPI();
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
                        // Десериализация JSON в список пользователей
                        List<MyUser> users = JsonConvert.DeserializeObject<List<MyUser>>(jsonResponse);
                        // Добавление адреса к изображениям
                        foreach (var user in users)
                        {
                            user.Image = "http://test.bipchik.keenetic.pro" + user.Image;
                            Console.WriteLine(user.Image);
                        }
                        // Отображение списка пользователей на странице
                        listView.ItemsSource = users;
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
