from random import randint, sample, shuffle
from django.views.decorators.csrf import csrf_protect, csrf_exempt
from language_tool_python import LanguageTool
from words.models import Word
from words.models import Suggestion
from users.views import check_auth
from django.shortcuts import render
from gtts import gTTS
import re

def index(request):
    return render(request, 'tests/index.html')

def tests(request):
    return render(request, 'tests/tests.html')

def links(request):
    return render(request, 'tests/links.html')

def about_us(request):
    return render(request, 'tests/about_as.html')

@csrf_exempt
def letter_verification(request):
    error = check_auth(request, "Необходимо авторизоваться!")
    if error:
        return error
    if request.method == 'GET':
        return render(request, 'tests/letter_verification.html')
    elif request.method == 'POST':
        tool = LanguageTool('en-US')
        text = request.POST.get('not_checked_text', '')
        errors = tool.check(text)
        if errors:
            context = {'text': text, 'errors': errors}
        else:
            context = {'text': text, 'errors': None}

        print(errors)

        return render(request, 'tests/letter_verification.html', context)

@csrf_protect
def check_word(request):
    error = check_auth(request, "Необходимо авторизоваться!")
    if error:
        return error
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
            request.user.right_answers += 1
            request.user.save()  # Сохранение изменений
            context['properly'] = 'Вы молодец! Правильный ответ!'
        else:
            request.user.wrong_answers += 1
            request.user.save()  # Сохранение изменений
            context['error'] = f'Правильное слово было: {random_word.name}'

        # Выбираем новое случайное слово, не совпадающее с предыдущим
        new_random_word_id = random_word_id
        while new_random_word_id == random_word_id:
            new_random_word_id = randint(1, Word.objects.count())

        request.session['random_word_id'] = new_random_word_id
        random_word = Word.objects.get(pk=new_random_word_id)

        # Select new random words
        random_words = sample(list(Word.objects.exclude(pk=new_random_word_id)), 2)

        # Append the random word
        random_words.append(random_word)

        # Shuffle the list of words
        shuffle(random_words)

        context['right'] = random_word
        context['proposal'] = random_word.translate
        context['words'] = random_words  # Update the list of words

    return render(request, 'tests/select_word.html', context)

@csrf_protect
def check_suggestion(request):
    error = check_auth(request, "Необходимо авторизоваться!")
    if error:
        return error
    if 'random_suggestion_id' not in request.session:
        request.session['random_suggestion_id'] = randint(1, Suggestion.objects.count())

    random_suggestion_id = request.session['random_suggestion_id']
    random_suggestion = Suggestion.objects.get(pk=random_suggestion_id)

    # Получаем правильное слово из поля right_word
    right_word = random_suggestion.right_word

    # Создаем копию предложения для замены правильного слова на многоточие
    replaced_suggestion = random_suggestion.suggestion.replace(right_word, '___')

    # Получаем список из правильного слова и еще двух случайных слов
    words = [right_word]

    # Получаем случайные предложения для вставки
    other_suggestions = Suggestion.objects.exclude(right_word=right_word).order_by('?')[:2]
    for other_suggestion in other_suggestions:
        words.append(other_suggestion.right_word)

    context = {
        'suggestion': replaced_suggestion,
        'words': words,
        'error': '',
        'properly': '',
        'proposal': right_word  # Предполагая, что здесь должен быть перевод
    }

    if request.method == 'POST':
        selected_word = request.POST.get('exampleRadios')
        if selected_word == right_word:
            request.user.right_answers += 1
            request.user.save()  # Сохранение изменений
            context['properly'] = 'Вы молодец! Правильный ответ!'
        else:
            request.user.wrong_answers += 1
            request.user.save()  # Сохранение изменений
            context['error'] = f'Правильное слово было: {right_word}'

        # Выбираем новое случайное слово, не совпадающее с предыдущим
        new_random_suggestion_id = random_suggestion_id
        while new_random_suggestion_id == random_suggestion_id:
            new_random_suggestion_id = randint(1, Suggestion.objects.count())

        request.session['random_suggestion_id'] = new_random_suggestion_id

        # Обновляем информацию о случайном предложении
        random_suggestion = Suggestion.objects.get(pk=new_random_suggestion_id)
        right_word = random_suggestion.right_word
        replaced_suggestion = random_suggestion.suggestion.replace(right_word, '____')
        words = [right_word]
        other_suggestions = Suggestion.objects.exclude(right_word=right_word).order_by('?')[:2]
        for other_suggestion in other_suggestions:
            words.append(other_suggestion.right_word)

        context['suggestion'] = replaced_suggestion
        context['words'] = words
        context['proposal'] = right_word  # Предполагая, что здесь должен быть перевод

    return render(request, 'tests/insertWord.html', context)


def translateWord(request):
    error = check_auth(request, "Необходимо авторизоваться!")
    if error:
        return error
    return render(request, 'tests/translate_word.html')


import os
from django.http import HttpResponse
from django.conf import settings

def download_app(request):
    # Путь к файлу приложения
    app_file_path = os.path.join(settings.STATIC_ROOT, 'Duolingo-Logo-2019-present.jpg')

    # Проверяем существование файла
    if os.path.exists(app_file_path):
        with open(app_file_path, 'rb') as file:
            # Читаем содержимое файла
            file_content = file.read()

        # Определяем тип содержимого для заголовка Content-Type
        content_type = 'image/jpeg'

        # Устанавливаем заголовки для скачивания файла
        response = HttpResponse(file_content, content_type=content_type)
        response['Content-Disposition'] = 'attachment; filename="Duolingo-Logo-2019-present.jpg"'

        return response
    else:
        # Если файл не существует, возвращаем сообщение об ошибке
        return HttpResponse("Файл не найден", status=404)


def text_to_audio(request):
    error = check_auth(request, "Необходимо авторизоваться!")
    if error:
        return error

    audio_url = None  # Инициализация переменной audio_url

    if request.method == 'POST':
        text = request.POST.get('text', '')
        if text:
            # Проверяем, содержит ли текст только символы английского языка и пробелы
            if re.match("^[a-zA-Z ]+$", text):
                # Если текст на английском языке, преобразуем его в аудио
                try:
                    # Проверяем и создаем директорию media, если ее нет
                    media_root = settings.MEDIA_ROOT
                    if not os.path.exists(media_root):
                        os.makedirs(media_root)

                    audio_file_path = os.path.join(media_root, 'output.mp3')
                    tts = gTTS(text=text, lang='en')
                    tts.save(audio_file_path)
                    audio_url = settings.MEDIA_URL + 'output.mp3'
                    return render(request, 'tests/text_to_audio.html', {'audio_url': audio_url, 'error': None})
                except Exception as e:
                    return render(request, 'tests/text_to_audio.html', {'audio_url': None, 'error': str(e)})
            else:
                # Если текст содержит символы, отличные от английских букв и пробелов, выводим ошибку
                error = "Текст должен содержать только английские буквы и пробелы"
        else:
            error = "Введите текст для преобразования в аудио"
    return render(request, 'tests/text_to_audio.html', {'audio_url': audio_url, 'error': error})