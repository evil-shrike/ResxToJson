# OVERVIEW

This tool converts .NET resources (*.resx-files) into client js-resources.
Currently the tool supports two output formats:
* js files for loading via RequireJS-plugin i18n (see http://requirejs.org/docs/api.html#i18n)
* json files for loading via i18next (see http://i18next.com/)


# OPTIONS

## input
Default value: current directory  
Required: no  
Aliases: `-i`, `-input`  
  
Absolute or relative path to a directory with *.resx files or to a resx file.
There can be several such option specified at once.
The tool expects to find one or many files with the same base name:
* Resources.resx  
* Resources.ru.resx  
* Resources.nl.res
* Resources.de.res

All such files form a _bundle_ with base name 'Resources' and 4 culture-specific resources (one of them is invariant culture).
If the option targets a file then you should specifies all files of a bundle (like: `-i res.resx -i res.nl.resx -i res.ru.resx`). You can mix specifying folders and separate files.

## outputFormat
Value: `RequireJs`, `i18next`  
Default value: `RequireJs`  
Required: no  
Aliases: `-outputFormat`, `-format`  

Output format selection:
* `RequireJs` (default) - output will be AMD modules suitable for use with [RequireJS](http://requirejs.org/) [i18n](http://requirejs.org/docs/api.html#i18n)  
* `i18next` - output will be JSON dictionary files that can be used with i18next


## outputDir
Default value: current directory  
Required: no  
Aliases: `-outputDir`, `-dir`  
  
Absolute or relative path to a directory where result *.js files will be placed. 
With this option a result js-file will be generated for each input file (whether how they were specified).
This is default option if none of `inputDir` or `inputFile` options were specified.
Cannot be used with `outputFile` option simultaneously.


## outputFile
Required: no  
Aliases: `-outputFile`, `-file`

Absolute or relative path to a result js-file. If several inputs specified (via `inputDir` or `inputFile` options)
then resources will be merged into the single js-file. 
Cannot be used with `outputDir` option simultaneously.

## outputFileFormat
Required: no  
Aliases: `-outputFileFormat`

Generate file in specific format.
Can be used tags:
```
<language> - language of resx file.
<resxFileName> - name of resx file.
```
## casing
Value: `keep`, `camelCase`, `lowerCase`  
Default value: `keep`  
Required: no  
Aliases: `-c`, `-case`  
  
It specified how resource keys from *.resx will be represented in json.  
It can be one of the following values:  
* `keep` - (default) as is (do not modify)
* `camelCase` - 'SomeMessage' -> 'someMessage'
* `lowerCase` - 'SomeMessage' -> 'somemessage'


## recursive
Value: none (it's a flag)  
Default value: not specified  
Required: no  
Aliases: `-f`, `-recursively`  

The flag specifies that searching in a directory specified with `input` option will be recursive.


## force
Value: none (it's a flag)  
Default value: not specified
Required: no
Aliases: `-f`, `-force`

The flag specifies then read-only files (if any) will be overwritten.


## fallbackCulture
Value: string  
Default value: 'dev'  
Required: no  
Aliases: `-fallback`, `-fallbackCulture`  

When using i18next as output format the 'root' translations get used as the fallback culture, which go in their own subdirectory (essentially forming their own culture). By default this will be 'dev', however you should probably specify something more appropriate like 'en' or 'fr'.


# INSTALL

You can install the tool as NuGet-package [ResxToJson](https://www.nuget.org/packages/ResxToJson) from nuget.org.

The package contains single exe module ResxToJson.exe in package's 'tools' folder.


# USAGE EXAMPLES
## RequireJS
Let's consider that we have a VS project in 'c:\projects\MyServerProject' with *.resx files.  
And we want to convert server resources into client resources in 'MyClientProject' folder.

```
ResxToJson.exe -i c:\projects\MyServerProject -dir .\MyClientProject -c camel
```

If MyServerProject contains two files Resources.resx and Resources.ru.resx then MyClientProject will contain: 

* resources.js - default resources from Resources.resx
* ru/resource.js - resources from Resources.ru.resx

`resources.js` will look like:
```js
define({
  "root": {
    "fileNotFound": "File cannot be found",
  },
  "ru": true
});
```

and `ru/resources.js` will look like:
```js
define({
  "fileNotFound": "Файл не найден"
});
```

Later we can import js-resources in our client application:
```js
define(["i18n!nls/resources"], function (resources) {
  alert(resources.fileNotFound);
});
```

## i18next 
Now let's consider that we have the same VS project ('MyServerProject' with *.resx files) and we want to convert server resources into client json resources for loading via i18next.  

```
ResxToJson.exe -i .\MyServerProject -dir .\MyClientProject\content\locales -format i18next -fallback en
```

The tool processes all *.resx in folder 'MyServerProject' and creates json dictionary files in the '.\MyClientProject\content\locales' folder (one for each resx file).

If 'MyServerProject' contains two files Resources.resx and Resources.ru.resx then MyClientProject\content\locales will contain: 

* en/resources.json - default resources from Resources.resx:
```json
{
    "fileNotFound": "File cannot be found"
}
```
* ru/resource.json - resources from Resources.ru.resx:
```json
{
    "fileNotFound": "Файл не найден"
}
```
