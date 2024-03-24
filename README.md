# Диплом

# Web

## Установка

Клонирование и переход в папку проекта:
```shell
git clone https://github.com/KlimentFis/Our_Diploma && cd Our_Diploma
```

Установка и активация виртуального окружения (Не обязательно):
```shell
python -m venv venv && venv\Scripts\activate.bat
```

Установка зависимостей:
```shell
pip install -r requerements.txt
```

Создание миграций:
```shell
python manage.py makemigrations
```

Проведение миграций:
```shell
python manage.py migrate
```

Заполнение Базы данных:
```shell
python manage.py parse_and_save
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

