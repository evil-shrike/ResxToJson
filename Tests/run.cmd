@echo off
echo Test: all files in dir to dir: -i . -dir dir1
..\ResxToJson.Cli\bin\Debug\ResxToJson.exe -dir out_dir2dir -f

echo Test: specified files to dir: -i file1 -i file2 -dir dir1
..\ResxToJson.Cli\bin\Debug\ResxToJson.exe -i Resources.resx -i Resources.ru.resx -i Resources2.resx -dir out_files2dir -f

echo Test: specified files to single file: -i file1 -i file2 -file result_file1
..\ResxToJson.Cli\bin\Debug\ResxToJson.exe -i Resources.resx -i Resources.ru.resx -i Resources2.resx -file out_files2file/res.js -f


echo Test: all files in dir to single file: -i . -file result_file1
..\ResxToJson.Cli\bin\Debug\ResxToJson.exe -i . -file out_dir2file/res.js  -f


echo Test: all files in dir to dir in i18next format: -i . -dir dir1 -format i18next
..\ResxToJson.Cli\bin\Debug\ResxToJson.exe -dir out_dir2diri18next -f -format i18next
