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

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var usersPage = container as ListView;
            var user = item as MyUser;
            var users = usersPage.ItemsSource as IList<MyUser>;
            var index = users.IndexOf(user);

            if (index == 0)
            {
                return SpecialFrameTemplate1;
            }
            else if (index == 1)
            {
                return SpecialFrameTemplate2;
            }
            else if (index == 2)
            {
                return SpecialFrameTemplate3;
            }
            else
            {
                // Assuming you have logic to distinguish between anonymous and detailed templates
                if (user.Anonymous)
                {
                    return AnonymousTemplate;
                }
                else
                {
                    return DetailedTemplate;
                }
            }
        }
    }
}
