using Microsoft.AspNetCore.Mvc;

namespace proctos.Models
{
    public class cloth : Controller
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsFavorite { get; set; }
        public string Image { get; set; } // Новый путь к изображению
        public string ImageOfProduct { get; set; } // Добавьте это, если используется старое имя
        public string NameProduct { get; set; } // Если это требуется отдельно
        public int IdProduct => Id; // Свойство для обратной совместимости

    }
}
