using BardMidiApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BardMidiApi.Services
{
    public class BardApiService
    {
        // We might need this for something?  Let's find out what by not including it
        //public BardApiService() { context = new MidiContext(); }

        private MidiContext _context;
        public BardApiService(MidiContext context)
        {
            _context = context;

        }
        // TODO: I want to return IEnumerable, but I think I need to ToList in order to actually execute the query?  
        // Should I pass them the enumerable and let them do what they want with it?  
        public async Task<List<SimpleMidiItem>> GetMidiItems()
        {
            return await _context.MidiItems.Include(m => m.Author).Select(m => new SimpleMidiItem(m)).ToListAsync();
        }

        public async Task<SimpleMidiItem> GetMidiItem(string hash)
        {
            return await _context.MidiItems.Where(m => m.Hash == hash).Include(m => m.Author).Select(m => new SimpleMidiItem(m)).SingleOrDefaultAsync();
        }
        // I guess we'll return the number of rows changed, if we're not returning an item
        public async Task<int> UpdateMidiItem(MidiItem midiItem)
        {
            // We are requiring that only the original author can update a midi
            var existingMidi = await _context.MidiItems.Include(m => m.Author).Where(m => m.Hash == midiItem.Hash && m.Author.ServiceId == midiItem.Author.ServiceId).SingleOrDefaultAsync();
            if (existingMidi == null)
            {
                return 0;
            }

            // Detach change tracking from existingMidi and its Author, so that we can mark the new objects as 'updates' to those objects
            _context.ChangeTracker.Clear();
            midiItem.Id = existingMidi.Id;
            midiItem.Author.Id = existingMidi.Author.Id;
            _context.Entry(midiItem).State = EntityState.Modified;
            _context.Entry(midiItem.Author).State = EntityState.Modified;

            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MidiItemExists(midiItem.Hash))
                {
                    return 0;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<int> AddMidiItem(MidiItem midiItem)
        {
            if (midiItem == null || _context.MidiItems.Any(m => m.Hash == midiItem.Hash) || midiItem.Author == null)
            {
                return 0;
            }

            // Because we don't yet have Author add endpoints and it'd be awkward dealing with FKs over REST
            // We'll just create the user if it doesn't exist
            var existingUser = await _context.Users.Where(u => u.ServiceId == midiItem.Author.ServiceId).SingleOrDefaultAsync();
            if (existingUser == null)
            {
                existingUser = midiItem.Author;
                _context.Add(existingUser);
            }
            midiItem.Author = existingUser;

            // TODO: Confirm the hash - we shouldn't just trust them on it, probably.  It's a minor security concern that would help with MITM attacks uploading virus-mids
            // But if we enforce https, MITM is basically impossible unless the attacker owns the network.  
            // Otherwise it hardly matters, it's just an ID
            // And computing hashes could be a bit computationally complex if we're not careful

            _context.Add(midiItem);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteMidiItem(string hash)
        {
            var midiItem = await _context.MidiItems.Where(m => m.Hash == hash).SingleOrDefaultAsync();
            if (midiItem == null)
            {
                return 0;
            }

            _context.MidiItems.Remove(midiItem);
            return await _context.SaveChangesAsync();
        }

        private bool MidiItemExists(string hash)
        {
            return _context.MidiItems.Any(e => e.Hash == hash);
        }
    }
}
