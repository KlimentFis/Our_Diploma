using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;
using static Diplom.models;
using static Diplom.config;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        private string _photoFilePath;

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
            string apiUrl = $"{Our_addres}/api/users/{Application.Current.Properties["Username"]}/";

            using (HttpClient client = new HttpClient())
            {
                string accessToken = Application.Current.Properties["AccessToken"].ToString();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<MyUser>(content);

                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        user.Image = $"{Our_addres}" + user.Image;
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
                    AnonimousCheck.IsChecked = user.Anonymous;
                    UseEnglisCheck.IsChecked = user.UseEnglish;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Токен недействителен, обновляем его
                    bool refreshed = await RefreshToken();
                    if (refreshed)
                    {
                        // Повторяем запрос после обновления токена
                        await LoadUserProfile();
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", "Не удалось обновить токен.", "OK");
                        // Отправляем пользователя на страницу авторизации
                        await Navigation.PushAsync(new LoginPage());
                    }
                }
                else
                {
                    await DisplayAlert("Ошибка", "Авторизируйтесь.", "OK");
                }
            }
        }

        private async Task<bool> RefreshToken()
        {
            try
            {
                string apiUrl = $"{Our_addres}/api/token/";

                string username = Application.Current.Properties["Username"].ToString();
                string password = Application.Current.Properties["Password"].ToString();

                var requestData = new
                {
                    username = username,
                    password = password
                };

                using (HttpClient client = new HttpClient())
                {
                    string jsonData = JsonConvert.SerializeObject(requestData);
                    HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string tokenContent = await response.Content.ReadAsStringAsync();
                        var tokenData = JsonConvert.DeserializeObject<dynamic>(tokenContent);

                        Application.Current.Properties["AccessToken"] = tokenData.access;
                        await Application.Current.SavePropertiesAsync();
                        return true;
                    }
                    else
                    {
                        // Обработка ошибки при запросе нового токена
                        string errorContent = await response.Content.ReadAsStringAsync();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка исключения при запросе нового токена
                Console.WriteLine($"Произошла ошибка при обновлении токена: {ex.Message}");
                return false;
            }
        }


        private async void OnFramePhoto(object sender, EventArgs e)
        {
            var photoStream = await PickPhotoAsync();
            if (photoStream != null)
            {
                string directory = Path.Combine(FileSystem.AppDataDirectory, "photos");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                _photoFilePath = Path.Combine(directory, "user_photo.jpg");

                using (var fileStream = File.Open(_photoFilePath, FileMode.Create))
                {
                    await photoStream.CopyToAsync(fileStream);
                }

                // Показываем сообщение для сохранения изменений
                bool saveChanges = await DisplayAlert("Изменения", "Для отображения фото, сохраните изменения.", "Сохранить", "Отмена");
                if (saveChanges)
                {
                    await SaveChanges(); // Вызываем SaveChanges асинхронно без ожидания
                }
            }
        }


        private async Task<Stream> PickPhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Ошибка", "Выбор фотографий не поддерживается на устройстве", "OK");
                return null;
            }

            var options = new PickMediaOptions
            {
                PhotoSize = PhotoSize.Custom
            };

            var file = await CrossMedia.Current.PickPhotoAsync(options);

            return file?.GetStream();
        }

        private async void SaveBtn_Clicked(object sender, EventArgs e)
        {
            await SaveChanges();
        }

        private async Task SaveChanges()
        {
            MyUser user = new MyUser
            {
                Username = Application.Current.Properties["Username"].ToString(),
                Password = Application.Current.Properties["Password"].ToString(),
                FirstName = FirstNameEntry.Text,
                LastName = LastNameEntry.Text,
                Patronymic = PatronomicNameEntry.Text,
                Anonymous = AnonimousCheck.IsChecked,
                UseEnglish = UseEnglisCheck.IsChecked,
            };

            string apiUrl = $"{Our_addres}/api/users/{Application.Current.Properties["Username"]}/";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string accessToken = Application.Current.Properties["AccessToken"].ToString();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(user.Username), "username");
                        content.Add(new StringContent(user.Password), "password");
                        content.Add(new StringContent(user.FirstName), "first_name");
                        content.Add(new StringContent(user.LastName), "last_name");
                        content.Add(new StringContent(user.Patronymic), "patronymic");
                        content.Add(new StringContent(user.Anonymous.ToString()), "anonymous");
                        content.Add(new StringContent(user.UseEnglish.ToString()), "use_english");

                        if (!string.IsNullOrEmpty(_photoFilePath))
                        {
                            var photoContent = new StreamContent(File.OpenRead(_photoFilePath));
                            photoContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                            content.Add(photoContent, "image", Path.GetFileName(_photoFilePath));
                        }

                        HttpResponseMessage response = await client.PutAsync(apiUrl, content);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            var updatedUser = JsonConvert.DeserializeObject<MyUser>(responseContent);

                            if (!string.IsNullOrEmpty(updatedUser.Image))
                            {
                                updatedUser.Image = $"{Our_addres}" + updatedUser.Image;
                                UserPhoto.Source = ImageSource.FromUri(new Uri(updatedUser.Image));
                            }
                            else
                            {
                                UserPhoto.Source = ImageSource.FromFile("DefaultUser.png");
                            }

                            await DisplayAlert("Успех", "Данные успешно отправлены!", "OK");
                        }
                        else
                        {
                            string errorContent = await response.Content.ReadAsStringAsync();
                            await DisplayAlert("Ошибка", $"Ошибка при отправке данных: {response.StatusCode}\n{errorContent}", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка: {ex.Message}", "OK");
            }
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
                string apiUrl = $"{Our_addres}/api/delete_user/";

                string accessToken = Application.Current.Properties["AccessToken"].ToString();

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

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

                        // Создаем новую главную страницу, чтобы открыть страницу авторизации
                        MainPage mainPage = new MainPage();
                        mainPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(LoginPage)));
                        Application.Current.MainPage = mainPage;
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

            // Создаем новую главную страницу, чтобы открыть страницу авторизации
            MainPage mainPage = new MainPage();
            mainPage.Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(LoginPage)));
            Application.Current.MainPage = mainPage;
        }

    }
}
