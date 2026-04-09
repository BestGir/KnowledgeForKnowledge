# API Documentation — KnowledgeForKnowledge

**Base URL:** `http://localhost:5129`  
**Auth:** Bearer JWT в заголовке `Authorization: Bearer <token>`  
**Content-Type:** `application/json` (если не указано иное)

---

## Общие соглашения

### Пагинация

Все списочные ответы возвращают:
```json
{
  "items": [...],
  "totalCount": 42,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

### Коды ошибок

| HTTP | Когда |
|------|-------|
| `400` | Невалидные данные, нарушение бизнес-правил (`{ "errors": {...} }` или `{ "message": "..." }`) |
| `401` | Не передан / невалидный JWT |
| `403` | Нет прав (чужой ресурс) |
| `404` | Сущность не найдена |
| `500` | Внутренняя ошибка |

### Енумы (числовые значения)

```
RequestStatus:           0=Open, 1=Fulfilled, 2=Closed, 3=OnHold
ApplicationStatus:       0=Pending, 1=Accepted, 2=Rejected
DealStatus:              0=Active, 1=CompletedByInitiator, 2=CompletedByPartner, 3=Completed, 4=Cancelled
SkillLevel:              0=Trainee, 1=Junior, 2=Middle, 3=Senior
SkillEpithet:            0=IT, 1=Design, 2=Cooking, 3=Language, 4=Music, 5=Sports, 6=Business, 7=Education, 8=Healthcare, 9=Other
VerificationRequestType: 0=SkillVerify, 1=AccountVerify
VerificationStatus:      0=Pending, 1=Approved, 2=Rejected
NotificationType:        0=NewApplication, 1=ApplicationAccepted, 2=ApplicationRejected,
                         3=DealCreated, 4=DealCompleted, 5=DealCancelled,
                         6=NewReview, 7=VerificationApproved, 8=VerificationRejected
