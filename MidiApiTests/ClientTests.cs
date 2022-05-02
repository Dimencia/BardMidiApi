using BardMidiApi.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MidiApiTests
{
    // This tests the HttpClient and end functionality, as a client
    public class ClientTests

    // TODO: The first two tests fail when running the entire set; something in ApiTests seems to be interfering or modifying something
    // Works fine if running just ClientTests
    {
        // Tests getting all midis, ensures the results match our test set
        [Test]
        public async Task HttpGetAllMidisTest()
        {
            using var client = Tests.factory.CreateClient();

            var response = await client.GetAsync("/api/MidiItems");
            response.EnsureSuccessStatusCode();
            var text = await response.Content.ReadAsStringAsync();
            var midiItems = JsonConvert.DeserializeObject<List<SimpleMidiItem>>(text);
            foreach (var midiItem in midiItems)
            {
                var initialMidi = Tests.testMidis.Where(m => m.Hash == midiItem.Hash).Single();
                Assert.IsTrue(Tests.AllValuesEqual(new SimpleMidiItem(initialMidi), midiItem));
            }
            Assert.AreEqual(Tests.testMidis.Count, midiItems.Count);
        }

        // Tests getting each specific midi in TestMidis, ensures the fields match each test midi
        [Test]
        public async Task HttpGetSpecificMidiTest()
        {
            using var client = Tests.factory.CreateClient();

            foreach (var midi in Tests.testMidis)
            {
                var response = await client.GetAsync($"/api/MidiItems/{midi.Hash}");
                response.EnsureSuccessStatusCode();
                var text = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleMidiItem>(text);
                Assert.IsTrue(Tests.AllValuesEqual(result, new SimpleMidiItem(midi)));
            }
        }

        // Tests updating each test midi and user with a small change, ensures success code... 
        // Verifying further would require the DbContext, which we don't have access to here and we test elsewhere...
        // Unless we also then do a GetSpecificMidiTest, but because these are made with transactions, it wouldn't give us results
        // Should this function in the controller and/or service maybe return the updated record?
        [Test]
        public async Task HttpUpdateMidiTest_ShouldGiveSuccess()
        {
            using var client = Tests.factory.CreateClient();

            foreach (var midi in Tests.testMidis)
            {
                var newMidi = new MidiItem()
                {
                    Author = midi.Author,
                    //Id = midi.Id, // Don't send an ID probably
                    AuthorNotes = midi.AuthorNotes,
                    DownloadUrl = midi.DownloadUrl,
                    Hash = midi.Hash,
                    Name = midi.Name + " Modified",
                    Score = midi.Score,
                    UploadDate = midi.UploadDate
                };

                newMidi.Author.DisplayName = "Modified Author";
                newMidi.Author.ServiceId = midi.Author.ServiceId;
                newMidi.Author.Id = null;
                newMidi.Id = null;

                var response = await client.PutAsync($"/api/MidiItems", new StringContent(JsonConvert.SerializeObject(newMidi), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }

        // Tests adding a new Midi and Author, ensures success code
        [Test]
        public async Task HttpPostMidiTest_ShouldGiveSuccess()
        {
            using var client = Tests.factory.CreateClient();

            foreach (var midi in Tests.testMidis)
            {
                var newMidi = new MidiItem()
                {
                    Author = midi.Author,
                    //Id = midi.Id, // Don't send an ID probably
                    AuthorNotes = midi.AuthorNotes,
                    DownloadUrl = midi.DownloadUrl,
                    Hash = midi.Hash.GetHashCode().ToString(), // Should pretty much avoid collisions... 
                    Name = midi.Name + " New Hash",
                    Score = midi.Score,
                    UploadDate = midi.UploadDate
                };

                newMidi.Author.DisplayName = "New Author";
                newMidi.Author.ServiceId = (ulong)newMidi.Author.ServiceId.GetHashCode();
                newMidi.Author.Id = null;

                var response = await client.PostAsync($"/api/MidiItems", new StringContent(JsonConvert.SerializeObject(newMidi), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }

        // TODO: Combined test; add a new set of items, read them, edit them, read them, delete them, read results
    }
}
