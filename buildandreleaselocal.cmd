dotnet tool uninstall --global errchk.Program
dotnet build -c Release
dotnet tool install --global --add-source ./nupkg errchk.Program