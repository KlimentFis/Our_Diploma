from django.contrib.auth.decorators import login_required
from django.http import JsonResponse
from django.shortcuts import render
from words.models import Word
# Create your views here.

@login_required
def translateWord(request):
    return render(request, 'translate_word.html')

def links(request):
    return render(request, 'links.html')

def all_words(request):
    context = {
        "data": Word.objects.all()
    }
    return render(request, 'tests/all_words.html', context)

def search_view(request):
    q = request.GET.get('q', '')
    if q:
        results = Word.objects.filter(name__istartswith=q) | Word.objects.filter(translate__istartswith=q)
    else:
        results = Word.objects.filter(name__istartswith='a')

    data = [{'id': item.id, 'name': item.name, 'translate': item.translate} for item in results]
    return JsonResponse(data, safe=False)