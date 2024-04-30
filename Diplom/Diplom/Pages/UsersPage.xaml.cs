using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http;
using Newtonsoft.Json;


namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersPage : ContentPage
    {
        // URL вашего DRF API
        readonly string apiUrl = "http://localhost:8888/api/my_users";

        public UsersPage()
        {
            InitializeComponent();
            GetDataFromAPI();
        }
        private async void GetDataFromAPI()
        {            // Создание HttpClient для отправки запросов
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Отправка GET-запроса и получение ответа
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    // Проверка успешности запроса
                    if (response.IsSuccessStatusCode)
                    {
                        // Чтение ответа в формате JSON
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        // Вывод полученных данных
                        _ = DisplayAlert("JSON Response", jsonResponse, "OK");
                    }
                    else
                    {
                        _ = DisplayAlert("Ошибка", $"Ошибка: {response.StatusCode}", "OK");
                    }
                }
                catch (Exception e)
                {
                    _ = DisplayAlert("Ошибка", $"Ошибка: {e.Message}", "OK");
                }
            }
        }
        public class User
        {
            public string Username { get; set; }
            public string First_name { get; set; }
            public string Last_name { get; set; }
            public string Email { get; set; }
        }
    }
}