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
from django.contrib import admin
from django.urls import include, path
from django.conf import settings
from django.conf.urls.static import static
from rest_framework import permissions
from drf_yasg.views import get_schema_view
from drf_yasg import openapi
from tests.views import index, about_us, tests, links, about_us, download_app
from users.views import userList, login_or_register

schema_view = get_schema_view(
   openapi.Info(
      title="OnlineLessons API",
      default_version='v1',
      description="API documentation for OnlineLessons",
      terms_of_service="https://www.google.com/policies/terms/",
      contact=openapi.Contact(email="contact@onlinelessons.local"),
      license=openapi.License(name="BSD License"),
   ),
   public=True,
   permission_classes=(permissions.AllowAny,),
)

urlpatterns = [
    path('', login_or_register, name='index'),
    path('index/', index, name='start'),
    path('admin/', admin.site.urls),
    path('tests/', include('tests.urls'), name='tests'),
    path('usersList/', userList, name='UserList'),
    path('user/', include('users.urls')),
    path('links/', links, name='links'),
    path('about_us/', about_us, name='about_us'),
    path('download_app/', download_app, name="download_app"),
    path('api/', include('api.urls'), name="api"),
    path('swagger-docs/', schema_view.with_ui('swagger', cache_timeout=0), name='schema-swagger-ui'),
    path('redoc/', schema_view.with_ui('redoc', cache_timeout=0), name='schema-redoc'),
] + static(settings.STATIC_URL, document_root=settings.STATIC_ROOT)



if settings.DEBUG:
    urlpatterns += static(settings.STATIC_URL, document_root=settings.MEDIA_ROOT)
    urlpatterns += static(settings.MEDIA_URL, document_root=settings.MEDIA_ROOT)