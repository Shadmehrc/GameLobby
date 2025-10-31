@echo off
setlocal
for %%f in (*.csproj) do set "P=%%f"
if not defined P (echo No .csproj here.& pause & exit /b 1)

echo [1/2] restore & build once...
dotnet restore "%P%" || goto :fail
dotnet build "%P%" -c Debug /m || goto :fail

echo [2/2] start instances without building...
start "App-5144" cmd /k dotnet run --no-build --project "%P%" --urls "http://localhost:5144"
start "App-5145" cmd /k dotnet run --no-build --project "%P%" --urls "http://localhost:5145"
start "App-5146" cmd /k dotnet run --no-build --project "%P%" --urls "http://localhost:5146"
echo Started on 5144, 5145, 5146.
pause
exit /b 0

:fail
echo Build failed. Fix errors above.
pause
exit /b 1
