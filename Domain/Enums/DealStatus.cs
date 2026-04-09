namespace Domain.Enums;

public enum DealStatus
{
    Active,                  // Идёт обмен
    CompletedByInitiator,    // Инициатор отметил завершение
    CompletedByPartner,      // Партнёр отметил завершение
    Completed,               // Оба отметили — сделка завершена
    Cancelled                // Отменена одной из сторон
}
