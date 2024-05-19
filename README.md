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

## Получение данных по Rest API

- ### Получение access токена:
```

string url = "http://192.168.1.16:8888/api/token/";
string username = LoginEntry.Text;
string password = PasswordEntry.Text;
string jsonData = $"{{\"username\":\"{username}\",\"password\":\"{password}\"}}";

using (HttpClient client = new HttpClient())
{
  HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
  HttpResponseMessage response = await client.PostAsync(url, content);

  if (response.IsSuccessStatusCode)
  {
  	string responseContent = await response.Content.ReadAsStringAsync();

  	// Распарсить responseContent в объект
  	var responseObject = JsonConvert.DeserializeObject<dynamic>(responseContent);

    // Получить значения токенов из объекта
    string refreshToken = responseObject.refresh;
    string accessToken = responseObject.access;

    // Сохранить токены в хранилище приложения
    Application.Current.Properties["RefreshToken"] = refreshToken;
    Application.Current.Properties["AccessToken"] = accessToken;
    await Application.Current.SavePropertiesAsync();

    // Перейти на другую страницу после успешной аутентификации
    await DisplayAlert("Успех", "Вы успешно вошли", "OK");
    await Navigation.PushAsync(new UsersPage());
  }
  else
  {
    // Вывод сообщения об ошибке в случае неудачного запроса
    await DisplayAlert("Ошибка", $"Ошибка при выполнении запроса: {response.StatusCode}", "OK");
  }
}
```
- ### Получение данных от Rest API с помощью access токена:
```
// Получение access токена из хранилища
string accessToken = Application.Current.Properties["AccessToken"].ToString();

// Создание объекта HttpClient для отправки запросов
using (HttpClient client = new HttpClient())
{
  // Создание объекта HttpRequestMessage
  var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
  // Добавление токена в заголовок запроса "Authorization"
  request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

  // Отправка запроса и получение ответа
  HttpResponseMessage response = await client.SendAsync(request);

  // Проверка успешности запроса
  if (response.IsSuccessStatusCode)
  {
    // Чтение ответа в формате JSON
    string jsonResponse = await response.Content.ReadAsStringAsync();
    // Десериализация JSON в список пользователей
    List<MyUser> users = JsonConvert.DeserializeObject<List<MyUser>>(jsonResponse);
    // Добавление адреса к изображениям
    foreach (var user in users)
    {
      user.Image = "http://127.0.0.1:8888" + user.Image;
      Console.WriteLine(user.Image);
    }
    // Отображение списка пользователей на странице
    listView.ItemsSource = users;
  }
  else
  {
  await DisplayAlert("Ошибка", $"Ошибка: {response.StatusCode}", "OK");
  }
}
```

# Руководство по Rest API
- ### GET /api/users/ - Для получения всех пользователей

Нужно передать в заголовке:
```
Authorization: Brearer access_токен
```

Результат выполнения:
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
Нужно передать в заголовке:
```
Authorization: Brearer access_токен
```
Результат выполнения:
```json
{
  "id": 1,
  "name": "absolute",
  "translate": "абсолютный"
}
```
- ### GET /api/suggestions/ - Для получения всех предложений

Нужно передать в заголовке:
```
Authorization: Brearer access_токен
```

Результат выполнения:
```json
{
  "id": 1,
  "suggestion": "The cat chased the mouse around the house.",
  "right_word": "mouse"
}
```
- ### POST /api/create_user/ - Для создания пользователя
Входные данные:
```json
{
  "username": "",
  "password": ""
}
```
Результат выполнения:
```json
{
  "refresh": "",
  "access": ""
}
```
- ### GET /api/all_users/ - Для получения всех ников пользоватеей
Результат выполнения:
```json
[
    "Test",
    "TEST",
    "Kirill",
    "MyTestUser"
]
```

- ### PUT /api/users/<str:username>/ - Для обнавления данных пользователя
Входные данные:
```json 
{
    "password": "",
    "username": "",
    "use_english": false
}
```
Результат выполнения:
```json
{
    "id": 5,
    "image": null,
    "password": "",
    "last_login": "2024-05-19T04:31:13.161827+03:00",
    "is_superuser": false,
    "username": "MyTestUser",
    "first_name": "",
    "last_name": "",
    "email": "",
    "is_staff": false,
    "is_active": true,
    "date_joined": "2024-05-19T04:31:13.145312+03:00",
    "patronymic": "",
    "use_english": false,
    "anonymous": false,
    "data_joined": "2024-05-19T04:31:12.777854+03:00",
    "right_answers": 0,
    "wrong_answers": 0,
    "groups": [],
    "user_permissions": []
}
```

- ### POST /api/delete_user/ - Для удаления пользователя [❗❗❗]
Нужно передать в заголовке:
```
Authorization: Brearer access_токен
```
Входные данные:
```json
{
  "username": "",
  "password": ""
}
```
Результат выполнения:
```json
{
  "detail": "Пользователь успешно удален!!!"
}
```
- ### POST /api/token/ - Для получения access и refresh токена
Входные данные:
```json
{
  "username": "",
  "password": ""
}
```
Результат выполнения:
```json
{
  "refresh": "",
  "access": ""
}
```

# Тестирование Rest API
Запуск приложения для тестирования API:
```
python manage.py runserver 0.0.0.0:8888
```

## Тестирование с помощью Эмулятора Android
```
Все ссылки в приложении должны иметь ввид http://localhost:8888
 ```
Пример:
```
readonly string apiUrl = "http://localhost:8888/api/users/";
```
## Тестирование с помощью устройства Android*
```
Все ссылки в приложении должны иметь ввид 

http:// + Адрес_полученный_с_помощью_сканирования_сети + :8888
```
Пример:
```
readonly string apiUrl = "http://192.168.1.16:8888/api/users/";
```

(*) подключенного к той же сети
## Заключение
``` text
Данное Rest API тестировалось с помощью ноутбука с запущенным Rest API

и телефоном под управлением операционной системой Android
```