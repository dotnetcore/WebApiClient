cd WebApiClient
rd MsBuild\BuildTask /s /q

cd ..\WebApiClient.BuildTask
dotnet build -c AOT_Release -f net45
dotnet build -c AOT_Release -f netcoreapp1.1 
dotnet publish -c AOT_Release -f net45 -o ..\WebApiClient\MsBuild\BuildTask\net45\
dotnet publish -c AOT_Release -f netcoreapp1.1 -o ..\WebApiClient\MsBuild\BuildTask\netcoreapp1.1\



cd ..\WebApiClient
dotnet build -c AOT_Release -f net45
dotnet build -c AOT_Release -f netcoreapp2.1
dotnet build -c AOT_Release -f netstandard1.3
dotnet build -c JIT_Release -f net45
dotnet build -c JIT_Release -f netcoreapp2.1 
dotnet build -c JIT_Release -f netstandard1.3

del MsBuild\BuildTask\net45\Microsoft.Build*.dll
dotnet pack -c AOT_Release
dotnet pack -c JIT_Release

pause