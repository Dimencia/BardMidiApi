namespace BardMidiApi.Models
{
    public class SimpleMidiItem
    {
        public string? Hash { get; set; } 
        public string? Name { get; set; }
        public string? AuthorName { get; set; }
        public int? Score { get; set; }
        public string? DownloadUrl { get; set; }
        public string? AuthorNotes { get; set; }
        public DateTimeOffset UploadDate { get; set; }

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
