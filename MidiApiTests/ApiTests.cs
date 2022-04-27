using BardMidiApi.Controllers;
using BardMidiApi.Models;
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
    // The test database is populated and we have testUsers and testMidis
    public class ApiTests
    {
        [Test]
        public async Task ApiAddTest_ShouldMatch()
        {
            using var context = Tests.CreateContext();

            var controller = new MidiItemsController(context);

            var testUser = new MidiUser() { DisplayName = "New Test User", ServiceId = 18721294887124 };
            var testMidi = new MidiItem() { Author = testUser, DownloadUrl = "https://newUrl.com", Hash = "abcdd5", Name = "asdf test 5midi", Score = 1, AuthorNotes = "Nffffot5es", UploadDate = DateTime.Now - TimeSpan.FromHours(50) };
            await controller.PostMidiItem(testMidi);

            context.ChangeTracker.Clear();

            var midi = context.MidiItems.Include(m => m.Author).Where(m => m.Hash == testMidi.Hash).Single();
            Assert.IsTrue(Tests.AllValuesEqual(midi, testMidi));
        }

        [Test]
        public async Task ApiGetSpecificTest_ShouldMatch()
        {
            using var context = Tests.CreateContext();
            var controller = new MidiItemsController(context);

            foreach (var midi in Tests.testMidis)
            {
                var result = (await controller.GetMidiItem(midi.Hash)).Value;
                Assert.IsNotNull(result);
                Assert.True(Tests.AllValuesEqual(result, new SimpleMidiItem(midi)));
            }
        }

        [Test]
        public async Task ApiGetAllTest_ShouldMatch()
        {
            using var context = Tests.CreateContext();
            var controller = new MidiItemsController(context);

            var results = (await controller.GetMidiItems()).Value;
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
    }
}
