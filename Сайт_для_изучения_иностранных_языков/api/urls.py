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
from .views import user_list, word_list, suggestion_list, delete_user, user_signup
from rest_framework_simplejwt.views import TokenObtainPairView, TokenRefreshView

urlpatterns = [
    path('users/', user_list, name='user_list'),
    path('words/', word_list, name='word_list'),
    path('suggestions/', suggestion_list, name='suggestion_list'),
    path('signup/', user_signup, name='user_signup'),
    path('delete_user/', delete_user, name='delete_user'),
    path('token/', TokenObtainPairView.as_view(), name='token_obtain_pair'),
    path('token/refresh/', TokenRefreshView.as_view(), name='token_refresh'),
    path('user_signup/', user_signup ,name='user_signup'),
    # path('get_csrf_token/', get_csrf_token, name='get_csrf_token')
]
