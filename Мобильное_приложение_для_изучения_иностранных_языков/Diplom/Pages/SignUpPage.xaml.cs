using System;
using System.Collections.Generic;
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

            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Введите логин и пароль");
            }

            if (password != confirmPassword)
            {
                errors.Add("Пароли не совпадают");
            }

            if (password.Length < 8 || confirmPassword.Length < 8)
            {
                errors.Add("Пароль слишком короткий. Минимум 8 символов.");
            }

            if (username.Length >= 16)
            {
                errors.Add("Логин должен быть не длиннее 15 символов.");
            }

            if (!IsAlphanumeric(password) || !IsAlphanumeric(confirmPassword))
            {
                errors.Add("Можно использовать только буквы и цифры в пароле.");
            }

            if (errors.Count > 0)
            {
                string errorMessage = string.Join("\n", errors);
                await DisplayAlert("Ошибка", errorMessage, "OK");
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

        // Метод для проверки строки на наличие только букв и цифр
        private bool IsAlphanumeric(string str)
        {
            foreach (char c in str)
            {
                if (!char.IsLetterOrDigit(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
