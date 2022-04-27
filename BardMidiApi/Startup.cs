using BardMidiApi.Models;
using Newtonsoft.Json;

namespace BardMidiApi
{
    public class Startup
    {

        // For now I'm using this just to seed our database
        public static async Task Seed(MidiContext context)
        {
            await context.Database.EnsureCreatedAsync();
            // This is a magic string but it's better to have it here than out there where it means nothing
            // If we change the file, we have to change this func anyway

            // And this isn't supposed to go into production
            // (But I will still use it to seed the server db so I'm not marking it #if debug)
            var knownSongs = JsonConvert.DeserializeObject<Dictionary<string, JsonGuildSong>>(await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory,"knownSongs.json")));

            // If it's null, I'd like it to throw and tell me that it failed, and not continue - this is only for testing, after all

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            foreach (var song in knownSongs.Values)
            {
                // The checksum is given as a string, hexadecimal
                // Convert it to a long... can't.  It's 128 bits, biggest datatype is 64 bits
                // Oh well.  We'll take it as a string and hope for the best (in production, I'd take it as a BINARY(16) value)
                var checksum = song.checksum;
                var existingSong = context.MidiItems.Where(m => m.Hash == checksum).SingleOrDefault();
                var author = context.Users.Where(u => u.ServiceId == song.contributorId).SingleOrDefault();

                if(author == null)
                {
                    author = new MidiUser()
                    {
                        DisplayName = song.contributor,
                        ServiceId = song.contributorId,
                        MidisContributed = new List<MidiItem>()
                    };
                    context.Users.Add(author);
                }

                if(existingSong == null)
                {
                    existingSong = new MidiItem()
                    {
                        Hash = checksum,
                        DownloadUrl = song.source_url,
                        Name = song.filename,
                        Score = song.m_score,
                        UploadDate = song.submittedAt,
                        AuthorNotes = song.author_notes,
                        Author = author
                    };
                    context.MidiItems.Add(existingSong);
                }
                if (author.MidisContributed == null)
                    author.MidisContributed = new List<MidiItem>();
                if (!author.MidisContributed.Any(m => m.Hash == checksum))
                    author.MidisContributed.Add(existingSong);
                // Need to save each time or the changes don't work for next iteration
                await context.SaveChangesAsync();
            }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            Console.WriteLine("Seeded " + knownSongs.Count + " values");
        }
    }
}
