@echo off

rem This is done because it's a lot more useful to have a junction to a separate, 
rem permanent content folder in the repo's root directory than to keep copying 
rem the folder every time the project is built, and it's arguably better 
rem maintenance-wise than hard-coding in a relative path to the content folder 
rem itself. 

rem The architecture the solution is being built for
SET arch=x64

rem The solution configuration
SET config=Debug

rem The version of .NET the solution is being built for
SET netversion=net5.0-windows

rem Create the junction

echo Creating link...
mklink /j .\Source\Game\OpenTPW\bin\%arch%\%config%\%netversion%\Content .\Content

echo Done.

pause