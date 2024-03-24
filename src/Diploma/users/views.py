from django.contrib.auth import login, get_user_model, logout, authenticate
from django.contrib.auth.decorators import login_required
from django.http import JsonResponse, HttpResponseNotAllowed
from django.shortcuts import render, redirect
from django.utils import timezone
from django.views.decorators.csrf import csrf_protect
from users.models import MyUser
from language_tool_python import LanguageTool
import re

def field_check(text):
    # Паттерн для поиска специальных символов
    pattern = r'[!@#$%^&*()_+{}\[\]:;<>,.?/\\|~`\-="]'
    # Проверяем текст на наличие специальных символов
    if re.search(pattern, text):
        return False
    else:
        return True

def check_auth(request, text):
    if not request.user.is_authenticated:
        context = {
            "error": text
        }
        return render(request, 'users/login_or_register.html', context)
    return None  # Возвращаем None если пользователь авторизован

def userList(request):
    error = check_auth(request, "Необходимо авторизоваться!")
    if error:
        return error
    users = MyUser.objects.filter(is_staff=False)
    context = {
        'users': users
    }
    return render(request, 'users/userList.html', context)

def profile(request):
    error = check_auth(request, "Необходимо авторизоваться!")
    if error:
        return error
    user = request.user
    if request.method == 'GET':
        return render(request, 'users/profile.html', {'user': user})
    else:
        LastName = request.POST.get('LastName', '')
        FirstName = request.POST.get('FirstName', '')
        Patronymic = request.POST.get('Patronymic', '')
        UseEnglish = request.POST.get('UseEnglish', False)
        Anonymous = request.POST.get('Anonymous', False)

        if UseEnglish == 'on':
            UseEnglish = True
        else:
            UseEnglish = False

        if Anonymous == 'on':
            Anonymous = True
        else:
            Anonymous = False

        user.last_name = LastName
        user.first_name = FirstName
        user.patronymic = Patronymic
        user.use_english = UseEnglish
        user.anonymous = Anonymous

        user.save()

        return redirect('profile')


@csrf_protect
def register(request):
    if request.method == 'GET':
        return render(request, 'users/register.html')
    else:
        nik = request.POST.get('Nik', '')  # Получение значения поля "Nik" из POST запроса
        password = request.POST.get('password', '')  # Получение значения поля "password" из POST запроса
        confirm_password = request.POST.get('confirm_password','')  # Получение значения поля "confirm_password" из POST запроса

        User = get_user_model()
        user = User.objects.filter(username=nik).first()

        context = {
            'error': '',
        }

        if user:
            context['error'] = 'Такой пользователь уже существует!'
        elif not (nik and password and confirm_password):
            context['error'] = 'Введите данные!'
        elif password != confirm_password:
            context['error'] = 'Пароли не совпадают!'
        elif len(password) < 8 or len(confirm_password) < 8:
            context['error'] = 'Пароль слишком короткий. Минимум 8 символов.'
        elif len(nik) >= 16:
            context['error'] = 'Логин должен быть не длиннее 15 симв.'
        elif not (field_check(password) and field_check(confirm_password) and field_check(nik)):
            context['error'] = 'Можно использовать только буквы и цифры.'
        else:
            # Создание нового пользователя с текущей датой присоединения
            user = User.objects.create_user(username=nik, password=password, is_active=True, data_joined=timezone.now())
            user.date_joined = timezone.now()  # Установка даты присоединения
            user.save()
            login(request, user)  # Вход пользователя в систему
            # Перенаправление на другую страницу при необходимости
            return redirect('index')

        return render(request, 'users/register.html', context)

def user_logout(request):
    logout(request)
    return redirect('login_or_register')

def upload_image(request):
    if request.method == 'POST' and request.FILES:
        image_file = request.FILES['image']
        # Сохраняем изображение на сервере, например, в базе данных или файловой системе
        # Пример для сохранения в модели пользователя
        request.user.image = image_file
        request.user.save()
        return JsonResponse({'image_url': request.user.image.url})
    return JsonResponse({'error': 'No image provided'}, status=400)

@csrf_protect
def user_login(request):
    if request.method == 'GET':
        return render(request, 'users/login.html')
    else:
        nik = request.POST.get('Nik', '')  # Получаем значение поля "Nik" из POST запроса
        password = request.POST.get('password', '')  # Получаем значение поля "password" из POST запроса

        context = {
            'error': '',
        }
        if not (nik and password):
            context['error'] = 'Введите данные!'
        elif not password:
            context['error'] = 'Введите пароль!'
        elif not nik:
            context['error'] = 'Введите логин!'
        else:
            user = authenticate(request, username=nik, password=password)
            if user is not None:
                login(request, user)
                # Редиректим на другую страницу, если необходимо
                return redirect('index')
            else:
                context['error'] = 'Ошибка аутентификации'

        return render(request, 'users/login.html', context)

@csrf_protect
@login_required
def delete_accaunt(request):
    if request.method == 'POST':
        # Удаляем пользователя из базы данных
        request.user.delete()
        # Выход пользователя из системы
        logout(request)
        return redirect('users/login_or_register')
    else:
        # Если метод запроса не POST, вернуть ошибку метода не разрешен
        return HttpResponseNotAllowed(['POST'])

def login_or_register(request):
    if request.user.is_authenticated:
        return render(request, 'tests/index.html')
    else:
        return render(request, 'users/login_or_register.html')