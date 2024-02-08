from django.views.decorators.csrf import csrf_protect
from django.contrib.auth import logout
from django.contrib.auth import authenticate, login
from django.contrib.auth import get_user_model
from django.shortcuts import redirect
from django.contrib.auth.models import User
from django.shortcuts import render
from django.contrib.auth.decorators import login_required

# Create your views here.
def index(request):
    return render(request, 'main.html')

@login_required
def tests(request):
    return render(request, 'main.html')

@login_required
def links(request):
    return render(request, 'links.html')

@login_required
def about_us(request):
    return render(request, 'about_as.html')

@login_required
def profile(request):
    return render(request, 'profile.html')

@csrf_protect
def register(request):
    if request.method == 'GET':
        return render(request, 'register.html')
    else:
        nik = request.POST.get('Nik', '')  # Получаем значение поля "Nik" из POST запроса
        password = request.POST.get('password', '')  # Получаем значение поля "password" из POST запроса
        confirm_password = request.POST.get('confirm_password', '')  # Получаем значение поля "confirm_password" из POST запроса

        User = get_user_model()
        user = User.objects.filter(username=nik).first()

        context = {
            'error': '',
        }
        if user:
            context['error'] = 'Такой пользователь уже есть!'
        elif not (password and confirm_password):
            context['error'] = 'Пароли не совпадают!'
        elif not (nik and password and confirm_password):
            context['error'] = 'Введите данные!'
        else:
            # Создаем нового пользователя
            user = User.objects.create_user(username=nik, password=password)
            # Аутентифицируем пользователя и выполняем вход
            user = authenticate(request, username=nik, password=password)
            if user is not None:
                login(request, user)
                # Редиректим на другую страницу, если необходимо
                return redirect('tests')
            else:
                context['error'] = 'Ошибка аутентификации'

        return render(request, 'register.html', context)



@csrf_protect
def user_login(request):
    if request.method == 'GET':
        return render(request, 'login.html')
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
                return redirect('tests')
            else:
                context['error'] = 'Ошибка аутентификации'

        return render(request, 'login.html', context)

@login_required
def letter_verification(request):
    return render(request, 'LetterVerification.html')

@login_required
def check_word(request):
    return render(request, 'selectWord.html')

@login_required
def userList(request):
    users = User.objects.filter(is_staff=False)
    context = {
        'users': users
    }
    return render(request, 'userList.html', context)

@login_required
def translateWord(request):
    return render(request, 'translateWord.html')

def login_or_register(request):
    if request.user.is_authenticated:
        return render(request, 'main.html')
    else:
        return render(request, 'login_or_register.html')

def user_logout(request):
    logout(request)
    return redirect('login_or_register')
