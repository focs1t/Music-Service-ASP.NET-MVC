using CourseWork.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { 

        }

        public DbSet<Albums> Albums { get; set; }
        public DbSet<Artists> Artists { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Concerts> Concerts { get; set; }
        public DbSet<Genres> Genres { get; set; }
        public DbSet<Playlists> Playlists { get; set; }
        public DbSet<Tours> Tours { get; set; }
        public DbSet<Tracks> Tracks { get; set; }
        public DbSet<TracksPlaylists> TracksPlaylists { get; set; }

        /*protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Albums>()
            .HasOne(a => a.genres)
            .WithMany(g => g.albums)
            .HasForeignKey(a => a.genresId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Albums>()
            .HasOne(a => a.artists)
            .WithMany(g => g.albums)
            .HasForeignKey(a => a.artistsId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comments>()
            .HasOne(c => c.albums)
            .WithMany(a => a.comments)
            .HasForeignKey(c => c.albumsId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Concerts>()
            .HasOne(c => c.tours)
            .WithMany(t => t.concerts)
            .HasForeignKey(c => c.toursId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tours>()
            .HasOne(t => t.artists)
            .WithMany(a => a.tours)
            .HasForeignKey(t => t.artistsId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tracks>()
            .HasOne(a => a.genres)
            .WithMany(g => g.albums)
            .HasForeignKey(a => a.genresId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tracks>()
            .HasOne(a => a.albums)
            .WithMany(g => g.albums)
            .HasForeignKey(a => a.albumsId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Tracks>()
            .HasOne(a => a.artists)
            .WithMany(g => g.albums)
            .HasForeignKey(a => a.artistsId)
            .OnDelete(DeleteBehavior.Cascade);
        }*/
    }
}
