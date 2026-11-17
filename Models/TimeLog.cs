namespace WatchTimeTracker.Models 
{       
        /// \brief Модель записи времени  (Time Log)
        /// \details Содержит информацию  о конкретном отрезке времени, потраченное на задачу.
        /// Если поле EndedAt = null, означает что таймер еще идет.
        public class TimeLog
        {
            /// \brief Идентификатор уникальной записи
            public int Id { get; set; }

            /// \brief Идентификатор задачи, к которой относится запись 
            public int TaskId { get; set; }

            /// \brief Дата и время начала работы
            public DateTime StartedAt { get; set; }

            /// \brief Дата и время окончания работы 
            /// \warning Возможен null, если таймер активен
            public DateTime? EndedAt { get; set; }

            /// \brief Длительность в секундах
            /// \details Вычисляется автоматически при остановке таймера, хранится в БД
            /// для ускорения агрегации данных в отчетах.
            public int? DurationSeconds { get; set; }

            /// \brief Комментарий к записи
            public string Comment { get; set; }

            /// \brief Дата создания записи
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }
}
