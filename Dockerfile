FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Lobby/Domain/Domain.csproj Lobby/Domain/
COPY Lobby/Application/Application.csproj Lobby/Application/
COPY Lobby/Infrastructure/Infrastructure.csproj Lobby/Infrastructure/
COPY Lobby/GameLobby/GameLobby.csproj Lobby/GameLobby/
COPY GameLobby.sln .

RUN dotnet restore GameLobby.sln

COPY . .

RUN dotnet publish Lobby/GameLobby/GameLobby.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GameLobby.dll"]
