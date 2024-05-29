using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models
{
    public class Tours
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Название тура")]
        [Required(ErrorMessage = "Поле 'Название тура' обязательно для заполнения")]
        public string name { get; set; }
        public string? Photo { get; set; }
        public int? artistsId { get; set; }
        public Artists? artists { get; set; }
        public virtual ICollection<Concerts> concerts { get; set; }
        public Tours()
        {
            concerts = new List<Concerts>();
        }
    }
}
