using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace CourseWork.Models
{
    public class Artists
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Исполнитель")]
        [Required(ErrorMessage = "Поле 'Исполнитель' обязательно для заполнения")]
        public string name { get; set; }
        [Display(Name = "Описание")]
        [DataType(DataType.MultilineText)]
        public string? description { get; set; }
        public string? Photo { get; set; }
        public virtual ICollection<Tracks> tracks { get; set; }
        public virtual ICollection<Albums> albums { get; set; }
        public virtual ICollection<Tours> tours { get; set; }
        public Artists()
        {
            tracks = new List<Tracks>();
            albums = new List<Albums>();
            tours = new List<Tours>();
        }
    }
}
