from django.shortcuts import render

# Create your views here.
def index(request):
    return render(request, 'main.html')

def links(request):
    return render(request, 'links.html')

def about_us(request):
    return render(request, 'about_as.html')

def profile(request):
    return render(request, 'profile.html')

def register(request):
    return render(request, 'register.html')

def login(request):
    return render(request, 'login.html')

def letter_verification(request):
    return render(request, 'LetterVerification.html')

def check_word(request):
    return render(request, 'selectWord.html')

def userList(request):
    return render(request, 'userList.html')

def translateWord(request):
    return render(request, 'translateWord.html')