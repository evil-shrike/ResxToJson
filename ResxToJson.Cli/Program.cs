// ******************************************************************************
//  Copyright (C) CROC Inc. 2014. All rights reserved.
// ******************************************************************************

using System;
using System.ComponentModel.Design.Serialization;
using System.IO;

namespace Croc.DevTools.ResxToJson
{
    class Program
    {
        enum ExitCode
        {
            InputArgumentMissing = -1,
            InvalidOutputArgument = -2,
            CaseArgumentMissing = -3,
            InvalidInputPath = -4,
            OutputFormatArgumentMissing = -5,
            FallbackArgumentMissing = -6
        }

        static void CrashAndBurn(ExitCode code, string crashMessage, params object[] args)
        {
            // Preserve the foreground color
            var c = Console.ForegroundColor;

            // Write out our error message in bright red text
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR: " + crashMessage, args);

            // Restore the foreground color
            Console.ForegroundColor = c;

            // Die!
            Environment.Exit((int)code);
        }

        static ResxToJsonConverterOptions getOptions(string[] args)
        {
            var options = new ResxToJsonConverterOptions();
            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];
                if (key == "-i" || key == "-input")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.InputArgumentMissing, "Value for option 'input' is missing");
                    }
                    options.Inputs.Add(args[i + 1]);
                    i++;
                    continue;
                }

                if (key == "-dir" || key == "-outputDir")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.InvalidOutputArgument, "Value for option 'outputDir' is missing");
                    }
                    options.OutputFolder = args[i + 1];
                    i++;
                    continue;
                }

                if (key == "-file" || key == "-outputFile")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.InvalidOutputArgument, "Value for option 'outputFile' is missing");
                        Console.WriteLine("ERROR: Value for option 'outputFile' is missing");
                        Environment.Exit(-2);
                    }
                    options.OutputFile = args[i + 1];
                    i++;
                    continue;
                }

                if (key == "-format" || key == "-outputFormat")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.OutputFormatArgumentMissing, "Value for option 'outputFormat' is missing");
                    }
                    OutputFormat format;
                    if (Enum.TryParse(args[i + 1], true, out format))
                    {
                        options.OutputFormat = format;
                    }
                    i++;
                    continue;
                }

                if (key == "-fallback" || key == "-fallbackCulture")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.FallbackArgumentMissing, "Value for option 'fallbackCulture' is missing");
                    }
                    options.FallbackCulture = args[i + 1];
                    i++;
                    continue;
                }

                if (key == "-c" || key == "-case")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.CaseArgumentMissing, "Value for option 'case' is missing");
                    }
                    JsonCasing casing;
                    if (Enum.TryParse(args[i + 1], true, out casing))
                    {
                        options.Casing = casing;
                    }
                    i++;
                    continue;
                }
                if (key == "-f" || key == "-force")
                {
                    options.Overwrite = OverwriteModes.Force;
                    continue;
                }
                if (key == "-r" || key == "-recursively")
                {
                    options.Recursive = true;
                    i++;
                    continue;
                }

                if (key == "-outputFileFormat")
                {
                    if (args.Length == i + 1)
                    {
                        CrashAndBurn(ExitCode.CaseArgumentMissing, "Value for option 'outputFileFormat' is missing");
                    }
                    options.OutputFileFormat = args[i + 1];
                    i++;
                    continue;
                }
            }
            return options;
        }

        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length == 1 && (args[0] == "-help" || args[0] == "-?"))
            {
                printHelp();
            }
            var options = getOptions(args);
            checkOptions(options);

            ConverterLogger logger = ResxToJsonConverter.Convert(options);
            foreach (var item in logger.Log)
            {
                ConsoleColor color;
                switch (item.Severity)
                {
                    case Severity.Trace:
                        color = ConsoleColor.DarkGray;
                        break;
                    case Severity.Info:
                        color = ConsoleColor.White;
                        break;
                    case Severity.Warning:
                        color = ConsoleColor.Yellow;
                        break;
                    case Severity.Error:
                        color = ConsoleColor.DarkRed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                var backupColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(item.Message);
                Console.ForegroundColor = backupColor;
            }

        }

        static void checkOptions(ResxToJsonConverterOptions options)
        {
            if (options.Inputs.Count > 0)
            {
                foreach (string input in options.Inputs)
                {
                    string path = input;
                    if (!Path.IsPathRooted(input))
                    {
                        path = Path.Combine(Environment.CurrentDirectory, input);
                    }

                    if (Directory.Exists(path))
                    {
                        options.InputFolders.Add(path);
                    }
                    else if (File.Exists(path))
                    {
                        options.InputFiles.Add(path);
                    }
                    else
                    {
                        CrashAndBurn(ExitCode.InvalidInputPath, "input path '{0}' doesn't relate to a file or a directory", path);
                    }
                }
            }
            else
            {
                options.InputFolders.Add(Environment.CurrentDirectory);
            }

            if (String.IsNullOrEmpty(options.OutputFolder) && String.IsNullOrEmpty(options.OutputFile))
            {
                options.OutputFolder = Environment.CurrentDirectory;
            }
        }

        static void printHelp()
        {
            Console.WriteLine(
@"ResxToJson (c) CROC Inc. 2014
A resx-resources to json converter for using with RequireJS i18n plugin (see https://github.com/requirejs/i18n).
USAGE:
  -input or -i              - path to directory with *.resx files or to separate file 
                              HINT: there can be several such options specifed at once
  -outputDir or -dir        - path to output directory (where result js files will be placed)
  -outputFile or -file      - path to output file (instead of outputDir)
  -outputFormat or -format  - output format selection:
                                RequireJs (default) -> output will be AMD modules suitable for use with requireJs i18n
                                i18next             -> output will be JSON dictionary files that can be used with i18next
  -outputFileFormat         - generate file in specific format.
                                Can be used tags:
                                <language> - language of resx file.
                                <resxFileName> - name of resx file.
  -fallback                 - When using i18next the 'root' translations get used as the fallback culture, which go in 
                              their own subdirectory (essentially forming their own culture). By default this will be 
                              'dev', however you should probably specify something more appropriate like 'en' or 'fr'
  -case or -c               - resource keys formating options: 
                                keep (default) - do not change names
                                camel - 'SomeMsg' -> 'someMsg'
                                lower - 'SomeMsg' -> 'somemsg'
  -force or -f              - overwrite existing read-only files (by default read-only files will not be overwritten)
  -recursive or -r          - search files in input dir recursively


EXAMPLES:
ResxToJson.exe -i .\Server -dir c:\src\MyPrj -c camel -force
Processes all *.resx in folder 'Server' (relative to current dir) and create js-files in 'c:\src\MyPrj' folder (one js-file for each resx-file)

ResxToJson.exe -i Messages.resx -i Messages.nl.resx -i Messages.de.resx -file .\client\resources.js
Process files Messages.resx, Messages.nl.resx, Messages.de.resx and create js-files for each resx with base name 'resources'

ResxToJson.exe -i .\Server -dir c:\src\MyPrj\content\locales -format i18next -fallback en
Processes all *.resx in folder 'Server' and creates json dictionary files in the 'c:\src\MyPrj\content\locales' folder (one for each resx file)
");
            Environment.Exit(0);
        }
    }
}
