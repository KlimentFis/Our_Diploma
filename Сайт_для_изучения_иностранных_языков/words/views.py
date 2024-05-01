from django.contrib.auth.decorators import login_required
from django.shortcuts import render
from words.models import Word
# Create your views here.

@login_required
def translateWord(request):
    return render(request, 'translateWord.html')

def links(request):
    return render(request, 'links.html')

def all_words(request):
    context = {
        "data": Word.objects.all()
    }
    return render(request, 'tests/all_words.html', context)