# Диплом 
## Тема: Комплекс программных средств для изученя инностранных языков

## Авторы: [Трунов К. А.](https://github.com/KlimentFis) и [Пелипенко В. Б.](https://github.com/bipchik)

# Сайт

## Установка

Клонирование проекта:
```shell
git clone https://github.com/KlimentFis/Our_Diploma 
```

Переход в папку проекта:
```shell
cd Our_Diploma/Сайт_для_изучения_иностранных_языков
```

Установка и активация виртуального окружения ( Не обязательно ):
```shell
python -m venv venv && venv\Scripts\activate.bat
```

Установка зависимостей:
```shell
pip install -r requirements.txt --use-deprecated legacy-resolver
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

Создание Супер-пользователя ( Не обязательно ):
```shell
python manage.py createsuperuser
```

## Запуск проекта
Для локальной разработки:
```shell
python manage.py runserver
```
Для продакшена:
```shell
python manage.py runserver 0.0.0.0:8888
```
## Заключение
При вводе сайта в эксплуатацию, поставьте флаг DEBUG=True в положение False, в фале [settings.py](Сайт_для_изучения_иностранных_языков/main/settings.py)

```
import secrets
JWT_SECRET_KEY = secrets.token_urlsafe(32)

# SECURITY WARNING: keep the secret key used in production secret!
SECRET_KEY = 'django-insecure-5g(3gz1rva!p3j%944$o(z(tstp+)glg(a5pzdhib8g3$n@n^y'

# SECURITY WARNING: don't run with debug turned on in production!
DEBUG = True

ALLOWED_HOSTS = ['*']

LOGOUT_REDIRECT_URL = '/index'
LOGIN_URL = '/'
```

При локальной разработке можно использовать sqlite3 вместо MySQL
```
DATABASES = {
    'default': {
        'ENGINE': 'django.db.backends.mysql',
        'NAME': 'OurDiploma',
        'USER': 'root',
        'PASSWORD': 'MySQL',
        'HOST': 'localhost',
        'PORT': '3306',
    }
}

# DATABASES = {
#     'default': {
#         'ENGINE': 'django.db.backends.sqlite3',
#         'NAME': BASE_DIR / 'db.sqlite3',
#     }
# }
```
Замените на
```
# DATABASES = {
#     'default': {
#         'ENGINE': 'django.db.backends.mysql',
#         'NAME': 'OurDiploma',
#         'USER': 'root',
#         'PASSWORD': 'MySQL',
#         'HOST': 'localhost',
#         'PORT': '3306',
#     }
# }

DATABASES = {
    'default': {
        'ENGINE': 'django.db.backends.sqlite3',
        'NAME': BASE_DIR / 'db.sqlite3',
    }
}
```

# Мобильное приложение
Установите нужные NuGet библиотеки:

| Id                  | Versions      | ProjectName  |
|:---------------------:|:---------------:|:--------------:|
| NETStandard.Library | {2.0.3}       | Diplom       |
| Xamarin.Essentials  | {1.7.0}       | Diplom       |
| System.Net.Http     | {4.3.4}       | Diplom       |
| Newtonsoft.Json     | {13.0.3}      | Diplom       |
| Microsoft.CSharp    | {4.7.0}       | Diplom       |
| Xamarin.Forms       | {5.0.0.2196}  | Diplom       |
| Xam.Plugin.Media    | {6.0.2}       | Diplom       |
| Microsoft.CSharp    | {4.7.0}       | Diplom.Android |
| Newtonsoft.Json     | {13.0.3}      | Diplom.Android |
| System.Net.Http     | {4.3.4}       | Diplom.Android |
| Xamarin.Forms       | {5.0.0.2196}  | Diplom.Android |
| Xamarin.Essentials  | {1.7.0}       | Diplom.Android |



# Руководство по Rest API
- ### GET /api/users/ - Для получения всех пользователей
#### Результат выполнения:
```json
{
  "id": 1,
  "image": "/media/users/eefe9ac4dc14bcd8af5e28bfaace5931.jpg",
  "last_login": "2024-04-21T19:22:33.700804+03:00",
  "is_superuser": false,
  "username": "",
  "first_name": "",
  "last_name": "",
  "email": "",
  "is_staff": false,
  "is_active": true,
  "date_joined": "2024-04-21T19:22:33.690868+03:00",
  "patronymic": "",
  "use_english": false,
  "anonymous": false,
  "data_joined": "2024-04-21T19:22:33.364042+03:00",
  "right_answers": 2,
  "wrong_answers": 1,
  "groups": [],
  "user_permissions": []
}
```
- ### GET /api/words/ - Для получения всех слов

#### Результат выполнения:
```json
{
  "id": 1,
  "name": "absolute",
  "translate": "абсолютный"
}
```
- ### GET /api/suggestions/ - Для получения всех предложений

#### Результат выполнения:
```json
{
  "id": 1,
  "suggestion": "The cat chased the mouse around the house.",
  "right_word": "mouse"
}
```
- ### POST /api/create_user/ - Для создания пользователя
#### Входные данные:
```json
{
  "username": "",
  "password": ""
}
```
#### Результат выполнения:
```json
{
  "refresh": "",
  "access": ""
}
```

- ### POST /api/delete_user/ - Для удаления пользователя [!!!]
#### Входные данные:
```json
{
  "username": "",
  "password": ""
}
```
#### Результат выполнения:
```json
{
  "detail": "Пользователь успешно удален!!!"
}
```
- ### POST /api/token/ - Для получения access и refresh токена
#### Входные данные:
```json
{
  "username": "",
  "password": ""
}
```
#### Результат выполнения:
```json
{
  "refresh": "",
  "access": ""
}
```
