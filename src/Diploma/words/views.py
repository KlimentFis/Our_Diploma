from django.contrib.auth.decorators import login_required
from django.shortcuts import render

# Create your views here.

@login_required
def translateWord(request):
    return render(request, 'translateWord.html')

def links(request):
    return render(request, 'links.html')