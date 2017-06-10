@echo off
echo Would you like to push the packages to NuGet when finished?
set /p choice="Enter y/n: "

@echo on

if /i %choice% equ y (
    ".nuget/nuget.exe" push PortableRest\bin\Release\PortableRest.*.nupkg -Source https://www.nuget.org/api/v2/package
)
pause