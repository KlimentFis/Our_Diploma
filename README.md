# Дипломный проект

### Тема: Комплекс программных средств для изучения иностранных языков

### Авторы:
- [Трунов К. А.](https://github.com/KlimentFis) ( [Сайт + API](https://github.com/KlimentFis/Our_Diploma/blob/main/Отчеты/220%20Трунов%20ДП.docx) )
- [Пелипенко В. Б.](https://github.com/bipchik) ( [Мобильное приложение](https://github.com/KlimentFis/Our_Diploma/blob/main/Отчеты/220%20Пелипенко%20ДП.docx) )

### Установка

#### Клонирование проекта:
```shell
git clone https://github.com/KlimentFis/Our_Diploma
```

#### Переход в папку проекта:
```shell
cd Our_Diploma/Сайт_для_изучения_иностранных_языков
```

#### Установка и активация виртуального окружения (не обязательно):
```shell
python -m venv venv && venv\Scripts\activate.bat
```

#### Установка зависимостей:
```shell
pip install -r requirements.txt --use-deprecated legacy-resolver
```

#### Создание миграций:
```shell
python manage.py makemigrations
```

#### Проведение миграций:
```shell
python manage.py migrate
```

#### Заполнение базы данных словами с переводом:
```shell
python manage.py parse_and_save
```

#### Заполнение базы данных предложениями и словами:
```shell
python manage.py create_suggestions static/suggestions.txt
```

#### Создание суперпользователя (не обязательно):
```shell
python manage.py createsuperuser
```

### Запуск сайта

Для локальной разработки:
```shell
python manage.py runserver
```

Для продакшн-сервера:
```shell
python manage.py runserver 0.0.0.0:8888
```

### Руководство по Rest API

Автоматическая документация:
- [Swagger UI](http://127.0.0.1:8000/swagger-docs/)
- [ReDoc](http://127.0.0.1:8000/redoc/)

### Заключение

Мобильное приложение может работать только в случае, если запущен сайт, так как данные в мобильном приложении получаются через запросы к REST API сайта. Следовательно, сайт является ключевым звеном в этом программном комплексе и должен быть всегда доступен для мобильного приложения.

### Мобильное приложение

Установите нужные NuGet библиотеки:

| Id                  | Версии        | Название проекта  |
|:---------------------:|:-------------:|:------------------:|
| NETStandard.Library  | {2.0.3}       | Diplom             |
| Xamarin.Essentials   | {1.7.0}       | Diplom             |
| System.Net.Http      | {4.3.4}       | Diplom             |
| Newtonsoft.Json      | {13.0.3}      | Diplom             |
| Microsoft.CSharp     | {4.7.0}       | Diplom             |
| Xamarin.Forms        | {5.0.0.2196}  | Diplom             |
| Xam.Plugin.Media     | {6.0.2}       | Diplom             |
| Microsoft.CSharp     | {4.7.0}       | Diplom.Android     |
| Newtonsoft.Json      | {13.0.3}      | Diplom.Android     |
| System.Net.Http      | {4.3.4}       | Diplom.Android     |
| Xamarin.Forms        | {5.0.0.2196}  | Diplom.Android     |
| Xamarin.Essentials   | {1.7.0}       | Diplom.Android     |

### Запуск мобильного приложения

<p>
  <img src="https://avatars.mds.yandex.net/i?id=0131f3b8ed7d7cc23cd520919a5583e3d26a0ca3-10803837-images-thumbs&n=13" alt="Контактная книга">
</p>
Запустите мобильное приложение, запустив проект в Visual Studio.
