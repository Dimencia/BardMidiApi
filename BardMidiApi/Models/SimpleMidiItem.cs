namespace BardMidiApi.Models
{
    // This class is what is returned to API users when they request a MIDI Item
    // It simplifies the Author into an AuthorName, 
    // And does not expose the Id of either the Author or MidiItem
    public class SimpleMidiItem
    {
        public string? Hash { get; set; } 
        public string? Name { get; set; }
        public string? AuthorName { get; set; }
        public int? Score { get; set; }
        public string? DownloadUrl { get; set; }
        public string? AuthorNotes { get; set; }
        public DateTimeOffset UploadDate { get; set; }

        // Easy conversion from full to simple
        public SimpleMidiItem(MidiItem midi)
        {
            // We could use a mapper, but we have custom mapping needed for the name
            this.Hash = midi.Hash;
            this.Name = midi.Name;
            this.AuthorName = midi.Author?.DisplayName ?? string.Empty;
            this.Score = midi.Score;
            this.DownloadUrl = midi.DownloadUrl;
            this.AuthorNotes = midi.AuthorNotes;
            this.UploadDate = midi.UploadDate;
        }
    }
}
