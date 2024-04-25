﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Diplom.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EducationalMaterialsPage : ContentPage
    {
        public EducationalMaterialsPage()
        {
            InitializeComponent();
        }

        [Obsolete]
        private void OnFrameEnglex(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://englex.ru/articles/"));
        }
        [Obsolete]
        private void OnFrameStudy(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://www.study.ru/courses"));
        }
        [Obsolete]
        private void OnFrameBistroenglish(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://www.bistroenglish.com/"));
        }
        [Obsolete]
        private void OnFrameDuolingo(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://ru.duolingo.com/"));
        }
        [Obsolete]
        private void OnFrameMemrise(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://www.memrise.com/ru/"));
        }
        [Obsolete]
        private void OnFrameEngblog(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://engblog.ru/grammar"));
        }

        [Obsolete]
        private void OnFrameLET(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://www.learn-english-today.com/lessons/lessons_list.html"));
        }
    }
}