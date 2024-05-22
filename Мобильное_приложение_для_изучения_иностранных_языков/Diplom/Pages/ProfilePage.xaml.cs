using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;
using static Diplom.models;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            CheckAccessToken();
        }

        private async void CheckAccessToken()
        {
            if (Application.Current.Properties.ContainsKey("AccessToken") &&
                Application.Current.Properties["AccessToken"] != null)
            {
                string accessToken = Application.Current.Properties["AccessToken"].ToString();

                bool isValidToken = await VerifyToken(accessToken);

                if (isValidToken)
                {
                    await LoadUserProfile();
                }
                else
                {
                    await DisplayAlert("Сессия истекла", "Ваша сессия истекла. Пожалуйста, войдите снова.", "OK");
                    Application.Current.Properties["AccessToken"] = null;
                    await Navigation.PushAsync(new LoginPage());
                }
            }
            else
            {
                await Navigation.PushAsync(new LoginPage());
            }
        }

        private async Task<bool> VerifyToken(string token)
        {
            
            await Task.Delay(500); 
            return true; 
        }

        private async Task LoadUserProfile()
        {
            string apiUrl = $"http://test.bipchik.keenetic.pro/api/users/{Application.Current.Properties["Username"]}/";

            using (HttpClient client = new HttpClient())
            {
                string accessToken = Application.Current.Properties["AccessToken"].ToString();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<MyUser>(content);

                    
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        user.Image = "http://test.bipchik.keenetic.pro" + user.Image; 
                        UserPhoto.Source = ImageSource.FromUri(new Uri(user.Image)); 
                    }
                    else
                    {
                        
                        UserPhoto.Source = ImageSource.FromFile("DefaultUser.png");
                    }

                    
                    UsernameLabel.Text = user.Username;
                    LastNameEntry.Text = user.LastName;
                    FirstNameEntry.Text = user.FirstName;
                    PatronomicNameEntry.Text = user.Patronymic;
                    AnonimousNameEntry.IsChecked = user.Anonymous;
                    UseEnglisNamehEntry.IsChecked = user.UseEnglish;
                }
                else
                {
                    await DisplayAlert("Ошибка", "Авторизируйтесь.", "OK");
                }
            }
        }

        private async void OnFramePhoto(object sender, EventArgs e)
        {
            var photoStream = await PickPhotoAsync();
            if (photoStream != null)
            {
                UserPhoto.Source = ImageSource.FromStream(() => photoStream);
            }
        }

        private async Task<Stream> PickPhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Ошибка", $"Выбор фотографий не поддерживается на устройстве", "OK");
                return null;
            }

            var options = new PickMediaOptions
            {
                PhotoSize = PhotoSize.Custom,
                CustomPhotoSize = 10,
                RotateImage = false,
                MaxWidthHeight = 2000,
                SaveMetaData = true,
                CompressionQuality = 92
            };

            var file = await CrossMedia.Current.PickPhotoAsync(options);

            return file?.GetStream();
        }

        private async void DeleteBtn_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Внимание!", "Вы уверены, что хотите удалить аккаунт?", "Да", "Нет");

            if (answer)
            {
                await DeleteAccount();
            }
        }

        private async Task DeleteAccount()
        {
            try
            {
                string apiUrl = "http://test.bipchik.keenetic.pro/api/delete_user/";
                string accessToken = Application.Current.Properties["AccessToken"].ToString();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    var deleteData = new
                    {
                        username = Application.Current.Properties["Username"].ToString(),
                        password = Application.Current.Properties["Password"].ToString()
                    };

                    string jsonData = JsonConvert.SerializeObject(deleteData);
                    HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                        await DisplayAlert("Успешно", result.detail.ToString(), "OK");

                        Application.Current.Properties["RefreshToken"] = null;
                        Application.Current.Properties["AccessToken"] = null;
                        await Application.Current.SavePropertiesAsync();
                        await Navigation.PushAsync(new LoginPage());
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        await DisplayAlert("Ошибка", $"Ошибка при удалении аккаунта: {response.StatusCode}\n{errorContent}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
        }

        private async void ExitBtn_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties["RefreshToken"] = null;
            Application.Current.Properties["AccessToken"] = null;
            await Application.Current.SavePropertiesAsync();
            await Navigation.PushAsync(new LoginPage());
        }

        private async void SaveBtn_Clicked(object sender, EventArgs e)
        {
            MyUser user = new MyUser
            {
                Username = Application.Current.Properties["Username"].ToString(),
                Password = Application.Current.Properties["Password"].ToString(),
                FirstName = FirstNameEntry.Text,
                LastName = LastNameEntry.Text,
                Patronymic = PatronomicNameEntry.Text,
                Anonymous = AnonimousNameEntry.IsChecked,
                UseEnglish = UseEnglisNamehEntry.IsChecked,
            };

  
            string json = JsonConvert.SerializeObject(user);

            
            Console.WriteLine("Отправляемые данные: " + json);

            
            string apiUrl = $"http://test.bipchik.keenetic.pro/api/users/{Application.Current.Properties["Username"]}/";

            try
            {
                
                using (HttpClient client = new HttpClient())
                {
                    
                    string accessToken = Application.Current.Properties["AccessToken"].ToString();
                    
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    
                    HttpResponseMessage response = await client.PutAsync(apiUrl, content);

                    
                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Успех", "Данные успешно отправлены!", "OK");
                        Console.WriteLine("Данные успешно отправлены!");
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        await DisplayAlert("Ошибка", $"Ошибка при отправке данных: {response.StatusCode}\n{errorContent}", "OK");
                        Console.WriteLine($"Ошибка при отправке данных: {response.StatusCode}\n{errorContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
