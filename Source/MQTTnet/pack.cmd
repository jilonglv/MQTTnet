cd /d %~dp0
REM dotnet pack .\src\example\example.csproj -o c:\published\example -c Release /p:Version=1.2.3
set rep="%appdata%\..\..\.nuget\packages\MQTTnet40"
if exist %rep% (rd /S/Q %rep%)
::dotnet pack -v q --nologo --output ..\..\..\lib -c Release /p:Version=4.2.1
::dotnet pack -v q --nologo --output ..\..\..\lib -c Release /p:CNC="CNC"
::dotnet pack -v q --nologo --output ..\..\..\lib -c Release
dotnet pack -v q --nologo -c Release
::dotnet pack /verbosity:q /nologo --output ..\..\lib -c Release /p:NuspecFile=.\project.nuspec
rem exit