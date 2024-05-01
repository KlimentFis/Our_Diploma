from django.contrib import admin
from .models import Suggestion, Word

# Register your models here.
admin.site.register(Word)
admin.site.register(Suggestion)