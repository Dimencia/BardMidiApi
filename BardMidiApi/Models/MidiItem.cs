using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BardMidiApi.Models
{
    [Index(nameof(Hash))]
    [Index(nameof(Score))]
    public class MidiItem
    {
        // We use an int Id to reduce storage & memory used for storing as FK in other tables
        // And because it can be indexed well, when using it for internal operations
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        // We expose a hash; this is the md5 hash of the file, with a triple purpose of acting to ensure the same file isn't included twice
        // And also protecting against MITM attacks, and acting as the primary key exposed to the user
        // This makes bruteforcing entries harder because they aren't sequential, and gives no correlation
        // I.e. it doesn't tell them that they're downloading midi 20383 (giving away info about how many midis we have)
        // It's mostly a moot point in this situation but, for posterity.  

        // This is annoying though.  The hash is 128 bits, like any good md5 hash
        // We can't store it in anything except a byte array or string
        // And a byte array makes equality checks awkward.  Oh well.  I guess that's how we have to do it... 
        // I guess for simplicity and finishing this we'll do a string.  A byte array would be more performant I believe, though
        [StringLength(32)]
        public string? Hash { get; set; }
        public MidiUser? Author { get; set; } // Links to Users table
        public string? Name { get; set; }
        public int? Score { get; set; }
        public string? DownloadUrl { get; set; }
        public string? AuthorNotes { get; set; }
        public DateTimeOffset UploadDate { get; set; }

        public MidiItem(SimpleMidiItem simple, MidiUser author)
        {
            this.Score = simple.Score;
            this.Author = author;
            this.Name = simple.Name;
            this.Hash = simple.Hash;
            this.DownloadUrl = simple.DownloadUrl; // I feel like there's a better way to do this
        }

        public MidiItem() { }
    }
}
