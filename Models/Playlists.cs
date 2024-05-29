using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace CourseWork.Models
{
    public class Playlists
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Автор")]
        [Required(ErrorMessage = "Поле 'Автор' обязательно для заполнения")]
        public string username { get; set; }
        [Display(Name = "Название плейлиста")]
        [Required(ErrorMessage = "Поле 'Название плейлиста' обязательно для заполнения")]
        public string name { get; set; }
        [Display(Name = "Описание")]
        [DataType(DataType.MultilineText)]
        public string? description { get; set; }
        [Display(Name = "Дата публикации")]
        public DateTime date { get; set; } = DateTime.Now;
        /*public virtual ICollection<Tracks> tracks { get; set; }
        public Playlists()
        {
            tracks = new List<Tracks>();
        }*/
    }
}
