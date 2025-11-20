using Players.Configuration;
using Players.GrpcServices; // »«·« «÷«›Â ò‰


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddControllers();
builder.Services.AddDependencyInjections();

var app = builder.Build();


app.MapControllers();

// Configure the HTTP request pipeline.
app.MapGrpcService<PlayerGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client...");

app.Run();
