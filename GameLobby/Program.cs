using GameLobby.Configuration;
using GameLobby.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDependencyInjections();
builder.Services.AddProjectServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", p => p
        .SetIsOriginAllowed(_ => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddSignalR()
    .AddStackExchangeRedis(builder.Configuration["Redis:ConnectionString"]);


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


app.UseSwagger();
app.UseSwaggerUI();


app.UseRouting();
app.UseCors("CorsPolicy");


app.UseAuthorization();

app.MapHub<LobbyHub>("/hubs/lobby");
app.MapGet("/health", () => Results.Ok(new { status = "ok" })); // just check ., maybe delete later



app.MapControllers();

// app.UseHttpsRedirection(); // http im alan
app.Run();