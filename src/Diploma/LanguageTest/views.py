import os
from random import randint, sample, shuffle
from django.http import HttpResponseNotAllowed, HttpResponseForbidden, JsonResponse, HttpResponse
from django.views.decorators.csrf import csrf_protect, csrf_exempt
from django.contrib.auth.decorators import login_required
from django.shortcuts import render, redirect
from language_tool_python import LanguageTool
from gtts import gTTS
from words.models import Word
from words.models import Suggestion
from users.views import check_auth

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
    tool = LanguageTool('en-US')
    if request.method == 'GET':
        return render(request, 'tests/LetterVerification.html')
    elif request.method == 'POST':
        text = request.POST.get('not_checked_text', '')
        errors = tool.check(text)
        if not errors:
            errors = None
        print(errors)
        context = {'text': text, 'errors': errors}
        return render(request, 'tests/LetterVerification.html', context)

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

    return render(request, 'tests/selectWord.html', context)

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
    replaced_suggestion = random_suggestion.suggestion.replace(right_word, '...')

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
        replaced_suggestion = random_suggestion.suggestion.replace(right_word, '...')
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
    return render(request, 'tests/translateWord.html')









def download_app(request):
    ...
#     # Path to the file you want to serve for download
#     file_name = 'brain.jpg'
#
#     # Construct the absolute file path using STATIC_ROOT
#     file_path = os.path.join(settings.STATIC_ROOT, file_name)
#
#     # Check if the file exists
#     if os.path.exists(file_path):
#         # Open the file in binary mode
#         with open(file_path, 'rb') as file:
#             # Read the file content
#             file_content = file.read()
#             # Create an HTTP response with the file content as the body
#             response = HttpResponse(file_content, content_type='image/jpeg')
#             # Set the Content-Disposition header to make the browser download the file
#             response['Content-Disposition'] = 'attachment; filename="img.jpg"'
#             return response
#     else:
#         # Handle the case where the file does not exist
#         return HttpResponse("File not found", status=404)


def download_file(request):
    # Путь к файлу для скачивания
    file_path = "{% load static %}{% static 'brain.jpg' %}"

    # Открываем файл для чтения в бинарном режиме
    with open(file_path, 'rb') as f:
        # Читаем содержимое файла
        file_data = f.read()

    # Определяем MIME-тип файла
    content_type = "text/plain"

    # Создаем HTTP-ответ с содержимым файла в теле ответа
    response = HttpResponse(file_data, content_type=content_type)

    # Устанавливаем заголовок Content-Disposition, чтобы браузер понял, что это вложение и должно быть скачано
    response['Content-Disposition'] = 'attachment; filename="file.txt"'

    # Возвращаем HTTP-ответ
    return response

# def text_to_audio(request):
#     return render(request, 'tests/text_to_audio.html')

def text_to_audio(request):
    error = check_auth(request, "Необходимо авторизоваться!")
    if error:
        return error
    if request.method == 'POST':
        text = request.POST.get('text', '')
        if text:
            # Преобразование текста в аудио
            audio_file_path = os.path.join('media', 'output.mp3')
            tts = gTTS(text=text, lang='en')
            tts.save(audio_file_path)
            return render(request, 'tests/text_to_audio.html', {'audio_file_path': audio_file_path})
    return render(request, 'tests/text_to_audio.html', {'audio_file_path': None})