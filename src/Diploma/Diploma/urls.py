"""Diploma URL Configuration

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/3.2/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  path('', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  path('', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.urls import include, path
    2. Add a URL to urlpatterns:  path('blog/', include('blog.urls'))
"""
from django.contrib import admin
from django.urls import path, include
from django.conf.urls.static import static
from django.conf import settings
from django.contrib.auth import views as auth_views
from LanguageTest.views import index, about_us, tests, links, about_us, download_app
from users.views import userList, login_or_register

urlpatterns = [
    path('', login_or_register, name='index'),
    path('index/', index, name='start'),
    path('admin/', admin.site.urls),
    path('tests/', include('LanguageTest.urls'), name='tests'),
    path('usersList/', userList, name='UserList'),
    path('user/', include('users.urls')),
    path('links/', links, name='links'),
    path('about_us/', about_us, name='about_us'),
    path('download_app/', download_app, name="download_app")
]

if settings.DEBUG:
    urlpatterns += static(settings.STATIC_URL, document_root=settings.MEDIA_ROOT)
    urlpatterns += static(settings.MEDIA_URL, document_root=settings.MEDIA_ROOT)