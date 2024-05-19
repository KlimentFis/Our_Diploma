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
                    user.Image = "http://test.bipchik.keenetic.pro" + user.Image; // Full image URL

                    // Bind the data to the UI elements
                    UsernameLabel.Text = user.Username;
                    LastNameEntry.Text = user.LastName;
                    FirstNameEntry.Text = user.FirstName;
                    PatronomicNameEntry.Text = user.Patronymic;
                    AnonimousEntry.IsChecked = user.Anonymous;
                    UseEnglishEntry.IsChecked = user.UseEnglish;
                    UserPhoto.Source = ImageSource.FromUri(new Uri(user.Image)); // Set the image source for the Image element
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

        private void SaveBtn_Clicked(object sender, EventArgs e)
        {
            DisplayAlert("Сохранение", $"Данные успешно сохранены", "OK");
        }
    }
}
