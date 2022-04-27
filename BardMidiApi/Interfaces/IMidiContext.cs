using BardMidiApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BardMidiApi.Interfaces
{ // Unused... might have made swapping easier.  But I think I'm supposed to trim everything out of IDbContext that I'm not using, so it's easier to implement a mock
    // But tbh that's just unit testing EFCore, we should probably assume that it works.... unless we update it, I guess.... hmmm
    // That is actually a good thing to test.
    public interface IMidiContext : IDisposable
    { 
        // Should these point at the interfaces...?  I think yes.  That's our contract, it has no functions, this is what comes from the db
        // I guess not, code-first won't accept it.
        public DbSet<MidiItem> MidiItems { get; set; }
        public DbSet<MidiUser> Users { get; set; }
        EntityEntry Entry(object entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
