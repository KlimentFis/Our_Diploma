using System;
using System.Collections.Generic;
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
    public partial class SignUpPage : ContentPage
    {
        private readonly string createUserUrl = $"{Our_addres}/api/create_user/";
        private readonly string allUsersUrl = $"{Our_addres}/api/all_users/";

        public SignUpPage()
        {
            InitializeComponent();
        }

        private async void SignUpButton_Clicked(object sender, EventArgs e)
        {
            string username = LoginTxt.Text;
            string password = PassTxt.Text;
            Application.Current.Properties["Username"] = username;
            Application.Current.Properties["Password"] = password;
            string confirmPassword = ConfPassTxt.Text;

            // Получаем текущую дату и время
            string lastlogin = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            // Добавляем дату и время регистрации в JSON
            string jsonData = $"{{\"username\":\"{username}\",\"password\":\"{password}\",\"last_login\":\"{lastlogin}\",\"data_joined\":\"{lastlogin}\"}}";

            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Ошибка", "Введите логин и пароль", "OK");
                return;
            }

            // Проверка на существующего пользователя
            var existingUsernames = await FetchAllUsernames();
            if (existingUsernames.Contains(username))
            {
                await DisplayAlert("Ошибка", "Пользователь с таким логином уже существует.", "OK");
                return;
            }

            if (username.Length >= 16)
            {
                errors.Add("- Логин должен быть не длиннее 15 символов.");
            }

            if (!IsAlphanumeric(password) || !IsAlphanumeric(confirmPassword))
            {
                errors.Add("- Можно использовать только буквы и цифры в пароле.");
            }

            if (password.Length < 8)
            {
                errors.Add("- Пароль слишком короткий. Минимум 8 символов.");
            }

            if (password != confirmPassword)
            {
                errors.Add("- Пароли не совпадают");
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
                    HttpResponseMessage response = await client.PostAsync(createUserUrl, content);

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


                        // Создаем новую главную страницу, чтобы открыть страницу профиля
                        MainPage mainPage = new MainPage();
                        mainPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(ProfilePage)));
                        Application.Current.MainPage = mainPage;
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

        // Метод для получения списка всех пользователей
        private async Task<List<string>> FetchAllUsernames()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(allUsersUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var usernames = JsonConvert.DeserializeObject<List<string>>(responseContent);
                        return usernames;
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось получить список пользователей", "OK");
                        return new List<string>();
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка: {ex.Message}", "OK");
                return new List<string>();
            }
        }
    }
}