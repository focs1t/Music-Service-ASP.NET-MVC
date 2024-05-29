using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace CourseWork.Models
{
    public class Genres
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Жанр")]
        [Required(ErrorMessage = "Поле 'Жанр' обязательно для заполнения")]
        public string name { get; set; }
        public virtual ICollection<Tracks> tracks { get; set; }
        public virtual ICollection<Albums> albums { get; set; }
        public Genres()
        {
            tracks = new List<Tracks>();
            albums = new List<Albums>();
        }
    }
}
