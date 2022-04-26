using BardMidiApi.Interfaces;

namespace BardMidiApi.Models
{
    public class MidiUser
    {
        public int Id { get; set; }
        public long ServiceId { get; set; }
        public string? DisplayName { get; set; }
        public IEnumerable<MidiItem>? MidisContributed { get; set; }
    }
}
