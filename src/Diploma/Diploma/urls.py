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
from django.urls import path
from LanguageTest.views import index, links, about_us, profile, register, letter_verification, check_word, login, userList, translateWord, login_or_register, tests

urlpatterns = [
    path('admin/', admin.site.urls),
    # path('', login_or_register, name='index'),
    path('', index, name='index'),
    # path('tests/', tests, name='tests'),
    path('links/', links, name='links'),
    path('about_us/', about_us, name='about_us'),
    path('profile/', profile, name='profile'),
    path('register/', register, name='register'),
    path('letter_verification/', letter_verification, name='letter_verification'),
    path('check_word/', check_word, name='check_word'),
    path('login/', login, name='login'),
    path('usersList/', userList, name='UserList'),
    path('translate/', translateWord, name='translateWord'),
    path('login_or_register/', login_or_register, name='login_or_register'),
]
