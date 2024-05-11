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

        public class MyUser
        {
            [JsonProperty("id")]
            public int Id { get; set; }
            [JsonProperty("image")]
            public string Image { get; set; }
            [JsonProperty("last_login")]
            public DateTime LastLogin { get; set; }
            [JsonProperty("is_superuser")]
            public bool IsSuperuser { get; set; }
            [JsonProperty("username")]
            public string Username { get; set; }
            [JsonProperty("first_name")]
            public string FirstName { get; set; }
            [JsonProperty("last_name")]
            public string LastName { get; set; }
            [JsonProperty("email")]
            public string Email { get; set; }
            [JsonProperty("is_staff")]
            public bool IsStaff { get; set; }
            [JsonProperty("is_active")]
            public bool IsActive { get; set; }
            [JsonProperty("date_joined")]
            public DateTime DateJoined { get; set; }
            [JsonProperty("patronymic")]
            public string Patronymic { get; set; }
            [JsonProperty("use_english")]
            public bool UseEnglish { get; set; }
            [JsonProperty("anonymous")]
            public bool Anonymous { get; set; }
            [JsonProperty("right_answers")]
            public int RightAnswers { get; set; }
            [JsonProperty("wrong_answers")]
            public int WrongAnswers { get; set; }
            [JsonProperty("groups")]
            public List<object> Groups { get; set; }
            [JsonProperty("user_permissions")]
            public List<object> UserPermissions { get; set; }
        }
    }
}
