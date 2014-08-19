# OVERVIEW

This tool converters .NET resources (*.resx-files) into client js-resource.
Result files should be loaded via RequireJS i18n plugin (see https://github.com/requirejs/i18n for details).

# OPTIONS

## input
Default value: current directory  
Required: no  
Aliases: -i, -input
  
Absolute or relative path to a directory with *.resx files or to a resx file.
There can be several such option specified at once.
The tool expects to find one or many files with the same base name:
* Resources.resx
* Resources.ru.resx
* Resources.nl.res
* Resources.de.res


## outputDir
Default value: current directory  
Required: no  
Aliases: -outputDir, -dir  
  
Absolute or relative path to a directory where result *.js files will be placed. 
A js file is generated for each input file.
This is default option if none of `inputDir` or `inputFile` options specified.
Cannot be used with `outputFile` option.

## outputFile
Required: no  
Aliases: -outputFile, -file

Absolute or relative path to a result js file. If several inputs specified (via `inputDir` or `inputFile` options)
then resources will be merged into the single js file. 
Cannot be used with `outputDir` option.


## casing
Value: keep, camelCase, lowerCase
Default value: keep  
Required: no  
Aliases: -c, -case  
  
It specified how resource keys from *.resx will be represented in json.  
It can be one of the following values:
* keep - (default) as is (do not modify)
* camelCase - 'SomeMessage' -> 'someMessage'
* lowerCase - 'SomeMessage' -> 'somemessage'


## recursive
Value: true/false
Default value: false
Required: no
Aliases: -f, -recursively

Specifies that searching in `inputDir` will be recirsive.


## force
Value: none, it's flag
Default value: not specified
Required: no
Aliases: -f, -force

If specified then read-only files (if any) will be overwritten.


# INSTALL

You can install the tool as NuGet-package [ResxToJson] (https://www.nuget.org/packages/ResxToJson) from nuget.org.

The package contains single exe module ResxToJson.exe in package 'tools' folder.


# USAGE EXAMPLES
Let's consider that we have a VS project in 'c:\projects\MyServerProject' with *.resx files.  
And we want to convert server resources into client resources in 'MyClientProject' folder.

```
ResxToJson.exe -i c:\projects\MyServerProject -o .\MyClientProject -c camel
```

If MyServerProject contains two files Resources.resx and Resources.ru.resx then MyClientProject will contain:
* resources.js - default resources from Resources.resx
* ru/resource.js - resources from Resources.ru.resx

resources.js will look like:
```
define({
  "root": {
    "fileNotFound": "File cannot be found",
  },
  "ru": true
});
```
and ru/resources.js will look like:
```
define({
  "fileNotFound": "Файл не найден"
});
```
Later we will import js-resources in our client application.
```
define(["i18n!nls/resources"], function (resources) {
  alert(resources.fileNotFound);
});
```
