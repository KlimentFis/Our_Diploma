from rest_framework.response import Response
from rest_framework.decorators import api_view
from rest_framework import status
from .serializers import MyUserSerializer, WordSerializer, SuggestionSerializer
from words.models import Word, Suggestion
from users.models import MyUser

@api_view(['GET', 'POST'])
def my_user_list(request):
    if request.method == 'GET':
        users = MyUser.objects.all()
        serializer = MyUserSerializer(users, many=True)
        return Response(serializer.data)
    elif request.method == 'POST':
        serializer = MyUserSerializer(data=request.data)
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

@api_view(['POST'])
def delete_user(request):
    if 'id' in request.data and 'password' in request.data:  # Проверяем наличие обоих параметров в запросе
        user_id = request.data['id']
        password = request.data['password']
        try:
            user = MyUser.objects.get(id=user_id, password=password)  # Проверяем соответствие ID и пароля
            user.delete()
            return Response(status=status.HTTP_204_NO_CONTENT)
        except MyUser.DoesNotExist:
            return Response({'error': 'Пользователь с указанным ID не найден или пароль неверен'}, status=status.HTTP_404_NOT_FOUND)
    else:
        return Response({'error': 'Необходимо указать ID пользователя и пароль'}, status=status.HTTP_400_BAD_REQUEST)

@api_view(['GET', 'POST'])
def word_list(request):
    if request.method == 'GET':
        words = Word.objects.all()
        serializer = WordSerializer(words, many=True)
        return Response(serializer.data)
    elif request.method == 'POST':
        serializer = WordSerializer(data=request.data)
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

@api_view(['GET', 'POST'])
def suggestion_list(request):
    if request.method == 'GET':
        suggestions = Suggestion.objects.all()
        serializer = SuggestionSerializer(suggestions, many=True)
        return Response(serializer.data)
    elif request.method == 'POST':
        serializer = SuggestionSerializer(data=request.data)
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)
