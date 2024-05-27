using Xamarin.Forms;
using System.Linq;
using System.Collections.Generic;
using static Diplom.models;

namespace Diplom
{
    public class UserTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AnonymousTemplate { get; set; }
        public DataTemplate DetailedTemplate { get; set; }
        public DataTemplate SpecialFrameTemplate1 { get; set; }
        public DataTemplate SpecialFrameTemplate2 { get; set; }
        public DataTemplate SpecialFrameTemplate3 { get; set; }
        public DataTemplate AnonymousSpecialFrameTemplate1 { get; set; }
        public DataTemplate AnonymousSpecialFrameTemplate2 { get; set; }
        public DataTemplate AnonymousSpecialFrameTemplate3 { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var usersPage = container as ListView;
            var user = item as MyUser;
            var users = usersPage.ItemsSource as IList<MyUser>;
            var index = users.IndexOf(user);

            if (index == 0)
            {
                return user.Anonymous ? SpecialFrameTemplate1 : AnonymousSpecialFrameTemplate1;
            }
            else if (index == 1)
            {
                return user.Anonymous ? SpecialFrameTemplate2 : AnonymousSpecialFrameTemplate2;
            }
            else if (index == 2)
            {
                return user.Anonymous ? SpecialFrameTemplate3 : AnonymousSpecialFrameTemplate3;
            }

            return user.Anonymous ? AnonymousTemplate : DetailedTemplate;
        }
    }
}