using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using vocafind_api.Mapping;
using vocafind_api.Models;
using vocafind_api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//mysql
builder.Services.AddDbContext<TalentcerdasContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 30))
        )
 );

//auto mapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));


// Config Email
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email:Smtp"));
builder.Services.AddTransient<IEmailService, EmailService>();


//builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
//builder.Services.AddTransient<IEmailService, EmailService>();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// pastikan ini ada agar bisa di akses 
app.UseStaticFiles();

// khusus untuk folder uploads
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
