using BardMidiApi.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BardMidiApi.Models
{
    public class MidiContext : DbContext
    {
        public MidiContext(DbContextOptions<MidiContext> options)
            : base(options)
        {
        }

        public MidiContext() : base(new DbContextOptions<MidiContext>()) { }

        public virtual DbSet<MidiItem> MidiItems { get; set; } = null!;
        public virtual DbSet<MidiUser> Users { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MidiItem>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<MidiUser>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
            // May not be required to be explicit, but better safe than sorry with the way they change things in .net sometimes
            modelBuilder.Entity<MidiItem>()
                .HasOne(m => m.Author)
                .WithMany(a => a.MidisContributed);
        }
    }
}
