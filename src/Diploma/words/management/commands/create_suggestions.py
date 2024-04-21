from django.core.management.base import BaseCommand
from words.models import Suggestion  # Замените myapp на имя вашего приложения
import os

class Command(BaseCommand):
    help = 'Creates Suggestion objects from a text file'

    def add_arguments(self, parser):
        parser.add_argument('file_path', type=str, help='Path to the file with suggestions')

    def handle(self, *args, **options):
        file_path = options['file_path']

        if not os.path.exists(file_path):
            self.stdout.write(self.style.ERROR(f"File '{file_path}' does not exist"))
            return

        with open(file_path, 'r', encoding='utf-8') as f:
            lines = f.readlines()

        for i in range(0, len(lines), 2):
            suggestion_text = lines[i].strip()[13:]
            right_word = lines[i + 1].strip()[7:]
            suggestion = Suggestion.objects.create(suggestion=suggestion_text, right_word=right_word)
            self.stdout.write(self.style.SUCCESS(f"Created Suggestion object: {suggestion}"))