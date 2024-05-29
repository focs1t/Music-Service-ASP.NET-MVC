using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models
{
    public class Tracks
    {
        [Key]
        public int id { get; set; }
        [Display(Name = "Название трека")]
        [Required(ErrorMessage = "Поле 'Название трека' обязательно для заполнения")]
        public string name { get; set; }
        [Display(Name = "Дата публикации")]
        public DateTime date { get; set; } = DateTime.Now;
        [Display(Name = "Файл")]
        //[Required(ErrorMessage = "Поле 'Файл' обязательно для заполнения")]
        public string? file { get; set; }
        public int? genresId { get; set; }
        public Genres? genres { get; set; }
        public int? albumsId { get; set; }
        public Albums? albums { get; set; }
        public int? artistsId { get; set; }
        public Artists? artists { get; set; }
        /*public virtual ICollection<Playlists> playlists { get; set; }
        public Tracks()
        {
            playlists = new List<Playlists>();
        }*/
    }
}
