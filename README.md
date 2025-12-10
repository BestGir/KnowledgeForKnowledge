# KnowledgeForKnowledge

Сервис для обмена знаниями и навыками между пользователями.

## Архитектура

Проект построен на основе чистой архитектуры (Clean Architecture) и состоит из следующих слоев:

- **Domain** - Доменный слой с сущностями, перечислениями и интерфейсами
- **Application** - Слой приложения с MediatR командами/запросами и FluentValidation валидаторами
- **Infrastructure** - Слой инфраструктуры с EF Core, PostgreSQL и репозиториями
- **API** - Слой представления с контроллерами (все запросы проходят через MediatR)

## Технологии

- .NET 9.0
- ASP.NET Core
- MediatR (CQRS паттерн)
- FluentValidation
- Entity Framework Core
- PostgreSQL

## Структура базы данных

Проект включает следующие таблицы:
- Accounts (Аккаунты пользователей)
- UserProfiles (Профили пользователей)
- SkillsCatalog (Каталог навыков)
- UserSkills (Навыки пользователя)
- Education (Образование)
- Proofs (Подтверждения навыков)
- SkillOffers (Предложения услуг)
- SkillRequests (Запросы услуг)
- VerificationRequests (Запросы на верификацию)

## Запуск проекта

### Через Docker Compose

```bash
docker-compose up -d
```

### Локально

1. Убедитесь, что PostgreSQL запущен
2. Обновите строку подключения в `API/appsettings.json`
3. Примените миграции:
   ```
   dotnet ef database update --project Infrastructure --startup-project API
   ```
4. Запустите API:
   ```
   dotnet run --project API
   ```
   
   API будет доступен по адресу: `https://localhost:5001` или `http://localhost:5000`

## Миграции

Создание новой миграции:
```
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API
```

Применение миграций:
```
dotnet ef database update --project Infrastructure --startup-project API
```

## Настройка Git

Проект настроен для работы с Git. Для инициализации репозитория:

```powershell
git init
git add .
git commit -m "Initial commit: Clean Architecture setup"
```

### Что настроено:
- `.gitignore` - игнорирует файлы сборки, IDE, временные файлы
- `.gitattributes` - настройки окончаний строк и обработки файлов
- Игнорируются `appsettings.Development.json` и другие конфиденциальные файлы

