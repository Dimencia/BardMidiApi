using BardMidiApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BardMidiApi.Models
{
    public class TestMidiContext : DbContext, IMidiContext
    {
        public TestMidiContext(DbContextOptions<MidiContext> options)
            : base(options)
        {
        }
        // Should these point at the interfaces...?  I think yes.  That's our contract, it has no functions, this is what comes from the db
        // I guess not, code-first won't accept it.
        public DbSet<MidiItem> MidiItems { get; set; } = null!;
        public DbSet<MidiUser> Users { get; set; } = null!;


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
