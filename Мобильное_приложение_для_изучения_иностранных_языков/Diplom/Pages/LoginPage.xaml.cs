using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Diplom.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void Login(object sender, EventArgs e)
        {
            try
            {
                string url = "http://192.168.1.16:8888/api/token/";
                string username = LoginEntry.Text;
                string password = PasswordEntry.Text;
                string jsonData = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";

                using (HttpClient client = new HttpClient())
                {
                    HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        // Обработка JSON-ответа
                        // Например, десериализация JSON в объект или вывод пользователю
                        Console.WriteLine(responseContent);
                    }
                    else
                    {
                        // Вывод сообщения об ошибке в случае неудачного запроса
                        await DisplayAlert("Ошибка", $"Ошибка при выполнении запроса: {response.StatusCode}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                // Вывод сообщения об ошибке в случае исключения
                await DisplayAlert("Ошибка", $"Ошибка: {ex.Message}", "OK");
            }
        }

        private async void SignUpPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }
    }
}
