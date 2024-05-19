from rest_framework.decorators import api_view, permission_classes
from rest_framework.permissions import IsAuthenticated, AllowAny
from rest_framework.response import Response
from users.models import MyUser
from words.models import Word, Suggestion
from .serializers import MyUserSerializer, WordSerializer, SuggestionSerializer
from rest_framework.exceptions import AuthenticationFailed
from rest_framework_simplejwt.tokens import RefreshToken
from rest_framework import status

@api_view(['POST'])
@permission_classes([AllowAny])
def create_user(request):
    if request.method == 'POST':
        serializer = MyUserSerializer(data=request.data)
        if serializer.is_valid():
            user = serializer.save()
            refresh = RefreshToken.for_user(user)
            return Response({
                'refresh': str(refresh),
                'access': str(refresh.access_token)
            }, status=status.HTTP_201_CREATED)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

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

@api_view(['GET', 'PUT', 'DELETE'])
@permission_classes([IsAuthenticated])
def user_detail(request, username):
    try:
        user = MyUser.objects.get(username=username)
    except MyUser.DoesNotExist:
        return Response(status=status.HTTP_404_NOT_FOUND)

    if request.method == 'GET':
        serializer = MyUserSerializer(user)
        return Response(serializer.data)

    elif request.method == 'PUT':
        serializer = MyUserSerializer(user, data=request.data)
        if serializer.is_valid():
            serializer.save()
            return Response(serializer.data)
        return Response(serializer.errors, status=status.HTTP_400_BAD_REQUEST)

    elif request.method == 'DELETE':
        user.delete()
        return Response(status=status.HTTP_204_NO_CONTENT)

@api_view(['GET'])
@permission_classes([IsAuthenticated])
def word_list(request):
    if request.method == 'GET':
        words = Word.objects.all()
        serializer = WordSerializer(words, many=True)
        return Response(serializer.data)
    else:
        return Response(status=status.HTTP_405_METHOD_NOT_ALLOWED)

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


@api_view(['POST'])
@permission_classes([IsAuthenticated])
def delete_user(request):
    username = request.user.username
    password = request.user.password

    if username and password:
        try:
            user = MyUser.objects.get(username=username, password=password)
            user.delete()
            # return Response(status=status.HTTP_204_NO_CONTENT)
            return Response({'detail': 'Пользователь успешно удален!!!'})
        except MyUser.DoesNotExist:
            return Response({'error': 'Enter the correct user nickname and password'}, status=status.HTTP_404_NOT_FOUND)
    else:
        raise AuthenticationFailed("Учетные данные не были предоставлены.")