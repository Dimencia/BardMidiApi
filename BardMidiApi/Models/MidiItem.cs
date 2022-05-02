using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BardMidiApi.Models
{
    // This is the base item for database table MidiItems
    // We create non-clustered indexes around the sorted Hash and Score
    // Because we generally query by hash, and sort by score
    [Index(nameof(Hash))]
    [Index(nameof(Score))]
    public class MidiItem
    {
        // We use an int Id to reduce storage & memory used for storing as FK in other tables
        // And because it can be indexed well, when using it for internal operations
        // This is not exposed for security reasons
        // And is nullable so that we can insert objects with no value and have it auto-generate them
        // (Which caused some problems during testing otherwise)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        // We expose a hash; this is the md5 hash of the file, with a triple purpose of acting to ensure the same file isn't included twice
        // And also protecting against MITM attacks, and acting as the primary key exposed to the user
        // This makes bruteforcing entries harder because they aren't sequential, and gives no correlation
        // I.e. it doesn't tell them that they're downloading midi 20383 (giving away info about how many midis we have)
        // It's mostly a moot point in this situation but, for posterity.  

        // Unfortunately, it's 128 bits.  We can store it as BINARY(16) or VARCHAR(32)
        // BINARY is more performant and better at indexing
        // But due to time constraints and for simplicity when comparing values on this end,
        // We'll just use a string.  
        [StringLength(32)]
        public string? Hash { get; set; }
        public MidiUser? Author { get; set; } // Links to Users table
        public string? Name { get; set; }
        public int? Score { get; set; }
        public string? DownloadUrl { get; set; }
        public string? AuthorNotes { get; set; }
        public DateTimeOffset UploadDate { get; set; }

        // Easy way to create a full MidItem out of a simple one and author
        // There is probably a better way to do this, and it's so far unused
        public MidiItem(SimpleMidiItem simple, MidiUser author)
        {
            this.Score = simple.Score;
            this.Author = author;
            this.Name = simple.Name;
            this.Hash = simple.Hash;
            this.DownloadUrl = simple.DownloadUrl;
            this.AuthorNotes = simple.AuthorNotes;
            this.UploadDate = simple.UploadDate;
        }

        public MidiItem(MidiItem other)
        {
            this.Score = other.Score;
            this.Author = other.Author;
            this.Name = other.Name;
            this.Hash = other.Hash;
            this.DownloadUrl = other.DownloadUrl;
            this.AuthorNotes = other.AuthorNotes;
            this.UploadDate = other.UploadDate;
        }

        public MidiItem() { }
    }
}
