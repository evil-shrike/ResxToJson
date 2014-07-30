@echo off
@IF EXIST "_stage" ( goto :error )
mkdir _stage
mkdir _stage\tools
rem xcopy ..\ResxToJson.Cli\bin\Debug\ResxToJson.exe _stage\tools\ /Y
rem xcopy ..\ResxToJson.Cli\bin\Debug\ResxToJson.pdb _stage\tools\ /Y 
rem xcopy ..\ResxToJson.Cli\bin\Debug\Croc.DevTools.ResxToJson.* _stage\tools\ /Y
rem xcopy ..\ResxToJson.Cli\bin\Debug\Newtonsoft.Json.* _stage\tools\ /Y

xcopy ResxToJson.nuspec _stage\ /Y
..\packages\ILRepack.1.25.0\tools\ILRepack.exe /out:_stage\tools\ResxToJson.exe ..\ResxToJson.Cli\bin\Debug\ResxToJson.exe ..\ResxToJson.Cli\bin\Debug\Croc.DevTools.ResxToJson.dll ..\ResxToJson.Cli\bin\Debug\Newtonsoft.Json.dll

cd _stage\
nuget pack ResxToJson.nuspec
cd ..

exit

:error
echo ERROR: Remove "_stage" dir first