```

---

## AUTH `/api/auth`

### `POST /api/auth/login`
Вход. Если к аккаунту привязан Telegram — возвращает сессию для 2FA вместо токена.

**Body:**
```json
{ "login": "user@example.com", "password": "secret" }
```

**Ответ 200 — без 2FA:**
```json
{ "token": "eyJ...", "accountId": "uuid", "isAdmin": false, "requiresOtp": false, "sessionId": null }
```

**Ответ 200 — с 2FA (Telegram привязан):**
```json
{ "token": "", "accountId": "uuid", "isAdmin": false, "requiresOtp": true, "sessionId": "hex32" }
```

**Ошибки:**
- `401` — неверный логин/пароль, аккаунт деактивирован, аккаунт заблокирован (в сообщении — минуты до разблокировки)

---

### `POST /api/auth/verify-otp`
Второй шаг 2FA — ввод кода из Telegram.

**Body:**
```json
{ "sessionId": "hex32", "code": "123456" }
```

**Ответ 200:**
```json
{ "token": "eyJ..." }
```

**Ошибки:**
- `400` — неверный код (максимум 5 попыток, потом сессия блокируется), сессия истекла (TTL 5 мин)

---

### `POST /api/auth/forgot-password`
Запрос сброса пароля через Telegram. Ответ одинаков независимо от того, существует ли аккаунт.

**Body:**
```json
{ "login": "user@example.com" }
```

**Ответ 200:**
```json
{ "sessionId": "hex32" }
```

> Если аккаунт не найден или Telegram не привязан — `sessionId: ""`. Намеренно, защита от перебора.

---

### `POST /api/auth/reset-password`
Сброс пароля по коду из Telegram.

**Body:**
```json
{ "sessionId": "hex32", "code": "123456", "newPassword": "newSecret123" }
```

**Ответ 204** — пароль изменён.

**Ошибки:**
- `400` — неверный код, сессия истекла (TTL 10 мин)

---

## ACCOUNTS `/api/accounts`

### `POST /api/accounts`
Регистрация нового аккаунта. Публичный.

**Body:**
```json
{ "login": "user@example.com", "password": "secret123" }
```

**Ответ 201:** `"uuid"` (ID нового аккаунта)

**Ошибки:**
- `400` — логин занят, пароль слишком короткий

---

### `GET /api/accounts/me` 🔒
Данные текущего авторизованного пользователя.

**Ответ 200:**
```json
{
  "accountID": "uuid",
  "login": "user@example.com",
  "telegramID": null,
  "isAdmin": false,
  "isActive": true,
  "createdAt": "2025-01-01T00:00:00Z"
}
```

---

### `GET /api/accounts/{id}` 🔒
Аккаунт по ID.

**Ответ 200:** Тот же формат что `/me`.  
**Ошибки:** `404`

---

### `GET /api/accounts?search=&page=1&pageSize=20` 🔒 Admin only
Список всех аккаунтов с поиском по логину.

**Ответ 200:** Пагинированный список `AccountDto`.

---

### `PUT /api/accounts/{id}` 🔒
Обновить TelegramID вручную.

**Body:**
```json
{ "telegramID": "123456789" }
```

**Ответ 204**. Только свой аккаунт (`403` иначе).

---

### `PUT /api/accounts/{id}/password` 🔒
Смена пароля.

**Body:**
```json
{ "currentPassword": "old", "newPassword": "new123" }
```

**Ответ 204**. Только свой аккаунт.

**Ошибки:**
- `400` — неверный текущий пароль
- `403` — чужой аккаунт

---

### `DELETE /api/accounts/{id}` 🔒
Деактивировать аккаунт (soft delete). Пользователь — только свой, Admin — любой.

**Ответ 204**.

---

### `PUT /api/accounts/{id}/activate` 🔒 Admin only
Реактивировать деактивированный аккаунт.

**Ответ 204**.

---

## USER PROFILES `/api/userprofiles`

> Весь раздел требует JWT (`[Authorize]` на контроллере), кроме GET профиля.

### `GET /api/userprofiles/{accountId}`
Профиль пользователя. Если профиль не создан — возвращает частичный объект из данных аккаунта (`hasProfile: false`).  
Поле `contactInfo` видно только самому пользователю и Admin.

**Ответ 200:**
```json
{
  "accountID": "uuid",
  "fullName": "Ivan Ivanov",
  "dateOfBirth": "1995-05-20T00:00:00Z",
  "photoURL": "/uploads/photos/uuid_abc.jpg",
  "contactInfo": "vk.com/ivan",
  "description": "Python developer",
  "isActive": true,
  "lastSeenOnline": "2025-04-01T10:00:00Z",
  "hasProfile": true
}
```

> Для анонимного запроса `contactInfo` будет `null`.

**Ошибки:**
- `404` — аккаунт не существует вовсе

---

### `PUT /api/userprofiles` 🔒
Создать или обновить свой профиль (upsert).

**Body:**
```json
{
  "fullName": "Ivan Ivanov",
  "dateOfBirth": "1995-05-20",
  "photoURL": null,
  "contactInfo": "vk.com/ivan",
  "description": "Python developer"
}
```

**Ответ 204**.

---

### `POST /api/userprofiles/photo` 🔒
Загрузить фото профиля. `multipart/form-data`, поле `photo`.

**Ограничения:** JPEG / PNG / WebP, макс 5 МБ.

**Ответ 200:**
```json
{ "photoUrl": "/uploads/photos/uuid_abc.jpg" }
```

**Ошибки:**
- `400` — файл не выбран, неверный формат, превышен размер

---

## SKILLS `/api/skills`

### `GET /api/skills?search=&epithet=&page=1&pageSize=20`
Каталог навыков. Публичный.

**Query params:**
- `search` — строка поиска по названию
- `epithet` — числовое значение `SkillEpithet` (0–9)

**Ответ 200:**
```json
{
  "items": [
    { "skillID": "uuid", "skillName": "Python", "epithet": 0 }
  ],
  "totalCount": 50, "page": 1, "pageSize": 20, "totalPages": 3
}
```

---

### `POST /api/skills` 🔒 Admin only

**Body:**
```json
{ "skillName": "Rust", "epithet": 0 }
```

**Ответ 201:** `{ "id": "uuid" }`

---

### `DELETE /api/skills/{id}` 🔒 Admin only

**Ответ 204**.

---

## SKILL OFFERS `/api/skilloffers`

### `GET /api/skilloffers?skillId=&accountId=&isActive=&search=&page=1&pageSize=20`
Список предложений. Публичный. Сортировка: новые первые.

**Query params:**
- `skillId` — фильтр по навыку
- `accountId` — фильтр по автору
- `isActive` — `true` / `false`
- `search` — поиск по заголовку, описанию, названию навыка

**Ответ 200:**
```json
{
  "items": [
    {
      "offerID": "uuid",
      "accountID": "uuid",
      "authorName": "Ivan Ivanov",
      "authorPhotoURL": "/uploads/photos/...",
      "skillID": "uuid",
      "skillName": "Python",
      "title": "Обучу Python",
      "details": "Базовый курс за 4 занятия",
      "isActive": true
    }
  ],
  "totalCount": 10, "page": 1, "pageSize": 20, "totalPages": 1
}
```

---

### `GET /api/skilloffers/{id}`
Карточка одного предложения. Публичный.

**Ответ 200:** Тот же формат что в списке.  
**Ошибки:** `404`

---

### `POST /api/skilloffers` 🔒
Создать предложение. Требует заполненного профиля.

**Body:**
```json
{
  "skillID": "uuid",
  "title": "Обучу Python",
  "details": "Базовый курс за 4 занятия"
}
```

**Ответ 201:** `{ "id": "uuid" }`

**Ошибки:**
- `400` — профиль не заполнен, `skillID` не найден в каталоге

---

### `PUT /api/skilloffers/{id}` 🔒
Обновить своё предложение.

**Body:**
```json
{
  "title": "Новый заголовок",
  "details": "Новое описание",
  "isActive": false
}
```

**Ответ 204**. Только владелец (`403` иначе).

---

### `DELETE /api/skilloffers/{id}` 🔒
Удалить своё предложение.

**Ответ 204**. Только владелец (`403` иначе).

---

## SKILL REQUESTS `/api/skillrequests`

### `GET /api/skillrequests?accountId=&status=&page=1&pageSize=20`
Список запросов на обучение. Публичный. Сортировка: новые первые.

**Ответ 200:**
```json
{
  "items": [
    {
      "requestID": "uuid",
      "accountID": "uuid",
      "authorName": "Ivan Ivanov",
      "authorPhotoURL": null,
      "skillID": "uuid",
      "skillName": "Python",
      "title": "Ищу репетитора по Python",
      "details": "Нужно с нуля",
      "status": 0
    }
  ],
  "totalCount": 4, "page": 1, "pageSize": 20, "totalPages": 1
}
```

---

### `GET /api/skillrequests/{id}`
Карточка одного запроса. Публичный.

**Ответ 200:** Тот же формат что в списке.  
**Ошибки:** `404`

---

### `POST /api/skillrequests` 🔒
Создать запрос. Требует заполненного профиля.

**Body:**
```json
{
  "skillID": "uuid",
  "title": "Ищу репетитора по Python",
  "details": "Нужно с нуля"
}
```

**Ответ 201:** `{ "id": "uuid" }`

**Ошибки:**
- `400` — профиль не заполнен, `skillID` не найден в каталоге

---

### `PUT /api/skillrequests/{id}` 🔒
Изменить статус своего запроса.

**Body:**
```json
{ "status": 2 }
```

> Допустимые значения: `0=Open`, `2=Closed`, `3=OnHold`.  
> Статус `1=Fulfilled` устанавливается системой автоматически при принятии заявки.

**Ответ 204**.

---

### `DELETE /api/skillrequests/{id}` 🔒
Удалить свой запрос.

**Ответ 204**. Только автор (`403` иначе).

---

## USER SKILLS `/api/userskills`

### `GET /api/userskills/{accountId}` 🔒
Навыки пользователя с уровнями.

**Ответ 200:**
```json
[
  { "skillID": "uuid", "skillName": "Python", "epithet": 0, "level": 2 }
]
```

---

### `POST /api/userskills` 🔒
Добавить навык себе.

**Body:**
```json
{ "skillID": "uuid", "level": 2 }
```

**Ответ 204**.

---

### `DELETE /api/userskills/{skillId}` 🔒
Убрать навык из своего профиля.

**Ответ 204**.

---

## APPLICATIONS `/api/applications`

Отклики на предложения и запросы.

### `GET /api/applications/incoming?page=1&pageSize=20` 🔒
Входящие заявки (кто откликнулся на мои предложения/запросы, статус `Pending`).

**Ответ 200:**
```json
{
  "items": [
    {
      "applicationID": "uuid",
      "applicantID": "uuid",
      "applicantName": "Petr Petrov",
      "offerID": "uuid",
      "skillRequestID": null,
      "message": "Хочу с вами обменяться",
      "status": 0,
      "createdAt": "2025-04-01T10:00:00Z"
    }
  ],
  "totalCount": 1, "page": 1, "pageSize": 20, "totalPages": 1
}
```

---

### `GET /api/applications/outgoing?page=1&pageSize=20` 🔒
Мои исходящие отклики. Формат аналогичен `incoming`.

---

### `GET /api/applications/processed?status=&page=1&pageSize=20` 🔒
Обработанные заявки (принятые / отклонённые). Фильтр `status`: `1=Accepted`, `2=Rejected`.

---

### `POST /api/applications` 🔒
Откликнуться на предложение или запрос. Указать ровно одно из двух полей.

**Body:**
```json
{
  "offerID": "uuid",
  "skillRequestID": null,
  "message": "Хочу обменяться"
}
```

**Ответ 201:** `{ "id": "uuid" }`

**Ошибки:**
- `400` — оба поля пусты или оба заполнены, уже существует отклик от этого пользователя
- `400` — нельзя откликнуться на собственное предложение/запрос

---

### `DELETE /api/applications/{id}` 🔒
Отозвать свой отклик (только пока статус `Pending`).

**Ответ 204**.

**Ошибки:**
- `400` — отклик уже обработан
- `403` — чужой отклик

---

### `PUT /api/applications/{id}/respond` 🔒
Принять или отклонить входящую заявку. Только владелец предложения/запроса.

**Body:**
```json
{ "status": 1 }
```

> `1=Accepted` — сделка (`Deal`) создаётся автоматически, оба участника получают уведомление.  
> `2=Rejected` — отклонение, заявитель получает уведомление.

**Ответ 204**.

**Ошибки:**
- `403` — вы не владелец предложения/запроса
- `400` — заявка уже обработана

---

## DEALS `/api/deals`

Сделки создаются автоматически при принятии заявки (`RespondApplication → Accepted`).

### `GET /api/deals?page=1&pageSize=20` 🔒
Мои сделки (все статусы).

**Ответ 200:**
```json
{
  "items": [
    {
      "dealID": "uuid",
      "initiatorID": "uuid",
      "partnerID": "uuid",
      "offerID": "uuid",
      "skillRequestID": null,
      "status": 0,
      "createdAt": "2025-04-01T10:00:00Z",
      "completedAt": null
    }
  ],
  "totalCount": 1, "page": 1, "pageSize": 20, "totalPages": 1
}
```

---

### `GET /api/deals/user/{accountId}?page=1&pageSize=20`
Публичная история завершённых/отменённых сделок пользователя. Публичный.

**Ответ 200:** Тот же формат.

---

### `GET /api/deals/{id}` 🔒
Детали сделки. Доступно только участникам.

**Ответ 200:** Тот же формат.

**Ошибки:**
- `403` — не участник сделки
- `404` — сделка не найдена

---

### `PUT /api/deals/{id}/complete` 🔒
Отметить сделку завершённой со своей стороны.

**Схема двустороннего завершения:**
- Один участник нажал → статус `CompletedByInitiator` (1) или `CompletedByPartner` (2)
- Оба нажали → статус `Completed` (3), рассылаются уведомления

**Ответ 204**.

**Ошибки:**
- `403` — не участник

---

### `PUT /api/deals/{id}/cancel` 🔒
Отменить активную сделку.

**Ответ 204**.

**Ошибки:**
- `403` — не участник
- `400` — сделка уже завершена

---

## REVIEWS `/api/reviews`

### `GET /api/reviews/{accountId}?page=1&pageSize=20`
Отзывы о пользователе. Публичный.

**Ответ 200:**
```json
{
  "items": [
    {
      "reviewID": "uuid",
      "authorID": "uuid",
      "authorName": "Ivan Ivanov",
      "rating": 5,
      "comment": "Отличный преподаватель",
      "createdAt": "2025-04-01T10:00:00Z"
    }
  ],
  "totalCount": 3, "page": 1, "pageSize": 20, "totalPages": 1
}
```

---

### `POST /api/reviews` 🔒
Оставить отзыв. Только по завершённой сделке (`status=Completed`), один раз на сделку.

**Body:**
```json
{ "dealID": "uuid", "rating": 5, "comment": "Отлично!" }
```

> `rating`: целое число от 1 до 5.

**Ответ 201:** `{ "id": "uuid" }`

**Ошибки:**
- `400` — сделка не завершена, отзыв уже оставлен, вы не участник этой сделки

---

## MATCHES `/api/matches`

### `GET /api/matches` 🔒
Умный подбор партнёров. Алгоритм: пересечение моих навыков с чужими запросами и чужих навыков с моими запросами.

**Ответ 200:**
```json
[
  {
    "accountID": "uuid",
    "fullName": "Petr Petrov",
    "photoURL": null,
    "matchScore": 2,
    "theyCanTeachMe": [
      { "skillID": "uuid", "skillName": "Python" }
    ],
    "iCanTeachThem": [
      { "skillID": "uuid", "skillName": "Design" }
    ]
  }
]
```

> `matchScore` — сумма совпадений в обе стороны. Список отсортирован по убыванию.

---

## EDUCATION `/api/education`

### `GET /api/education/{accountId}` 🔒
Список записей об образовании.

**Ответ 200:**
```json
[
  {
    "educationID": "uuid",
    "institutionName": "НИУ ВШЭ",
    "degreeField": "Computer Science",
    "yearCompleted": 2022
  }
]
```

---

### `POST /api/education` 🔒
Добавить запись об образовании.

**Body:**
```json
{
  "institutionName": "НИУ ВШЭ",
  "degreeField": "Computer Science",
  "yearCompleted": 2022
}
```

> `degreeField` и `yearCompleted` — опциональные.

**Ответ 201:** `{ "id": "uuid" }`

---

### `DELETE /api/education/{id}` 🔒
Удалить свою запись об образовании.

**Ответ 204**. Только своя запись (`403` иначе).

---

## PROOFS `/api/proofs`

Подтверждающие файлы к навыкам (дипломы, сертификаты).

### `GET /api/proofs/{accountId}` 🔒
Список файлов-подтверждений пользователя.

**Ответ 200:**
```json
[
  {
    "proofID": "uuid",
    "skillID": "uuid",
    "skillName": "Python",
    "fileURL": "/uploads/proofs/uuid_abc.pdf",
    "uploadedAt": "2025-04-01T10:00:00Z"
  }
]
```

---

### `POST /api/proofs` 🔒
Загрузить документ. `multipart/form-data`.

**Form fields:**
- `file` — файл (JPEG / PNG / WebP / PDF, макс 10 МБ)
- `skillID` _(опционально, guid)_ — привязать к конкретному навыку

**Ответ 201:** `{ "id": "uuid", "fileUrl": "/uploads/proofs/..." }`

**Ошибки:**
- `400` — файл не выбран, неверный формат, превышен размер, достигнут лимит 20 файлов

---

## VERIFICATION `/api/verification`

### `GET /api/verification?accountId=&status=&page=1&pageSize=20` 🔒
Заявки на верификацию.  
Обычный пользователь видит только свои.  
Admin видит все, может фильтровать по `accountId`.

**Ответ 200:**
```json
{
  "items": [
    {
      "verificationRequestID": "uuid",
      "accountID": "uuid",
      "requestType": 0,
      "proofID": "uuid",
      "status": 0,
      "createdAt": "2025-04-01T10:00:00Z"
    }
  ],
  "totalCount": 1, "page": 1, "pageSize": 20, "totalPages": 1
}
```

---

### `POST /api/verification` 🔒
Подать заявку на верификацию.

**Body:**
```json
{ "requestType": 0, "proofID": "uuid" }
```

> `requestType`: `0=SkillVerify`, `1=AccountVerify`. `proofID` — опционально.

**Ответ 201:** `{ "id": "uuid" }`

---

### `PUT /api/verification/{id}/review` 🔒 Admin only
Рассмотреть заявку.

**Body:**
```json
{ "status": 1 }
```

> `1=Approved`, `2=Rejected`

**Ответ 204**.

---

## NOTIFICATIONS `/api/notifications`

### `GET /api/notifications?unreadOnly=false&page=1&pageSize=30` 🔒
Список уведомлений текущего пользователя.

**Ответ 200:**
```json
{
  "items": [
    {
      "notificationID": "uuid",
      "type": 0,
      "message": "Новый отклик на ваше предложение",
      "isRead": false,
      "relatedEntityId": "uuid",
      "createdAt": "2025-04-01T10:00:00Z"
    }
  ],
  "totalCount": 5, "page": 1, "pageSize": 30, "totalPages": 1
}
```

> `relatedEntityId` — ID связанной сущности (заявки, сделки и т.д.), можно использовать для навигации.

---

### `PUT /api/notifications/{id}/read` 🔒
Пометить одно уведомление прочитанным.

**Ответ 204**.

---

### `PUT /api/notifications/read-all` 🔒
Пометить все уведомления прочитанными.

**Ответ 204**.

---

## TELEGRAM `/api/telegram`

### `POST /api/telegram/generate-link-token` 🔒
Сгенерировать токен для привязки Telegram-аккаунта.  
После получения пользователь открывает бота и отправляет `/start TOKEN`.

**Ответ 200:**
```json
{ "token": "ABC123DEF456" }
```

---

### `GET /api/telegram/notifications/settings` 🔒
Текущие настройки Telegram-уведомлений.

**Ответ 200:**
```json
{ "notificationsEnabled": true }
```

---

### `PUT /api/telegram/notifications/settings` 🔒
Включить или отключить Telegram-уведомления.

**Body:**
```json
{ "notificationsEnabled": false }
```

**Ответ 204**.

---

### `POST /api/telegram/webhook`
Вебхук для Telegram Bot API. Вызывается только серверами Telegram. С фронтенда не использовать.

---

## Типовые сценарии

### Регистрация и вход
```
POST /api/accounts                          — регистрация
POST /api/auth/login                        — вход
  → requiresOtp: false  →  токен получен
  → requiresOtp: true   →  POST /api/auth/verify-otp  →  токен получен
