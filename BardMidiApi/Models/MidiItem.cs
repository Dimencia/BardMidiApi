namespace BardMidiApi.Models
{
    public class MidiItem
    {
        public long Id { get; set; }
        public long Hash { get; set; } // Technically could collide, so we won't use it as our key
        public string? Name { get; set; }
        public string? Author { get; set; }
        public int? Score { get; set; }
        public string? DownloadUrl { get; set; }
    }
}
