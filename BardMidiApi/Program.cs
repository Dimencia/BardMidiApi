using BardMidiApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
