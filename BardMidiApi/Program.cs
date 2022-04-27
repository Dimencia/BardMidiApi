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

builder.Services.AddControllers().AddNewtonsoftJson(x => 
    x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MidiContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);



// This was part of the MSDN but I don't know what it's doing or how to do it in modern .net... 
// services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//        .AddEntityFrameworkStores<ApplicationDbContext>();

// So, Discord auth is important but, the API itself should be oblivious to it
// It should accept an auth header, containing the middle-of-the-process token from Discord, from a client app or whatever
// And we finish the oauth process and get their actual user

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


//app.Lifetime.ApplicationStarted.Register(async () =>
//{
//    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
//    {
//        var db = serviceScope.ServiceProvider.GetService<MidiContext>();
//
//        //if (await db.Database.EnsureCreatedAsync()) // Returns false if it already exist, not what I want
//        //{
//        if(db != null && db.MidiItems.Count() == 0 && db.Users.Count() == 0)
//            await Startup.Seed(db);
//        //}
//    }
//});
 // Re-enable to seed


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

//app.UseRouting();
//
//app.UseAuthentication();
//app.UseAuthorization();
//
//app.UseEndpoints(endpoints =>
//endpoints.MapRazorPages()); // Er... ?We don't have any, do we...?

app.MapControllers();

app.Run();

public partial class Program { } // Makes it public for testing