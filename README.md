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
### Авто-документация:
- #### http://127.0.0.1:8000/swagger-docs/
- #### http://127.0.0.1:8000/redoc/
