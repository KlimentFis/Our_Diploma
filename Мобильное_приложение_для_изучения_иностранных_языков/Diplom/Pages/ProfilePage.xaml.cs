using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media.Abstractions;
using Plugin.Media;
using Newtonsoft.Json;
using static Diplom.Pages.UsersPage;
using System.Net.Http;


namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        readonly string apiUrl = "http://192.168.1.16:8888/api/users/";
        public ProfilePage()
        {
            InitializeComponent();
        }

        [Obsolete]
        private async void OnFramePhoto(object sender, EventArgs e)
        {
            var photoStream = await PickPhotoAsync();
            if (photoStream != null)
            {

                // Отобразите выбранное изображение в элементе Image
                Photo.Source = ImageSource.FromStream(() => photoStream);
            }
        }

        private async Task<Stream> PickPhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                _ = DisplayAlert("Ошибка", $"Выбор фотографий не поддерживается на устройстве", "OK");
                return null;
            }

            var options = new PickMediaOptions
            {
                // Размер выбранного изображения
                PhotoSize = PhotoSize.Custom,
                CustomPhotoSize = 10, // Укажите желаемый размер в процентах от оригинального размера
                RotateImage = false,
                MaxWidthHeight = 2000, // Максимальные размеры изображения (не обязательно)
                SaveMetaData = true, // Сохранение метаданных (не обязательно)
                CompressionQuality = 92 // Качество сжатия изображения (не обязательно)
            };

            var file = await CrossMedia.Current.PickPhotoAsync(options);

            if (file == null)
                return null;

            return file.GetStream();
        }
        private async void DeleteBtn_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Внимание!", "Вы уверены, что хотите удалить аккаунт?", "Да", "Нет");

            if (answer)
            {
                // Удаление аккаунта
            }
        }
        private void SaveBtn_Clicked(Object sender, EventArgs e)
        {
            _ = DisplayAlert("Сохранение", $"Данные успешно сохранены", "OK");
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            MyUser user = await GetUserDataFromAPI();
            if (user != null)
            {
                UserNameLabel.Text = user.Username;
            }
        }

        private async Task<MyUser> GetUserDataFromAPI()
        {
            try
            {
                string accessToken = Application.Current.Properties["AccessToken"].ToString();

                using (HttpClient client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl + "profile/"); // URL для получения профиля текущего пользователя
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        MyUser user = JsonConvert.DeserializeObject<MyUser>(jsonResponse);
                        return user;
                    }
                    else
                    {
                        await DisplayAlert("Ошибка", $"Ошибка: {response.StatusCode}", "OK");
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                await DisplayAlert("Ошибка", $"Ошибка: {e.Message}", "OK");
                return null;
            }
        }

    }
}