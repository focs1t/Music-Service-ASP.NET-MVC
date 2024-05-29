using System.ComponentModel.DataAnnotations;

namespace CourseWork.Models
{
    public class TracksPlaylists
    {
        public int Id { get; set; }

        [Display(Name = "Название трека")]
        public int? tracksId { get; set; }
        public Tracks? tracks { get; set; }

        //public List<Tracks>? tracks { get; set; } = new List<Tracks>();

        [Display(Name = "Название плейлиста")]
        public int? playlistsId { get; set; }
        public Playlists? playlists { get; set; }
    }
}
