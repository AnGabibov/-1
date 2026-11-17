namespace TimeTrackWatch.Models
{
       /// \brief Модель задачи
       /// \details Задача принадлежит одному\конктреному проекту и содержит записи времени
       public class TaskItem
       {
            /// \brief Идентификатор уникальной задачи (Первичный ключ)
            public int id { get; set; }

            /// \brief Идентификатор проекта, к которому относится задача (Внешний ключ)
            /// \warning При удалении проекта удаляются все его задачи (CASCADE)
            public int ProjectId { get; set; }

            /// \brief Название задачи 
            public string Name { get; set; }

            /// \brief Описание задачи 
            public string Description { get; set; }

            /// \brief Статус задачи 
            /// \details Возможные значения: "open" 
            public string Status { get; set; } = "open";

            /// \brief Дата и время создания задачи
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       }
}
