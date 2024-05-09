using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersPage : ContentPage
    {
        // URL вашего DRF API
        readonly string apiUrl = "http://192.168.1.16:8888/api/users/";

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
                            user.Image = "http://127.0.0.1:8888" + user.Image;
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

        public class MyUser
        {
            public int Id { get; set; }
            public string Image { get; set; }
            public DateTime LastLogin { get; set; }
            public bool IsSuperuser { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public bool IsStaff { get; set; }
            public bool IsActive { get; set; }
            public DateTime DateJoined { get; set; }
            public string Patronymic { get; set; }
            public bool UseEnglish { get; set; }
            public bool Anonymous { get; set; }
            public DateTime DataJoined { get; set; }
            public int RightAnswers { get; set; }
            public int WrongAnswers { get; set; }
            public List<object> Groups { get; set; }
            public List<object> UserPermissions { get; set; }
        }
    }
}
