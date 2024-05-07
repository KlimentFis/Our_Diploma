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
    public partial class TestsTranslation : ContentPage
    {
        private RadioButton lastCheckedRadioButton; // Хранит ссылку на последнюю выбранную радиокнопку

        public TestsTranslation()
        {
            InitializeComponent();

            // При нажатии на фрейм срабатывает RadioButton
            Frame1.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnFrameTapped(RadioBtn1))
            });

            Frame2.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnFrameTapped(RadioBtn2))
            });

            Frame3.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => OnFrameTapped(RadioBtn3))
            });
        }

        private void OnFrameTapped(RadioButton radioButton)
        {
            if (radioButton == lastCheckedRadioButton) // Если выбрана уже выбранная радиокнопка
            {
                radioButton.IsChecked = false; // Снимаем отметку
                lastCheckedRadioButton = null; // Сбрасываем ссылку на последнюю выбранную радиокнопку
            }
            else
            {
                if (lastCheckedRadioButton != null)
                    lastCheckedRadioButton.IsChecked = false; // Снимаем отметку с последней выбранной радиокнопки
                radioButton.IsChecked = true; // Устанавливаем отметку на выбранную радиокнопку
                lastCheckedRadioButton = radioButton; // Обновляем ссылку на последнюю выбранную радиокнопку
            }
        }
    }
}
