using System;
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
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string url = "http://test.bipchik.keenetic.pro/api/token/";
                //string url = config.Our_addres;
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

                        // Сохранить токены в хранилище приложения
                        Application.Current.Properties["RefreshToken"] = refreshToken;
                        Application.Current.Properties["AccessToken"] = accessToken;
                        await Application.Current.SavePropertiesAsync();

                        // Перейти на другую страницу после успешной аутентификации
                        await DisplayAlert("Успех", "Вы успешно вошли", "OK");
//                        await Navigation.PushAsync(new UsersPage());
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
