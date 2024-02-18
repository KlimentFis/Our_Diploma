from django.db import models

class Word(models.Model):  # Исправлено: добавлено 'Model'
    name = models.CharField(max_length=100, blank=False)  # Исправлено: добавлен 'max_length'
    translate = models.CharField(max_length=100, blank=False)  # Исправлено: добавлен 'max_length'

    def __str__(self):
        return f"{self.name} - {self.translate}"

class Suggestion(models.Model):  # Исправлено: добавлено 'Model'
    suggestion = models.TextField(blank=False)
    right_word = models.CharField(max_length=100, blank=False)  # Исправлено: добавлен 'max_length'

    def __str__(self):
        return f"{self.suggestion} - {self.right_word}"