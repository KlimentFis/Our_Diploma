from django.contrib import admin
from django.contrib.auth.admin import UserAdmin
from .models import MyUser

# Регистрация вашей модели с использованием административного класса
admin.site.register(MyUser)