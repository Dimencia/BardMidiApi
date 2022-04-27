

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BardMidiApi.Models
{
    public class MidiUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public ulong ServiceId { get; set; }
        public string? DisplayName { get; set; }
        [JsonIgnore]
        public List<MidiItem>? MidisContributed { get; set; }
    }
}
