#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BardMidiApi.Models;

namespace BardMidiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MidiItemsController : ControllerBase
    {
        private readonly MidiContext _context;

        public MidiItemsController(MidiContext context)
        {
            _context = context;
        }

        // GET: api/MidiItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SimpleMidiItem>>> GetMidiItems()
        {
            // We don't want to return the IDs; they should use the hash as the ID.  So, return a SimpleMidiItem always
            return await _context.MidiItems.Select(m => new SimpleMidiItem(m)).ToListAsync();
        }

        // GET: api/MidiItems/52341234234
        [HttpGet("{hash}")]
        public async Task<ActionResult<SimpleMidiItem>> GetMidiItem(long hash)
        {
            var midiItem = await _context.MidiItems.Where(m => m.Hash == hash).Select(m => new SimpleMidiItem(m)).SingleOrDefaultAsync();

            if (midiItem == null)
            {
                return NotFound();
            }

            return midiItem;
        }

        // PUT: api/MidiItems/5632451234123
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{hash}")] // TODO: Auth
        public async Task<IActionResult> PutMidiItem(long hash, SimpleMidiItem midiItem)
        {
            if (hash != midiItem.Hash)
            {
                return BadRequest();
            }

            _context.Entry(midiItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MidiItemExists(hash))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MidiItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost] // TODO: Auth
        public async Task<ActionResult<SimpleMidiItem>> PostMidiItem(SimpleMidiItem midiItem)
        {
            // Check for duplicate hashes; assume they aren't collisions, but the same file with a different (or even same) name, and reject it
            if (_context.MidiItems.Any(m => m.Hash == midiItem.Hash))
            {
                return BadRequest("Midi already exists");
            }

            // TODO: Confirm the hash - we shouldn't just trust them on it, probably.  It's a minor security concern that would help with MITM attacks uploading virus-mids
            // But if we enforce https, MITM is basically impossible unless the attacker owns the network.  
            // Otherwise it hardly matters, it's just an ID
            // And computing hashes could be a bit computationally complex if we're not careful

            _context.MidiItems.Add(new MidiItem(midiItem, null)); // TODO: AUTH.  Unsure how we get it here, it might already be a db user, or we might have to get it
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMidiItem", new { hash = midiItem.Hash }, midiItem);
        }

        // DELETE: api/MidiItems/5123124325
        [HttpDelete("{hash}")] // TODO: Auth
        public async Task<IActionResult> DeleteMidiItem(long hash)
        {
            var midiItem = await _context.MidiItems.Where(m => m.Hash == hash).SingleOrDefaultAsync();
            if (midiItem == null)
            {
                return NotFound();
            }

            _context.MidiItems.Remove(midiItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MidiItemExists(long hash)
        {
            return _context.MidiItems.Any(e => e.Hash == hash);
        }
    }
}
