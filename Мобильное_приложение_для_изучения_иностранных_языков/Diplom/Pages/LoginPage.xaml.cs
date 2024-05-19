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
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string usersUrl = "http://test.bipchik.keenetic.pro/api/all_users/";
                string username = LoginEntry.Text;
                string password = PasswordEntry.Text;

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
                                // await Navigation.PushAsync(new UsersPage());
                            }
                            else
                            {
                                await DisplayAlert("Ошибка", $"Ошибка при выполнении запроса: {authResponse.StatusCode}", "OK");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Ошибка", "Пользователь с таким логином не существует", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Ошибка при получении списка пользователей", "OK");
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
