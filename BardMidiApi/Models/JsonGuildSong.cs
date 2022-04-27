using System.ComponentModel;

namespace BardMidiApi.Models
{
    // The structure that was used to make the json file that we're using to seed the db
    public class JsonGuildSong
    {
        public string? checksum { get; set; }
        [DisplayName("Filename")]
        public string? filename { get; set; }
        [DisplayName("Contributor")]
        public string? contributor { get; set; }
        public ulong contributorId { get; set; }
        public DateTimeOffset submittedAt { get; set; }

        [DisplayName("Mordhau Score")]
        public int m_score { get; set; }
        public string? source_url { get; set; }
        public string? author_notes { get; set; }

    }
}
