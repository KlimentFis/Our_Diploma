# Дипломный проект
### Тема: Комплекс программных средств для изученя инностранных языков


### Авторы: [Трунов К. А.](https://github.com/KlimentFis) и [Пелипенко В. Б.](https://github.com/bipchik)


## Сайт

### Установка

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
# SECURITY WARNING: don't run with debug turned on in production!
DEBUG = True

ALLOWED_HOSTS = ['*']
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
```
Замените на
```
DATABASES = {
    'default': {
        'ENGINE': 'django.db.backends.sqlite3',
        'NAME': BASE_DIR / 'db.sqlite3',
    }
}
```

## Мобильное приложение
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

## Руководство по Rest API
### Авто-документация:
- #### http://127.0.0.1:8000/swagger-docs/
- #### http://127.0.0.1:8000/redoc/
