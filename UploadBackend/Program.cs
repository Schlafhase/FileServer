using Microsoft.Extensions.FileProviders;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseSwagger();
app.UseSwaggerUI();
// app.UseHttpsRedirection();
app.MapControllers();

Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads"));
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "uploads")),
    RequestPath = "/uploads"
});


app.Run();