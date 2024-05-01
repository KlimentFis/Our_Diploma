# Диплом

# Web

## Установка

Клонирование проекта:
```shell
git clone https://github.com/KlimentFis/Our_Diploma 
```

Переход в папку проекта
```shell
cd Our_Diploma/Сайт_для_изучения_иностранных_языков
```

Установка и активация виртуального окружения (Не обязательно):
```shell
python -m venv venv && venv\Scripts\activate.bat
```

Установка зависимостей:
```shell
pip install -r requerements.txt
```

Переход в папку проекта
```shell
cd Diploma
```

Создание миграций:
```shell
python manage.py makemigrations
```

Проведение миграций:
```shell
python manage.py migrate
```

Заполнение Базы данных словами с переводом:
```shell
python manage.py parse_and_save
```

Заполнение Базы данных предложениями и словами:
```shell
python manage.py create_suggestions static/seggestions.txt
```

Создание Супер-пользователя:
```shell
python manage.py createsuperuser
```

Запуск проекта:
```shell
python manage.py runserver
```

# Mobile

```
Ничего устанавливать, кроме Visual Studio не нужно :)
```

#### P.S. В проекте используется Rest API, то-есть мобильное приложение отправляет JSON запрос к сайту, а сайт отправляет обратно JSON ответ. В связи с этим, должны быть запущены оба приложения!!!
