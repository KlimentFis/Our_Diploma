using Diplom.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Diplom
{
    public partial class MainPage : FlyoutPage
    {
        public MainPage()
        {
            InitializeComponent();
            flyoutMenu.listviewMenu.ItemSelected += OnSelectedItem;
        }

        private void OnSelectedItem(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is FlyoutItemPage item)
            {
                // Если пользователь не авторизован и выбирает "профиль", открываем страницу авторизации
                if (!IsUserLoggedIn() && item.TargetPage == typeof(ProfilePage))
                {
                    Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(LoginPage)));
                }
                else
                {
                    // Иначе открываем страницу, выбранную из меню
                    Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetPage));
                }

                flyoutMenu.listviewMenu.SelectedItem = null;
                IsPresented = false;
            }
        }
        private bool IsUserLoggedIn()
        {
            //здесь логика проверки
            return true;
        }
    }
}
