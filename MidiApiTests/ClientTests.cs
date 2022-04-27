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
    {
        [Test]
        public async Task HttpGetAllMidiTest_ShouldHaveContent()
        {
            using var client = Tests.factory.CreateClient();

            var response = await client.GetAsync("/api/MidiItems");
            response.EnsureSuccessStatusCode();
            // I'd like to deserialize and test but that would be too much to rely on in a unit test
            // So we'll just make sure it's longer than "[]" - that it contains anything
            var text = await response.Content.ReadAsStringAsync();
            Assert.Greater(text.Length, 2);
        }


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
                newMidi.Author.Id = null; // I have no clue how this ever became not-null

                var response = await client.PostAsync($"/api/MidiItems", new StringContent(JsonConvert.SerializeObject(newMidi), Encoding.UTF8, "application/json"));
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
