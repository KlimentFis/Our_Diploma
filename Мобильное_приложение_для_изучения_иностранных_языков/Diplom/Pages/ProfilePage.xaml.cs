﻿using System;
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
        public ProfilePage()
        {
            InitializeComponent();
        }
        [Obsolete]
        public ProfilePage(MyUser user)
        {
            BindingContext = user;

            // Загрузка изображения пользователя
            if (!string.IsNullOrEmpty(user.Image))
            {
                UserPhoto.Source = ImageSource.FromUri(new Uri(user.Image));
            }
            else
            {
                // Если изображение пользователя отсутствует, отображаем стандартное изображение или пустоту
                UserPhoto.Source = "ProfileMenuLight.png"; // Замените "placeholder_image.png" на свой путь к стандартному изображению
            }

            // Привязываем обработчик события к событию нажатия на элемент
            UserPhoto.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnFramePhoto(null, EventArgs.Empty))


            });
        }

        [Obsolete]
        private async void OnFramePhoto(object sender,EventArgs e)
        {
            var photoStream = await PickPhotoAsync();
            if (photoStream != null)
            {
                // Отобразите выбранное изображение в элементе Image
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
    }
}