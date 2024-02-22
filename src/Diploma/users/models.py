from django.contrib.auth.models import AbstractUser
from django.db import models
from django.utils import timezone

class MyUser(AbstractUser):
    image = models.ImageField(upload_to='users', blank=True, verbose_name='фото')
    patronymic = models.CharField(max_length=75, blank=True)
    use_english = models.BooleanField(default=False, blank=True)
    anonymous = models.BooleanField(default=False, blank=False)
    data_joined = models.DateTimeField(null=True, blank=True)