using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models
{
    public class Comments
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Пользователь")]
        [Required]
        //[Required(ErrorMessage = "Поле 'Пользователь' обязательно для заполнения")]
        public string username { get; set; }
        [Display(Name = "Комментарий")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Поле 'Комментарий' обязательно для заполнения")]
        public string description { get; set; }
        [Display(Name = "Дата публикации")]
        public DateTime date { get; set; }
        public int? albumsId { get; set; }
        public Albums? albums { get; set; }
    }
}
