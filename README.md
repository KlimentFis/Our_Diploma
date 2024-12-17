# Дипломный проект


### Тема: Комплекс программных средств для изученя инностранных языков


### Авторы:
- [Трунов К. А.](https://github.com/KlimentFis) ( [Сайт + API](https://github.com/KlimentFis/Our_Diploma/blob/main/Отчеты/220%20Трунов%20ДП.docx) )
- [Пелипенко В. Б.](https://github.com/bipchik) ( [Мобильное приложение](https://github.com/KlimentFis/Our_Diploma/blob/main/Отчеты/220%20Пелипенко%20ДП.docx)  )


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

### Запуск сайта
Для локальной разработки:
```shell
python manage.py runserver
```
Для продакшена:
```shell
python manage.py runserver 0.0.0.0:8888
```

### Руководство по Rest API
Авто-документация:
- http://127.0.0.1:8000/swagger-docs/
- http://127.0.0.1:8000/redoc/

### Заключение
Мобильное приложение может работать только если запущен сайт, т.к. данные в мобильном приложении получаются посредством обращения мобильного приложения к REST API сайта, поэтому, сайт, в этом программном комплексе, является ключевым звеном, доступ к которому, постоянно, должен быть у мобильного приложения.


### Мобильное приложение
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

### Зпуск Мобильного приложения
<p>
  <img src="https://avatars.mds.yandex.net/i?id=0131f3b8ed7d7cc23cd520919a5583e3d26a0ca3-10803837-images-thumbs&n=13" alt="Контактная книга">
</p>
Запустите мобильное приложение, посредством запуска проекта в Visual Studio.
