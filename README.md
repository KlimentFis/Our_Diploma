# Диплом 
## Тема: Комплекс программных средств для изученя инностранных языков

# Web

## Установка

Клонирование проекта:
```shell
git clone https://github.com/KlimentFis/Our_Diploma 
```

Переход в папку проекта:
```shell
cd Our_Diploma/Сайт_для_изучения_иностранных_языков
```

Установка и активация виртуального окружения (Не обязательно):
```shell
python -m venv venv && venv\Scripts\activate.bat
```

Установка зависимостей ( <span style="color:red">ВЫСВЕЧИВАЕТСЯ ЛОЖНАЯ ОШИБКА</span> ):
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

Создание Супер-пользователя:
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

# Mobile
Установите нужные NuGet библиотеки:

| Id                  | Versions      | ProjectName  |
|---------------------|---------------|--------------|
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
