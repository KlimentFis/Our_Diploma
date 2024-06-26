from rest_framework import serializers
from words.models import Word, Suggestion
from django.contrib.auth.hashers import make_password
from users.models import MyUser


class MyUserSerializer(serializers.ModelSerializer):
    image = serializers.ImageField(allow_null=True, required=False)
    password = serializers.CharField(write_only=True)  # поле 'password' соответствует полю в модели MyUser

    class Meta:
        model = MyUser
        fields = ('username', 'first_name', 'last_name', 'patronymic', 'wrong_answers', 'right_answers', 'anonymous', 'use_english', 'image', 'password')
        extra_kwargs = {
            'id': {'read_only': True}  # id только для чтения
        }

    def create(self, validated_data):
        validated_data['password'] = make_password(
            validated_data.get('password'))  # использование 'password' из validated_data
        return super().create(validated_data)

    def update(self, instance, validated_data):
        if 'password' in validated_data:
            validated_data['password'] = make_password(
                validated_data.get('password'))  # использование 'password' из validated_data

            # Проверка и исключение поля last_login
        if 'last_login' in validated_data:
            validated_data.pop('last_login')

            # Проверка и исключение поля date_joined
        if 'date_joined' in validated_data:
            validated_data.pop('date_joined')

        return super().update(instance, validated_data)

class WordSerializer(serializers.ModelSerializer):
    class Meta:
        model = Word
        fields = '__all__'

class SuggestionSerializer(serializers.ModelSerializer):
    class Meta:
        model = Suggestion
        fields = '__all__'