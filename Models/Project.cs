namespace WatchTimeTracker.Models 
{
    /// \brief Модель проекта
    /// \details Хранит проект, при необходимости привязанный к клиенту, и содержит задачи
    public class Project
    {
         /// \brief Идентификатор проекта (Первичный ключ)
         public int Id { get; set; }

         /// \brief Идентификатор клиента, которому принадлежит проект (Внешний ключ)
         /// \warning Если проект не привязан к клиенту
         public int? ClientId { get; set; }

         /// \brief Название проекта
         public string Name { get; set; }

         /// \brief Описание проекта
         public string Description { get; set; }

         /// \brief Цвет проекта 
         public string Color { get; set; } = "#3B82F6";

         /// \brief Ставка проекта
         public double HourlyRate { get; set; }

         /// \brief Статус проекта
         /// \details Возможные значения: "active" или "archived" 
         public string Status { get; set; } = "active";

         /// \brief Дата и время создания проекта (хранится в UTC)
         public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
     }
}

