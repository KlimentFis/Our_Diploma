using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;
using static Diplom.models;
using System.Text;
using System.Reflection;

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
            // Implement your token verification logic here.
            await Task.Delay(500); // Simulate network call
            return true; // Change to actual validation logic
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

                    // Set the Image property for the user
                    if (!string.IsNullOrEmpty(user.Image))
                    {
                        user.Image = "http://test.bipchik.keenetic.pro" + user.Image; // Full image URL
                        UserPhoto.Source = ImageSource.FromUri(new Uri(user.Image)); // Set the image source for the Image element
                    }
                    else
                    {
                        // If user does not have an image, set default image
                        UserPhoto.Source = ImageSource.FromFile("DefaultUser.png");
                    }

                    // Bind the data to the UI elements
                    UsernameLabel.Text = user.Username;
                    LastNameEntry.Text = user.LastName;
                    FirstNameEntry.Text = user.FirstName;
                    PatronomicNameEntry.Text = user.Patronymic;
                    AnonimousNameEntry.IsChecked = user.Anonymous;
                    UseEnglisNamehEntry.IsChecked = user.UseEnglish;
                }
                else
                {
                    await DisplayAlert("Ошибка", "Не удалось загрузить данные профиля.", "OK");
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
                // Удаление аккаунта
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

            // Сериализуем объект в JSON
            string json = JsonConvert.SerializeObject(user);

            // Выводим JSON в консоль для проверки
            Console.WriteLine("Отправляемые данные: " + json);

            // Определяем URL для отправки запроса
            string apiUrl = $"http://test.bipchik.keenetic.pro/api/users/{Application.Current.Properties["Username"]}/";

            try
            {
                // Отправляем запрос методом PUT
                using (HttpClient client = new HttpClient())
                {
                    // Получаем токен доступа
                    string accessToken = Application.Current.Properties["AccessToken"].ToString();
                    // Устанавливаем заголовок Authorization
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    // Создаем HttpContent из JSON
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Отправляем PUT запрос
                    HttpResponseMessage response = await client.PutAsync(apiUrl, content);

                    // Проверяем ответ сервера
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
