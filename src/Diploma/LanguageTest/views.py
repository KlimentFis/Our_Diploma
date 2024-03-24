import re
from random import randint, sample, shuffle, random
from django.http import HttpResponseNotAllowed, HttpResponseForbidden, JsonResponse
from django.views.decorators.csrf import csrf_protect, csrf_exempt
from django.contrib.auth import logout
from django.contrib.auth import authenticate
from django.contrib.auth.decorators import login_required
from django.contrib.auth import get_user_model, login
from django.utils import timezone
from django.shortcuts import render, redirect
from language_tool_python import LanguageTool
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

@login_required
def translateWord(request):
    return render(request, 'tests/translateWord.html')