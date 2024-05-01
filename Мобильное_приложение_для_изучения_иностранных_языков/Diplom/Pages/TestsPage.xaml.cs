using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            await Navigation.PushAsync(new TestsWords());
        }
        [Obsolete]
        private async void OnFrameTranslation(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TestsTranslation());
        }
        [Obsolete]
        private async void OnFrameText(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TestsText());
        }
    }
}