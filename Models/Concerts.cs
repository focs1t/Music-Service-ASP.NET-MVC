using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models
{
    public class Concerts
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Название тура")]
        [Required(ErrorMessage = "Поле 'Название тура' обязательно для заполнения")]
        public string name { get; set; }
        [Display(Name = "Город")]
        [Required(ErrorMessage = "Поле 'Город' обязательно для заполнения")]
        public string city { get; set; }
        [Display(Name = "Дата проведения")]
        [Required(ErrorMessage = "Поле 'Дата проведения' обязательно для заполнения")]
        public DateTime date { get; set; }
        [Display(Name = "Обложка")]
        public int? toursId { get; set; }
        public Tours? tours { get; set; }
    }
}
