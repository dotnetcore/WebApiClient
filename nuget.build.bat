cd WebApiClient.AOT.Task
dotnet publish -v q -c AOT_Release -f net46 -o ..\WebApiClient\MsBuild\BuildTask\net46\
dotnet publish -v q -c AOT_Release -f netcoreapp2.0 -o ..\WebApiClient\MsBuild\BuildTask\netcoreapp2.0\

cd ..\WebApiClient
dotnet build -c AOT_Release -f net45
dotnet build -c AOT_Release -f netcoreapp2.1
dotnet build -c AOT_Release -f netstandard1.3

dotnet build -c JIT_Release -f net45
dotnet build -c JIT_Release -f netcoreapp2.1 
dotnet build -c JIT_Release -f netstandard2.0

dotnet pack -c AOT_Release
dotnet pack -c JIT_Release

pause