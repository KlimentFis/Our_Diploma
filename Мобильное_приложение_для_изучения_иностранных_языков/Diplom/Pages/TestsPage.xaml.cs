using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestsPage : ContentPage
    {
        public TestsPage()
        {
            InitializeComponent();
        }


        private async void OnFrameWords(object sender, EventArgs e)
        {
            if (!IsUserLoggedIn())
            {
                await Navigation.PushAsync(new LoginPage());
                return;
            }
            else
            {
                await Navigation.PushAsync(new TestsWords());
            }
        }

        private async void OnFrameTranslation(object sender, EventArgs e)
        {
            if (!IsUserLoggedIn())
            {
                await Navigation.PushAsync(new LoginPage());
                return;
            }
            else
            {
                await Navigation.PushAsync(new TestsTranslation());
                return;
            }
        }

        private async void OnFrameText(object sender, EventArgs e)
        {
            if (!IsUserLoggedIn())
            {
                await Navigation.PushAsync(new LoginPage());
                return;
            }
            else
            {
                await Navigation.PushAsync(new TestsText());
            }
        }

        private bool IsUserLoggedIn()
        {
            // Проверка наличия токена в хранилище
            if (Application.Current.Properties.ContainsKey("AccessToken"))
            {
                // Токен присутствует, пользователь вошел в систему
                return true;
            }
            else
            {
                // Токен отсутствует, пользователь не вошел в систему
                return false;
            }
        }

    }
}
