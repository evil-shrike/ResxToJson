// ******************************************************************************
//  Copyright (C) CROC Inc. 2014. All rights reserved.
// ******************************************************************************

using System;
using System.IO;

namespace Croc.DevTools.ResxToJson
{
	class Program
	{
		static ResxToJsonConverterOptions getOptions(string[] args)
		{
			var options = new ResxToJsonConverterOptions();
			for (int i = 0; i < args.Length; i++)
			{
				string key = args[i];
				if (key == "-i" || key == "-inputDir")
				{
					if (args.Length == i + 1)
					{
						Console.WriteLine("ERROR: Value for option 'inputDir' is missing");
						Environment.Exit(-1);
					}
					options.InputFolder = args[i + 1];
					i++;
					continue;
				}
				if (key == "-o" || key == "-outputDir")
				{
					if (args.Length == i + 1)
					{
						Console.WriteLine("ERROR: Value for option 'outputDir' is missing");
						Environment.Exit(-2);
					}
					options.OutputFolder = args[i + 1];
					i++;
					continue;
				}
				if (key == "-c" || key == "-case")
				{
					if (args.Length == i + 1)
					{
						Console.WriteLine("ERROR: Value for option 'case' is missing");
						Environment.Exit(-3);
					}
					JsonCasing casing;
					if (Enum.TryParse(args[i + 1], true, out casing))
					{
						options.Casing = casing;
					}
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
			ResxToJsonConverter.Convert(options);
		}

		static void checkOptions(ResxToJsonConverterOptions options)
		{
			if (String.IsNullOrEmpty(options.InputFolder))
			{
				options.InputFolder = Environment.CurrentDirectory;
			}
			else
			{
				if (!Path.IsPathRooted(options.InputFolder))
				{
					options.InputFolder = Path.Combine(Environment.CurrentDirectory, options.InputFolder);
				}
				if (!Directory.Exists(options.InputFolder))
				{
					Console.WriteLine("ERROR: Directory path '{0}' specified as input doesn't exist", options.InputFolder);
					Environment.Exit(-4);
				}
			}
			if (String.IsNullOrEmpty(options.OutputFolder))
			{
				options.OutputFolder = Environment.CurrentDirectory;
			}

		}
		static void printHelp()
		{
			Console.WriteLine(
@"(c) CROC Inc. 2014
ResxToJson - *.resx to json converter for using with RequireJS i18n plugin (see https://github.com/requirejs/i18n)
USAGE:
  -inputDir or -i  - path to directory with *.resx files
  -outputDir or -o - path to output directory (where result js files will be placed)
  -case or -c      - resource keys formating options: keep (default), camel ('SomeMsg' -> 'someMsg'), lower ('SomeMsg' -> 'somemsg')
");
			Environment.Exit(0);
		}
	}
}
