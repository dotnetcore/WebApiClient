@echo off

set "folder=artifacts\package\release"

for %%f in ("%folder%\*.nupkg") do (
	echo push %%f
    nuget push %%f -source https://api.nuget.org/v3/index.json
)