using BardMidiApi.Interfaces;
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
        public int Id { get; set; }
        // We expose a hash; this is the md5 hash of the file, with a triple purpose of acting to ensure the same file isn't included twice
        // And also protecting against MITM attacks, and acting as the primary key exposed to the user
        // This makes bruteforcing entries harder because they aren't sequential, and gives no correlation
        // I.e. it doesn't tell them that they're downloading midi 20383 (giving away info about how many midis we have)
        // It's mostly a moot point in this situation but, for posterity.  
        public long Hash { get; set; }
        public MidiUser? Author { get; set; } // Links to Users table
        public string? Name { get; set; }
        public int? Score { get; set; }
        public string? DownloadUrl { get; set; }

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
