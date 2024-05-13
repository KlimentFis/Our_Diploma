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
                // Проверяем, выбран ли пункт меню "Профиль" и авторизован ли пользователь
                if (item.TargetPage == typeof(ProfilePage) || item.TargetPage == typeof(UsersPage))
                {
                    if (!IsUserLoggedIn())
                    {
                        // Если пользователь не авторизован, открываем страницу авторизации
                        Detail = new NavigationPage((Page)Activator.CreateInstance(typeof(LoginPage)));
                        flyoutMenu.listviewMenu.SelectedItem = null;
                        IsPresented = false;
                        return;
                    }
                }

                // Открываем страницу, выбранную из меню
                Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetPage));
                flyoutMenu.listviewMenu.SelectedItem = null;
                IsPresented = false;
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
