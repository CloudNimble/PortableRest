@echo off
echo Would you like to push the packages to NuGet when finished?
set /p choice="Enter y/n: "

del *.nupkg
@echo on
".nuget/nuget.exe" pack PortableRest.nuspec -symbols
".nuget/nuget.exe" pack PortableRest.Signed.nuspec -symbols
if /i %choice% equ y (
    ".nuget/nuget.exe" push PortableRest.*.nupkg
)
pause