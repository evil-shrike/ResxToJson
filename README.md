# Overview 

Resx-files into client js-resource converter  (for RequireJS i18n plugin)

The tool converts Visual Studio resources (*.resx files) into client-side js-files.
Result files should be loaded via RequireJS i18n plugin (see https://github.com/requirejs/i18n for details).

# OPTIONS

## inputDir
Default value: current directory
Required: no
Aliases: -i, -inputDir

Absolute or relative path to a directory with *.resx files.
The tool expects to find one or many files with the same base name:
* Resources.resx
* Resources.ru.resx
* Resources.nl.res
* Resources.de.res

## outputDir
Default value: current directory
Required: no
Aliases: -o, -oututDir

Absolute or relative path to a directory where result *.js files will be placed.

## casing
Default value: keep
Required: no
Aliases: -c, -case

It specified how resource keys from *.resx will be represented in json.
It can be one of the following values:
* keep - (default) as is (do not modify)
* camelCase - 'SomeMessage' -> 'someMessage'
* lowerCase - 'SomeMessage' -> 'somemessage'


# USAGE EXAMPLES
Let's consider that we have a VS project in c:\projects\MyServerProject with *.resx files.
And we want to convert server resources into client resources.

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
  "fileNotFound": "Файл не найден",
});
```