using System;
using System.Collections.Generic;
using System.Text;
using static Diplom.models;
using Xamarin.Forms;

namespace Diplom
{
    public class UserTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AnonymousTemplate { get; set; }
        public DataTemplate DetailedTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var user = item as MyUser;
            return user.Anonymous ? AnonymousTemplate : DetailedTemplate;
        }
    }
}
