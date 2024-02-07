from django.shortcuts import render
from django.views.decorators.csrf import csrf_protect


# Create your views here.
from src.Diploma.LanguageTest.models import MyUser


def index(request):
    return render(request, 'main.html')

# @login_required
def tests(request):
    return render(request, 'main.html')

# @login_required
def links(request):
    return render(request, 'links.html')

# @login_required
def about_us(request):
    return render(request, 'about_as.html')

# @login_required
def profile(request):
    return render(request, 'profile.html')

def register(request):
    if request.method == 'GET':
        return render(request, 'register.html')
    else:
        nik = request.POST.get('Nik', '')  # Получаем значение поля "Nik" из POST запроса
        password = request.POST.get('password', '')  # Получаем значение поля "password" из POST запроса
        confirm_password = request.POST.get('confirm_password', '')  # Получаем значение поля "confirm_password" из POST запроса

        context = {
            'error': '',
        }

        if not (nik and password and confirm_password):
            # Если одно из полей пустое, выполните соответствующие действия
            # Например, выведите сообщение об ошибке или выполните другую логику

            # Вернуть ответ HTTP, например:

            return render(request, 'profile.html', *context)
        else:
            ...



@csrf_protect
def login(request):
    if request.method == 'GET':
        return render(request, 'login.html')
    else:
        ...

# @login_required
def letter_verification(request):
    return render(request, 'LetterVerification.html')

# @login_required
def check_word(request):
    return render(request, 'selectWord.html')

# @login_required
def userList(request):
    return render(request, 'userList.html')

# @login_required
def translateWord(request):
    return render(request, 'translateWord.html')

def login_or_register(request):
    return render(request, 'login_or_register.html')