using BardMidiApi;
using BardMidiApi.Interfaces;
using BardMidiApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AnyOrigin", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers().AddNewtonsoftJson(x => 
    x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MidiContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);


// TODO: Auth - Discord OAuth, or custom OAuth

//builder.Services.AddAuthentication()
//    .AddDiscord(x =>
//    {
//        x.AppId = builder.Configuration["Discord:AppId"];
//        x.AppSecret = builder.Configuration["Discord:AppSecret"];
//        foreach(var scope in builder.Configuration["Discord:Scopes"].Split(','))
//            x.Scope.Add(scope);
//    });
//
//builder.Services.AddRazorPages();

var app = builder.Build();

// Seeding from json file
//app.Lifetime.ApplicationStarted.Register(async () =>
//{
//    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
//    {
//        var db = serviceScope.ServiceProvider.GetService<MidiContext>();
//        // Seed only if the db is completely empty
//        if(db != null || (db.MidiItems.Count() == 0 && db.Users.Count() == 0))
//            await Seed(db);
//    }
//});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseCors("AnyOrigin");
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bard Midi API");
    c.InjectStylesheet("/swagger/custom.css");
    c.RoutePrefix = String.Empty;
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

public partial class Program
{  // Makes it public for testing to be able to reference it

    public static async Task Seed(MidiContext context)
    {
        await context.Database.EnsureCreatedAsync();
        // Seeding only happens once as setup, so I'll hardcode this path
        var knownSongs = JsonConvert.DeserializeObject<Dictionary<string, JsonGuildSong>>(await File.ReadAllTextAsync(Path.Combine(Environment.CurrentDirectory, "knownSongs.json")));

        // If it's null, I'd like it to throw and tell me that it failed, and not continue - this is only for testing, after all

    #pragma warning disable CS8602 // Dereference of a possibly null reference.
        foreach (var song in knownSongs.Values)
        {
            var existingSong = context.MidiItems.Where(m => m.Hash == song.checksum).SingleOrDefault();
            var author = context.Users.Where(u => u.ServiceId == song.contributorId).SingleOrDefault();

            if (author == null)
            {
                author = new MidiUser()
                {
                    DisplayName = song.contributor,
                    ServiceId = song.contributorId,
                    MidisContributed = new List<MidiItem>()
                };
                context.Users.Add(author);
            }

            if (existingSong == null)
            {
                existingSong = new MidiItem()
                {
                    Hash = song.checksum,
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
            if (!author.MidisContributed.Any(m => m.Hash == song.checksum))
                author.MidisContributed.Add(existingSong);
            // Need to save each time or the changes don't seem to work for next iteration
            // Might could be resolved by clearing ChangeTracking
            await context.SaveChangesAsync();
        }
    #pragma warning restore CS8602 // Dereference of a possibly null reference.

        Console.WriteLine("Seeded " + knownSongs.Count + " values");
    }

} 
