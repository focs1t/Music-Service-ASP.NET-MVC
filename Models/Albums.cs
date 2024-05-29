using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Linq;

namespace CourseWork.Models
{
    public class Albums
    {

        [Key]
        public int id { get; set; }
        [Display(Name = "Название альбома")]
        [Required(ErrorMessage = "Поле 'Название альбома' обязательно для заполнения")]
        public string name { get; set; }
        [Display(Name = "Описание")]
        [DataType(DataType.MultilineText)]
        public string? description { get; set; }
        [Display(Name = "Дата публикации")]
        public DateTime date { get; set; } = DateTime.Now;
        public string? Photo { get; set; }
        public int? genresId { get; set; }
        public Genres? genres { get; set; }
        public int? artistsId { get; set; }
        public Artists? artists { get; set; }
        public virtual ICollection<Tracks> tracks { get; set; }
        public virtual ICollection<Comments> comments { get; set; }
        public Albums()
        {
            tracks = new List<Tracks>();
            comments = new List<Comments>();
        }
    }
}
