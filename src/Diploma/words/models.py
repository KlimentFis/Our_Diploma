from django.db import models

# Create your models here.

class Word(models.Model):  # Исправлено: добавлено 'Model'
    name = models.CharField(max_length=100, blank=False)  # Исправлено: добавлен 'max_length'
    translate = models.CharField(max_length=100, blank=False)  # Исправлено: добавлен 'max_length'

    def __str__(self):
        return self.name

class Suggestion(models.Model):  # Исправлено: добавлено 'Model'
    suggestion = models.TextField(blank=False)
    right_word = models.CharField(max_length=100, blank=False)  # Исправлено: добавлен 'max_length'
    # wrong_words = models.ForeignKey(Word, on_delete=models.CASCADE)  # Исправлено: добавлен 'on_delete'

    def __str__(self):
        return self.suggestion