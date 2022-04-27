using BardMidiApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BardMidiApi.Interfaces
{
    // TODO: Build out a TestMidiContext using this, which should test EF operations
    // Particularly to catch errors or discrepancies that might occur when updating EF
    public interface IMidiContext : IDisposable
    {
        public DbSet<MidiItem> MidiItems { get; set; }
        public DbSet<MidiUser> Users { get; set; }
        EntityEntry Entry(object entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
