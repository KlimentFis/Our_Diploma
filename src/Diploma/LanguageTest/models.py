from django.contrib.auth.models import AbstractUser
from django.db import models

# Create your models here.
class MyUser(AbstractUser):
    username = models.CharField(max_length=50)
    hash_password = models.CharField(max_length=50)
    first_name = models.CharField(max_length=75, blank=True)
    last_name = models.CharField(max_length=75, blank=True)
    patronymic = models.CharField(max_length=75, blank=True)
    is_active = models.BooleanField()
    is_staff = models.BooleanField()
    is_superuser = models.BooleanField()
    data_joined = models.DateField()
    groups = models.ManyToManyField('auth.Group', related_name='myuser_groups')
    user_permissions = models.ManyToManyField('auth.Permission', related_name='myuser_user_permissions')
