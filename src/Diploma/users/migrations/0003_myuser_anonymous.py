# Generated by Django 3.2.18 on 2024-02-21 18:15

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('users', '0002_auto_20240212_2322'),
    ]

    operations = [
        migrations.AddField(
            model_name='myuser',
            name='anonymous',
            field=models.BooleanField(default=False),
        ),
    ]
