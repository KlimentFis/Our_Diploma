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
        string apiUrl = "https://api.exchangeratesapi.io/v1/latest";

        public UsersPage()
        {
            InitializeComponent();

            GetDataFromAPI();
        }

        private async void GetDataFromAPI()
        {
            // Создание HttpClient для отправки запросов
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
            public string username { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
        }
    }
}