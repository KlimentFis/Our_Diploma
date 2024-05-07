using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public static string AccessToken { get; set; }
        public static string RefreshToken { get; set; }

        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
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

                        // Распарсить responseContent в объект
                        var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

                        // Получить значения токенов из объекта
                        string refreshToken = responseObject.refresh;
                        string accessToken = responseObject.access;

                        // Сохранить токены в вашем приложении
                        Diplom.Pages.LoginPage.RefreshToken = refreshToken;
                        Diplom.Pages.LoginPage.AccessToken = accessToken;

                        // Перейти на другую страницу или выполнить другие действия после успешного входа
                        await DisplayAlert("Успех", "Вы успешно вошли", "OK");
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
