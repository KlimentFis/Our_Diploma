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
        private readonly string usersUrl = $"{Our_addres}/api/all_users/";
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string username = LoginEntry.Text;
                string password = PasswordEntry.Text;

                // Check if username or password fields are empty
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    await DisplayAlert("Ошибка", "Введите логин и пароль", "OK");
                    return;
                }

 
                Application.Current.Properties["Username"] = username;
                Application.Current.Properties["Password"] = password;

                using (HttpClient client = new HttpClient())
                {
                    // Fetch the list of all users
                    HttpResponseMessage usersResponse = await client.GetAsync(usersUrl);
                    if (usersResponse.IsSuccessStatusCode)
                    {
                        string usersContent = await usersResponse.Content.ReadAsStringAsync();
                        var usersList = JsonConvert.DeserializeObject<string[]>(usersContent);

                        // Check if the entered username exists in the list
                        if (Array.Exists(usersList, user => user == username))
                        {
                            string authUrl = "http://test.bipchik.keenetic.pro/api/token/";
                            string jsonData = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";
                            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                            HttpResponseMessage authResponse = await client.PostAsync(authUrl, content);

                            if (authResponse.IsSuccessStatusCode)
                            {
                                string responseContent = await authResponse.Content.ReadAsStringAsync();
                                var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

                                string refreshToken = responseObject.refresh;
                                string accessToken = responseObject.access;

                                Application.Current.Properties["RefreshToken"] = refreshToken;
                                Application.Current.Properties["AccessToken"] = accessToken;
                                await Application.Current.SavePropertiesAsync();

                                await DisplayAlert("Успех", "Вы успешно вошли", "OK");
                                //await Navigation.PushAsync(new ProfilePage());
                            }
                            else
                            {
                                await DisplayAlert("Ошибка", $"Ошибка при выполнении запроса: {authResponse.StatusCode}", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", "Неправильный логин или пароль", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Неправильный логин или пароль", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Ошибка: {ex.Message}", "OK");
            }
        }


        private async void SignUpPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }
    }
}
