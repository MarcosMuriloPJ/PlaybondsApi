using PlaybondsApi.Hubs;
using PlaybondsApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowSpecificOrigin", builder =>
  {
    builder.WithOrigins("http://localhost:4200")
          .AllowCredentials()
          .AllowAnyMethod()
          .AllowAnyHeader();
  });
});


builder.Services.AddControllers();
builder.Services.AddSignalR();
// Em caso de escalada do projeto, implementaria os serviços em um método de extensão
builder.Services.AddSingleton<GameService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddHostedService<GameTimerService>();

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");
app.UseRouting();

app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();
