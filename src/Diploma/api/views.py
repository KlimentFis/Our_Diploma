from rest_framework.decorators import api_view, permission_classes
from rest_framework.permissions import IsAuthenticated
from rest_framework import status
from .serializers import MyUserSerializer, WordSerializer, SuggestionSerializer
from users.models import MyUser
from words.models import Word, Suggestion
from rest_framework_simplejwt.views import TokenObtainPairView
from rest_framework.response import Response

class CustomObtainJWTView(TokenObtainPairView):
    # Переопределяем метод post для возврата дополнительных данных, например, имя пользователя и его ID
    def post(self, request, *args, **kwargs):
        response = super().post(request, *args, **kwargs)
        if response.status_code == 200:
            # Добавляем дополнительные данные в ответ
            token = response.data['access']
            user = request.user
            # Возвращаем токен и дополнительные данные
            return Response({
                'token': token,
                'username': user.username,
                'user_id': user.id
            })
        return response

@api_view(['GET', 'POST'])
@permission_classes([IsAuthenticated])
def user_list(request):
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
@permission_classes([IsAuthenticated])
def delete_user(request):
    if 'password' in request.data and 'username' in request.data:
        password = request.data['password']
        username = request.data['username']
        try:
            user = MyUser.objects.get(username=username, password=password)
            user.delete()
            return Response(status=status.HTTP_204_NO_CONTENT)
        except MyUser.DoesNotExist:
            return Response({'error': 'Enter the correct user nickname and password'}, status=status.HTTP_404_NOT_FOUND)
    else:
        return Response({'error': 'Please provide user username and password.'}, status=status.HTTP_400_BAD_REQUEST)

@api_view(['GET', 'POST'])
@permission_classes([IsAuthenticated])
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
@permission_classes([IsAuthenticated])
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
