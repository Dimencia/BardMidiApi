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
    // Does not include any deserialization to avoid reliance on Newtonsoft
    public class ClientTests
    {
        // Tests getting all midis, and ensures that the status code is a success, and the results aren't empty
        [Test]
        public async Task HttpGetAllMidiTest_ShouldHaveContent()
        {
            using var client = Tests.factory.CreateClient();

            var response = await client.GetAsync("/api/MidiItems");
            response.EnsureSuccessStatusCode();
            var text = await response.Content.ReadAsStringAsync();
            Assert.Greater(text.Length, 2);
        }

        // Tests getting each specific midi in TestMidis and ensures a success code, and non-empty results
        [Test]
        public async Task HttpGetSpecificMidiTest_ShouldHaveContent()
        {
            using var client = Tests.factory.CreateClient();

            foreach (var midi in Tests.testMidis)
            {
                var response = await client.GetAsync($"/api/MidiItems/{midi.Hash}");
                response.EnsureSuccessStatusCode();
                var text = await response.Content.ReadAsStringAsync();
                Assert.Greater(text.Length, 2);
            }
        }

        // Tests updating each test midi and user with a small change, ensures success code
        // Does rely on Newtonsoft to serialize
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
