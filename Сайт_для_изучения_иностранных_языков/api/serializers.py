from rest_framework import serializers
from words.models import Word, Suggestion
from django.contrib.auth.hashers import make_password
from users.models import MyUser


class MyUserSerializer(serializers.ModelSerializer):
    image = serializers.ImageField(allow_null=True, required=False)

    class Meta:
        model = MyUser
        fields = '__all__'
        extra_kwargs = {
            'password': {'write_only': True},
            'is_active': {'default': True},  # Установка is_active по умолчанию как True
            'id': {'read_only': True}        # id только для чтения
        }

    def create(self, validated_data):
        if 'password' in validated_data:
            validated_data['password'] = make_password(validated_data['password'])
        return super().create(validated_data)

    def update(self, instance, validated_data):
        if 'password' in validated_data:
            validated_data['password'] = make_password(validated_data['password'])
        # Удаляем id из validated_data если он есть
        validated_data.pop('id', None)
        return super().update(instance, validated_data)

class WordSerializer(serializers.ModelSerializer):
    class Meta:
        model = Word
        fields = '__all__'

class SuggestionSerializer(serializers.ModelSerializer):
    class Meta:
        model = Suggestion
        fields = '__all__'