```

### Заполнение профиля
```
PUT  /api/userprofiles                      — создать/обновить профиль
POST /api/userprofiles/photo                — загрузить фото
POST /api/userskills                        — добавить навыки
POST /api/education                         — добавить образование
POST /api/proofs                            — загрузить сертификат
```

### Обмен навыками
```
POST /api/skilloffers                       — создать предложение
POST /api/skillrequests                     — создать запрос
POST /api/applications  { offerID }         — откликнуться на предложение
PUT  /api/applications/{id}/respond { 1 }   — принять заявку → Deal создаётся
PUT  /api/deals/{id}/complete               — оба участника отмечают завершение
POST /api/reviews                           — оставить отзыв
```

### Умный поиск
```
GET  /api/matches                           — найти подходящих партнёров
GET  /api/skilloffers?search=python         — поиск по предложениям
GET  /api/skillrequests?skillId=uuid        — запросы по навыку
```

### Привязка Telegram и 2FA
```
POST /api/telegram/generate-link-token      — получить токен
  → пользователь пишет боту /start TOKEN
  → аккаунт привязывается автоматически
  → при следующем входе: POST /api/auth/login → 2FA → POST /api/auth/verify-otp
```

### Сброс пароля
```
POST /api/auth/forgot-password { login }    — получить sessionId
  → в Telegram приходит код
POST /api/auth/reset-password               — сменить пароль
```
