#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BardMidiApi.Models;
using BardMidiApi.Interfaces;
using BardMidiApi.Services;

namespace BardMidiApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MidiItemsController : ControllerBase
    {
        private readonly BardApiService apiService;

        public MidiItemsController(BardApiService apiService)
        {
            this.apiService = apiService;
        }

        // GET: api/MidiItems/all
        [HttpGet("all/{page?}")]
        public async Task<ActionResult<ApiPaginatedList<SimpleMidiItem>>> GetAllMidiItems(int page = 0)
        {
            var result = await apiService.GetMidiItems(page);
            if (result == null)
                return BadRequest();
            return result;
        }

        // GET: api/MidiItems/52341234234
        [HttpGet("{hash}")]
        public async Task<ActionResult<SimpleMidiItem>> GetMidiItem(string hash)
        {
            var result = await apiService.GetMidiItem(hash);
            if (result == null)
                return BadRequest();
            return result;
        }

        // PUT: api/MidiItems/5632451234123
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut] // TODO: Auth.  For now assume auth is done and the Author of the midiItem is authed
        public async Task<IActionResult> PutMidiItem(MidiItem midiItem)
        {
            if (midiItem == null || midiItem.Author == null)
                return BadRequest();

            var rowsUpdated = await apiService.UpdateMidiItem(midiItem);
            if (rowsUpdated > 0)
                return NoContent(); // This is apparently the correct response
            return NotFound();
        }

        // POST: api/MidiItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost] // TODO: Auth
        public async Task<ActionResult<SimpleMidiItem>> PostMidiItem(MidiItem midiItem)
        {
            var rowsUpdated = await apiService.AddMidiItem(midiItem);
            if (rowsUpdated > 0)
                return CreatedAtAction("GetMidiItem", new { hash = midiItem.Hash }, midiItem);
            return BadRequest();
        }

        // DELETE: api/MidiItems/5123124325
        [HttpDelete("{hash}")] // TODO: Auth
        public async Task<IActionResult> DeleteMidiItem(string hash)
        {
            var rowsUpdated = await apiService.DeleteMidiItem(hash);
            if(rowsUpdated > 0)
                return NoContent();
            return NotFound();
        }

        
    }
}
