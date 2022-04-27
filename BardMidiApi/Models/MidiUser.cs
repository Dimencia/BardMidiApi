

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BardMidiApi.Models
{
    // Primary class for Users table
    [Index(nameof(ServiceId))]
    public class MidiUser
    {
        // Much like the MidiItem, the Id is an auto-generating int for performance reasons, that we do not expose for security reasons
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        // And in this case, we expose and use a ulong as the user's lookup key, with a nonclustered index for it
        public ulong ServiceId { get; set; }
        public string? DisplayName { get; set; }
        [JsonIgnore] // Ignore when using JsonConvert to avoid recursion problems
        public List<MidiItem>? MidisContributed { get; set; }
    }
}
