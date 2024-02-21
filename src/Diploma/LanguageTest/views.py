from random import randint, sample, shuffle
from django.http import HttpResponseNotAllowed, HttpResponseForbidden
from django.views.decorators.csrf import csrf_protect
from django.contrib.auth import logout
from django.contrib.auth import authenticate
from django.contrib.auth.decorators import login_required
from django.contrib.auth import get_user_model, login
from django.utils import timezone
from django.shortcuts import render, redirect
from users.models import MyUser
from words.models import Word
from words.models import Suggestion

# Create your views here.



def index(request):
    return render(request, 'index.html')

@login_required
def tests(request):
    return render(request, 'tests.html')

@login_required
def links(request):
    return render(request, 'links.html')

@login_required
def about_us(request):
    return render(request, 'about_as.html')

@login_required
def profile(request):
    user = request.user

    if request.method == 'GET':
        return render(request, 'profile.html', {'user': user})
    else:
        LastName = request.POST.get('LastName', '')
        FirstName = request.POST.get('FirstName', '')
        Patronymic = request.POST.get('Patronymic', '')
        UseEnglish = request.POST.get('UseEnglish', False)

        if UseEnglish == 'on':
            UseEnglish = True
        else:
            UseEnglish = False

        user.last_name = LastName
        user.first_name = FirstName
        user.patronymic = Patronymic
        user.use_english = UseEnglish

        user.save()

        return redirect('profile')

@csrf_protect
def register(request):
    if request.method == 'GET':
        return render(request, 'register.html')
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
        elif not (password.isalnum() and confirm_password.isalnum() and nik.isalnum()):
            context['error'] = 'Можно использовать только буквы и цифры.'
        else:
            # Создание нового пользователя с текущей датой присоединения
            user = User.objects.create_user(username=nik, password=password, is_active=True, data_joined=timezone.now())
            user.date_joined = timezone.now()  # Установка даты присоединения
            user.save()
            login(request, user)  # Вход пользователя в систему
            # Перенаправление на другую страницу при необходимости
            return redirect('index')

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
                return redirect('index')
            else:
                context['error'] = 'Ошибка аутентификации'

        return render(request, 'login.html', context)

@login_required
def letter_verification(request):
    return render(request, 'LetterVerification.html')

@csrf_protect
@login_required
def check_word(request):
    if 'random_word_id' not in request.session:
        request.session['random_word_id'] = randint(1, Word.objects.count())

    random_word_id = request.session['random_word_id']
    random_word = Word.objects.get(pk=random_word_id)

    # Select other random words
    random_words = sample(list(Word.objects.exclude(pk=random_word_id)), 2)

    # Append the random word
    random_words.append(random_word)

    # Shuffle the list of words
    shuffle(random_words)

    context = {
        'right': random_word,
        'words': random_words,
        'error': '',
        'properly': '',
        'proposal': f'{random_word.translate}'
    }

    if request.method == 'POST':
        selected_word = request.POST.get('exampleRadios')
        if selected_word == random_word.name:
            context['properly'] = 'Вы молодец!'
        else:
            context['error'] = f'Правильное слово было: {random_word.name}'

        # Choose a new random word and update it in the session
        request.session['random_word_id'] = randint(1, Word.objects.count())

        random_word_id = request.session['random_word_id']
        random_word = Word.objects.get(pk=random_word_id)

        # Select new random words
        random_words = sample(list(Word.objects.exclude(pk=random_word_id)), 2)

        # Append the random word
        random_words.append(random_word)

        # Shuffle the list of words
        shuffle(random_words)

        context['right'] = random_word
        context['proposal'] = random_word.translate
        context['words'] = random_words  # Update the list of words

    return render(request, 'selectWord.html', context)

@csrf_protect
@login_required
def check_suggestion(request):



    context = {
        'right': '',
        'suggestion': 'Test',
        'words': '',
        'error': '',
        'properly': '',
    }

    return render(request, 'insertWord.html', context)


@login_required
def userList(request):
    users = MyUser.objects.filter(is_staff=False).exclude(username=request.user.username)
    context = {
        'users': users
    }
    return render(request, 'userList.html', context)

@login_required
def translateWord(request):
    return render(request, 'translateWord.html')

def login_or_register(request):
    if request.user.is_authenticated:
        return render(request, 'index.html')
    else:
        return render(request, 'login_or_register.html')

def user_logout(request):
    logout(request)
    return redirect('login_or_register')

@csrf_protect
@login_required
def delete_accaunt(request):
    if request.method == 'POST':
        # Удаляем пользователя из базы данных
        request.user.delete()
        # Выход пользователя из системы
        logout(request)
        return redirect('login_or_register')
    else:
        # Если метод запроса не POST, вернуть ошибку метода не разрешен
        return HttpResponseNotAllowed(['POST'])
