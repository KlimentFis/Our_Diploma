"""main URL Configuration

The `urlpatterns` list routes URLs to views. For more information please see:
    https://docs.djangoproject.com/en/3.2/topics/http/urls/
Examples:
Function views
    1. Add an import:  from my_app import views
    2. Add a URL to urlpatterns:  path('', views.home, name='home')
Class-based views
    1. Add an import:  from other_app.views import Home
    2. Add a URL to urlpatterns:  path('', Home.as_view(), name='home')
Including another URLconf
    1. Import the include() function: from django.urls import include, path
    2. Add a URL to urlpatterns:  path('blog/', include('blog.urls'))
"""
from django.urls import path
from django.conf.urls.static import static
from django.conf import settings
from .views import letter_verification, check_word, check_suggestion, tests, translateWord, about_us, text_to_audio
from words.views import all_words, search_view

urlpatterns = [
    path('', tests, name='tests'),
    path('letter_verification/', letter_verification, name='letter_verification'),
    path('check_word/', check_word, name='check_word'),
    path('check_suggestion/', check_suggestion, name='check_suggestion'),
    path('translate_word/', translateWord, name='translateWord'),
    path('all_words/', all_words, name='all_words'),
    path('search/', search_view, name='search'),
    path('text_to_audio/', text_to_audio, name='text_to_audio'),
]

if settings.DEBUG:
    urlpatterns += static(settings.STATIC_URL, document_root=settings.STATIC_ROOT)
    urlpatterns += static(settings.MEDIA_URL, document_root=settings.MEDIA_ROOT)

# urlpatterns = [
#     path('', tests, name='tests'),
#     path('letter_verification/', letter_verification, name='letter_verification'),
#     path('check_word/', check_word, name='check_word'),
#     path('check_suggestion/', check_suggestion, name='check_suggestion'),
#     path('translate_word', translateWord, name='translateWord'),
#     # path('all_words/', all_words, name="all_words"),
#
#
#     # path('all_words/', all_words, name='all_words'),
#     # path('search/', search_view, name='search'),
#
#     path('all_words/', all_words, name='all_words'),
#     path('search/', search_view, name='search'),  # Проверьте, что этот маршрут существует
#
#     # path('text_to_audio/', text_to_audio, name="text_to_audio")
#     path('text_to_audio/', text_to_audio, name='text_to_audio'),
# ]
#
# if settings.DEBUG:
#     urlpatterns += static(settings.STATIC_URL, document_root=settings.MEDIA_ROOT)
#     urlpatterns += static(settings.MEDIA_URL, document_root=settings.MEDIA_ROOT)