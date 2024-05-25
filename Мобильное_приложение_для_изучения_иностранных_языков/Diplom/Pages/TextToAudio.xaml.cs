using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Diplom.Pages
{
    public partial class TextToAudio : ContentPage
    {
        public TextToAudio()
        {
            InitializeComponent();
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            var text = EditorTxt.Text;
            if (!string.IsNullOrEmpty(text))
            {
                await TextToSpeech.SpeakAsync(text);
            }
        }
    }
}
