### to publish
*x64* 
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true

*osx silicon*

dotnet publish -c Release -r osx-arm64 --self-contained true /p:PublishSingleFile=true

### to run
 dotnet run --project PharmaBack.TrayApp
