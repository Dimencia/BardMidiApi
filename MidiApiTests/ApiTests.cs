using BardMidiApi.Controllers;
using BardMidiApi.Models;
using BardMidiApi.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiApiTests
{
    // This tests the API via directly calling the controller, using a test database on the production server
    // The test database is populated and we have testUsers and testMidis from Tests
    public class ApiTests
    {
        [Test]
        public async Task ApiAddTest()
        {
            using var context = Tests.CreateContext();

            var service = new BardApiService(context);

            var testUser = new MidiUser() { DisplayName = "New Test User", ServiceId = 18721294887124 };
            var testMidi = new MidiItem() { Author = testUser, DownloadUrl = "https://newUrl.com", Hash = "abcdd5", Name = "asdf test 5midi", Score = 1, AuthorNotes = "Nffffot5es", UploadDate = DateTime.Now - TimeSpan.FromHours(50) };
            await service.AddMidiItem(testMidi);

            context.ChangeTracker.Clear();

            var midi = await context.MidiItems.Include(m => m.Author).Where(m => m.Hash == testMidi.Hash).SingleAsync();
            Assert.IsTrue(Tests.AllValuesEqual(midi, testMidi));
        }

        [Test]
        public async Task ApiGetSpecificTest()
        {
            using var context = Tests.CreateContext();
            var service = new BardApiService(context);

            foreach (var midi in Tests.testMidis)
            {
                var result = await service.GetMidiItem(midi.Hash);
                Assert.IsNotNull(result);
                Assert.True(Tests.AllValuesEqual(result, new SimpleMidiItem(midi)));
            }
        }

        [Test]
        public async Task ApiGetAllTest()
        {
            using var context = Tests.CreateContext();
            var service = new BardApiService(context);

            var results = await service.GetMidiItems();
            Assert.IsNotNull(results);
            // Each result should have a pair that matches it
            foreach (var midi in Tests.testMidis)
            {
                var simple = new SimpleMidiItem(midi);
                int numMatches = 0;
                foreach (var midiItem in results)
                {
                    if (Tests.AllValuesEqual(midiItem, simple))
                        numMatches++;
                }
                Assert.AreEqual(numMatches, 1); // Only one match each, no duplicates, not 0
            }
        }

        [Test]
        public async Task ApiUpdateTest()
        {
            using var context = Tests.CreateContext();
            var service = new BardApiService(context);

            foreach(var midi in Tests.testMidis)
            {
                var midiId = await context.MidiItems.Where(m => m.Hash == midi.Hash).Select(m => m.Id).SingleAsync();
                var newMidi = new MidiItem(midi);
                newMidi.Score += 100;
                newMidi.AuthorNotes += " modified";
                newMidi.UploadDate += TimeSpan.FromDays(1);
                newMidi.Name += " modified";
                newMidi.DownloadUrl += " modified";
                newMidi.Author.DisplayName += " modified";
                // The newMidi should be able to have any Id and UpdateMidiItem will fix it to match its hash, since the user doesn't know Id exists
                newMidi.Id = -1;
                var results = await service.UpdateMidiItem(newMidi);
                Assert.AreEqual(2, results); // It should update both the midi and author
                // Retrieve and verify
                var retrieved = await context.MidiItems.Include(m => m.Author).Where(m => m.Hash == newMidi.Hash).SingleAsync();
                Assert.IsTrue(Tests.AllValuesEqual(newMidi, retrieved));
                Assert.AreEqual(midi.Hash, retrieved.Hash);
                Assert.AreEqual(midiId, retrieved.Id);
                // Interesting sidenote... we're in a transaction.  Because we don't ChangeTracker.Clear(), we're only reading back in-memory and not requerying
                // Which is important because this never truly saves to the database
            }

        }

        [Test]
        public async Task ApiDeleteTest()
        {
            using var context = Tests.CreateContext();
            var service = new BardApiService(context);

            foreach (var midi in Tests.testMidis)
            {
                var results = await service.DeleteMidiItem(midi.Hash);
                Assert.AreEqual(results, 1);
                // Retrieve and verify
                var retrieved = await context.MidiItems.Where(m => m.Hash == midi.Hash).CountAsync();
                Assert.AreEqual(retrieved, 0);
                // We are again in a transaction, and these aren't really being deleted
            }

        }
    }
}
