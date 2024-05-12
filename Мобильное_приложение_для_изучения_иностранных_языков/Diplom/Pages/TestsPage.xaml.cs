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

        [Obsolete]
        private async void OnFrameWords(object sender, EventArgs e)
        {

 //           if (!IsUserLoggedIn())
 //           {
 //               await Navigation.PushAsync(new LoginPage());
 //               return;
 //           }
 //           else
 //           {
                await Navigation.PushAsync(new TestsWords());
 //           }
        }

        [Obsolete]
        private async void OnFrameTranslation(object sender, EventArgs e)
        {
 //           if (!IsUserLoggedIn())
 //           {
 //               await Navigation.PushAsync(new LoginPage());
 //               return;
 //           }
 //           else
 //           {
                await Navigation.PushAsync(new TestsTranslation());
                return;
 //           }
        }

        [Obsolete]
        private async void OnFrameText(object sender, EventArgs e)
        {
 //           if (!IsUserLoggedIn())
 //           {
 //               await Navigation.PushAsync(new LoginPage());
 //               return;
 //           }
 //           else
 //           {
                await Navigation.PushAsync(new TestsText());
 //           }
        }

        private bool IsUserLoggedIn()
        {
            //здесь логика проверки
            return false;
        }
    }
}
