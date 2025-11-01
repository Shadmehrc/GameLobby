FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY Domain/Domain.csproj Domain/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY GameLobby/GameLobby.csproj GameLobby/
COPY GameLobby.sln .

RUN dotnet restore GameLobby.sln


COPY . .


RUN dotnet publish GameLobby/GameLobby.csproj -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GameLobby.dll"]