using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        private const string url = "http://test.bipchik.keenetic.pro/api/create_user/";

        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            string username = LoginTxt.Text;
            string password = PassTxt.Text;
            string confirmPassword = ConfPassTxt.Text;

            // Получаем текущую дату и время
            string lastlogin = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            // Добавляем дату и время регистрации в JSON
            string jsonData = $"{{\"username\":\"{username}\",\"password\":\"{password}\",\"last_login\":\"{lastlogin}\",\"data_joined\":\"{lastlogin}\"}}";

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Ошибка", "Введите логин и пароль", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Ошибка", "Пароли не совпадают", "OK");
                return;
            }

            try
            {
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
    }
}
