using Players.Configuration;
using Players.GrpcServices;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddDependencyInjections();

var app = builder.Build();

app.MapControllers();     
app.MapGrpcService<PlayerGrpcService>();

app.MapGet("/", () => "Players gRPC service is running.");

app.Run();