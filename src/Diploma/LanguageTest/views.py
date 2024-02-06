from django.contrib.auth.decorators import login_required
from django.shortcuts import render

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

def register(request):
    return render(request, 'register.html')

def login(request):
    return render(request, 'login.html')

@login_required
def letter_verification(request):
    return render(request, 'LetterVerification.html')

@login_required
def check_word(request):
    return render(request, 'selectWord.html')

@login_required
def userList(request):
    return render(request, 'userList.html')

@login_required
def translateWord(request):
    return render(request, 'translateWord.html')

def login_or_register(request):
    return render(request, 'login_or_register.html')