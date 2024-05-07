using System;
using System.Net.Http;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Diplom.Pages
{
    public partial class SignUpPage : ContentPage
    {
        public static string AccessToken { get; set; }
        public static string RefreshToken { get; set; }
        private const string url = "http://192.168.1.16:8888/api/create_user/";

        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            string username = LoginTxt.Text;
            string password = PassTxt.Text;
            string confirmPassword = ConfPassTxt.Text;

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

            var httpClient = new HttpClient();

            var requestData = new
            {
                Username = username,
                Password = password
            };

            var jsonRequest = JsonConvert.SerializeObject(requestData);
            var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostAsync(url, content);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", "Не удалось выполнить запрос: " + ex.Message, "OK");
                return;
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Распарсить jsonResponse в объект
                var responseObject = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                // Получить значения токенов из объекта
                string refreshToken = responseObject.refresh;
                string accessToken = responseObject.access;

                // Сохранить токены в вашем приложении
                Diplom.Pages.SignUpPage.RefreshToken = refreshToken;
                Diplom.Pages.SignUpPage.AccessToken = accessToken;

                // Отобразить успешное сообщение
                await DisplayAlert("Успех", "Пользователь успешно зарегистрирован", "OK");
            }
            else
            {
                await DisplayAlert("Ошибка", "Не удалось зарегистрировать пользователя. Код ошибки: " + response.StatusCode, "OK");
            }
        }
    }
}
