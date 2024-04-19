from rest_framework import serializers
from words.models import Word, Suggestion
from users.models import MyUser
from django.contrib.auth.hashers import make_password

class MyUserSerializer(serializers.ModelSerializer):
    image = serializers.ImageField(allow_null=True)

    class Meta:
        model = MyUser
        fields = '__all__'

    def create(self, validated_data):
        # Хешируем пароль перед сохранением пользователя
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