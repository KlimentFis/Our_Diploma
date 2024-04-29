from rest_framework import serializers, permissions, viewsets
from rest_framework.authentication import TokenAuthentication
from rest_framework.permissions import IsAuthenticated  # Добавим импорт разрешения IsAuthenticated
from words.models import Word, Suggestion
from users.models import MyUser
from django.contrib.auth.hashers import make_password

class MyUserSerializer(serializers.ModelSerializer):
    image = serializers.ImageField(allow_null=True)

    class Meta:
        model = MyUser
        fields = '__all__'
        # Это поле должно быть доступно только для чтения после создания пользователя
        extra_kwargs = {'password': {'write_only': True}}

    def create(self, validated_data):
        validated_data['password'] = make_password(validated_data['password'])
        return super().create(validated_data)

class WordSerializer(serializers.ModelSerializer):
    class Meta:
        model = Word
        fields = '__all__'

class SuggestionSerializer(serializers.ModelSerializer):
    class Meta:
        model = Suggestion
        fields = '__all__'

class WordViewSet(viewsets.ModelViewSet):
    queryset = Word.objects.all()
    serializer_class = WordSerializer
    authentication_classes = [TokenAuthentication]  # Добавляем аутентификацию по токену
    permission_classes = [IsAuthenticated]  # Только аутентифицированные пользователи имеют доступ к данным

class SuggestionViewSet(viewsets.ModelViewSet):
    queryset = Suggestion.objects.all()
    serializer_class = SuggestionSerializer
    authentication_classes = [TokenAuthentication]  # Добавляем аутентификацию по токену
    permission_classes = [IsAuthenticated]  # Только аутентифицированные пользователи имеют доступ к данным
