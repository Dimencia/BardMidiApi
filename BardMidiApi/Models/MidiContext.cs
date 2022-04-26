using Microsoft.EntityFrameworkCore;

namespace BardMidiApi.Models
{
    public class MidiContext : DbContext
    {
        public MidiContext(DbContextOptions<MidiContext> options)
            : base(options)
        {
        }

        public DbSet<MidiItem> MidiItems { get; set; } = null!;
    }
}
