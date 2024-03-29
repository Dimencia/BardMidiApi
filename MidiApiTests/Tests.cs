using BardMidiApi.Controllers;
using BardMidiApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MidiApiTests
{
    // Static class holding pre-load data for all tests to use
    public static class Tests
    {

        private const string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=BardMidiTests;Trusted_Connection=True";
        private static bool _databaseInitialized;


        public static readonly WebApplicationFactory<Program> factory;
        public static readonly List<MidiUser> testUsers;
        public static readonly List<MidiItem> testMidis;


        static Tests()
        {
            using var context = CreateContextWithoutTransaction();

            // Create some arbitrary users and midis
            // TODO: Pull some real data from the production set to be hardcoded here
            testUsers = new List<MidiUser>()
            { 
                new MidiUser() { DisplayName = "Test User", ServiceId = 123874824 }, 
                new MidiUser() { DisplayName = "Test User 2", ServiceId = 123872124 } 
            };

            testMidis = new List<MidiItem>()
            {
                new MidiItem() { Author = testUsers[0], DownloadUrl = "https://no.com", Hash = "abc", Name = "A test midi", Score = 1, AuthorNotes = "Notes", UploadDate = DateTime.Now },
                new MidiItem() { Author = testUsers[1], DownloadUrl = "https://no23.com", Hash = "a2bc", Name = "A tes23t midi", Score = 1, AuthorNotes = "N3otes", UploadDate = DateTime.Now+TimeSpan.FromSeconds(2) },
                new MidiItem() { Author = testUsers[0], DownloadUrl = "https://n44o.com", Hash = "abc5", Name = "A test 5midi", Score = 1, AuthorNotes = "Not5es", UploadDate = DateTime.Now-TimeSpan.FromHours(5) }
            };

            if (!_databaseInitialized)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                foreach (var user in testUsers)
                    context.Add(user);
                foreach (var midi in testMidis)
                    context.Add(midi);


                context.SaveChanges();

                _databaseInitialized = true;
            }

            // The factory is used to test full Client functionality - it can create HttpClients
            factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<MidiContext>(s => CreateContext()); // Use test context and values, which enforces transactions to avoid saving test data
                });
            });
            
        }


        // These might should go somewhere else like in MidiItem - but they're only important for testing
        // In general cases, a MidiItem or User equals another when its key equals another
        public static bool AllValuesEqual(MidiItem a, MidiItem b)
        {
            return a.DownloadUrl == b.DownloadUrl && a.Name == b.Name && a.Score == b.Score && a.Hash == b.Hash && a.AuthorNotes == b.AuthorNotes && a.UploadDate == b.UploadDate
                && AllValuesEqual(a.Author, b.Author);
        }

        public static bool AllValuesEqual(SimpleMidiItem a, SimpleMidiItem b)
        {
            return a.DownloadUrl == b.DownloadUrl && a.Name == b.Name && a.Score == b.Score && a.Hash == b.Hash && a.AuthorNotes == b.AuthorNotes && a.UploadDate == b.UploadDate
                && a.AuthorName == b.AuthorName;
        }

        public static bool AllValuesEqual(MidiUser a, MidiUser b)
        {
            if (a == null && b == null)
                return true;
            if (a == null || b == null)
                return false;
            return a.DisplayName == b.DisplayName && a.ServiceId == b.ServiceId;
        }

        private static MidiContext CreateContextWithoutTransaction()
        {
            return new MidiContext(
                new DbContextOptionsBuilder<MidiContext>()
                    .UseSqlServer(ConnectionString).Options);
        }

        public static MidiContext CreateContext()
        {
            var context = CreateContextWithoutTransaction();
            context.Database.BeginTransaction();
            return context;
        }
    }
}