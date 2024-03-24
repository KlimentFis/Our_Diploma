from django.core.management.base import BaseCommand
from words.models import Word
from bs4 import BeautifulSoup as BS
from fake_useragent import UserAgent
from requests import get

class Command(BaseCommand):
    help = 'Parse data and save to database'

    def handle(self, *args, **kwargs):
        url = "https://habr.com/ru/articles/240439/"
        words = []
        translates = []

        headers = {
            "user-agent": UserAgent().random
        }

        page = get(url, headers=headers)
        soup = BS(page.text, "html.parser")
        raw_data = soup.find("table").find_all("tr")  # Используем find_all вместо findAll

        if len(raw_data) >= 2:
            for row in raw_data[1:]:  # Начинаем с второй строки таблицы
                td = row.find_all("td")  # Находим все теги <td> в этой строке
                if len(td) >= 3:  # Проверяем, что третий элемент существует
                    word = td[1].text.strip()  # Получаем текст из второго элемента и удаляем лишние пробелы
                    translate = td[2].text.strip()  # Получаем текст из третьего элемента и удаляем лишние пробелы
                    words.append(word)
                    translates.append(translate)

        # Сохраняем данные в базу данных
        for i in range(len(words)):
            new_word = Word(name=words[i], translate=translates[i])
            new_word.save()

        self.stdout.write(self.style.SUCCESS('Data parsed and saved successfully!'))
