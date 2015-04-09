// ******************************************************************************
//  Copyright (C) CROC Inc. 2014. All rights reserved.
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Croc.DevTools.ResxToJson
{
	public class ConverterResult
	{
		private readonly List<LogItem> m_log = new List<LogItem>();
		public void AddMsg(Severity severity , string msg, params object[] args)
		{
			m_log.Add(new LogItem(severity, String.Format(msg, args)));
		}

		public List<LogItem> Log
		{
			get { return m_log; }
		}
	}

	public class LogItem
	{
		public LogItem(Severity severity, string message)
		{
			Severity = severity;
			Message = message;
		}

		public Severity Severity { get; private set; }
		public String Message { get; private set; }
	}
	public enum Severity
	{
		Trace,
		Info,
		Warning,
		Error
	}

	public class ResxToJsonConverter
	{
		class JsonResources
		{
			public JsonResources()
			{
				LocalizedResources = new Dictionary<string, JObject>();
			}

			public JObject BaseResources { get; set; }

			public IDictionary<string, JObject> LocalizedResources { get; private set; }
		}

		public static ConverterResult Convert(ResxToJsonConverterOptions options)
		{
			var result = new ConverterResult();

			IDictionary<string, ResourceBundle> bundles = null;
			if (options.InputFiles.Count > 0)
			{
				bundles = ResxHelper.GetResources(options.InputFiles);
			}
			if (options.InputFolders.Count > 0)
			{
				var bundles2 = ResxHelper.GetResources(options.InputFolders, options.Recursive);
				if (bundles == null )
				{
					bundles = bundles2;
				}
				else
				{
					// join two bundles collection
					foreach (var pair in bundles2)
					{
						bundles[pair.Key] = pair.Value;
					}
				}
			}

			if (bundles == null || bundles.Count == 0)
			{
				result.AddMsg(Severity.Warning, "No resx files were found");
				return result;
			}
			result.AddMsg(Severity.Trace, "Found {0} resx bundles", bundles.Count);
			if (bundles.Count > 1 && !String.IsNullOrEmpty(options.OutputFile))
			{
				// join multiple resx resources into a single js-bundle
				var bundleMerge = new ResourceBundle(Path.GetFileNameWithoutExtension(options.OutputFile));
				foreach (var pair in bundles)
				{
					bundleMerge.MergeWith(pair.Value);
				}
				result.AddMsg(Severity.Trace, "As 'outputFile' option was specified all bundles were merged into single bundle '{0}'", bundleMerge.BaseName);
				bundles = new Dictionary<string, ResourceBundle> {{bundleMerge.BaseName, bundleMerge}};
			}

			foreach (ResourceBundle bundle in bundles.Values)
			{
				JsonResources jsonResources = generateJsonResources(bundle, options);
				string baseFileName;
				string baseDir;
				if (!string.IsNullOrEmpty(options.OutputFile))
				{
					baseFileName = Path.GetFileName(options.OutputFile);
					baseDir = Path.GetDirectoryName(options.OutputFile);
				}
				else
				{
					baseFileName = bundle.BaseName.ToLowerInvariant() + GetOutputFileExtension(options.OutputFormat);
					baseDir = options.OutputFolder;
				}
				if (string.IsNullOrEmpty(baseDir))
				{
					baseDir = Environment.CurrentDirectory;
				}

				result.AddMsg(Severity.Trace, "Processing '{0}' bundle (contains {1} resx files)", bundle.BaseName, bundle.Cultures.Count);                
                string dirPath = options.OutputFormat == OutputFormat.i18next 
                    ? Path.Combine(baseDir, options.FallbackCulture)
                    : baseDir;
                string outputPath = Path.Combine(dirPath, baseFileName);
				string jsonText = stringifyJson(jsonResources.BaseResources, options);
				writeOutput(outputPath, jsonText, options, result);

				if (jsonResources.LocalizedResources.Count > 0)
				{
					foreach (KeyValuePair<string, JObject> pair in jsonResources.LocalizedResources)
					{
						dirPath = Path.Combine(baseDir, pair.Key);
						outputPath = Path.Combine(dirPath, baseFileName);
						jsonText = stringifyJson(pair.Value, options);
						writeOutput(outputPath, jsonText, options, result);
					}
				}
			}

			return result;
		}

        private static string GetOutputFileExtension(OutputFormat format)
	    {
            switch (format)
            {
                case OutputFormat.RequireJs:
                    return ".js";
                default:
                    return ".json";
            }
	    }

	    private static void writeOutput(string outputPath, string jsonText, ResxToJsonConverterOptions options, ConverterResult result)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			if (File.Exists(outputPath))
			{
				var attrs = File.GetAttributes(outputPath);
				if ((attrs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					if (options.Overwrite == OverwriteModes.Skip)
					{
						result.AddMsg(Severity.Error, "Cannot overwrite {0} file, skipping", outputPath);
						return;
					}
					// remove read-only attribute
					attrs = ~FileAttributes.ReadOnly & attrs;
					File.SetAttributes(outputPath, attrs);
				}
				// if existing file isn't readonly we just overwrite it
			}
			File.WriteAllText(outputPath, jsonText, Encoding.UTF8);
			result.AddMsg(Severity.Info, "Created {0} file", outputPath);
		}

		static string stringifyJson(JObject json, ResxToJsonConverterOptions options)
		{
			string text = json.ToString(Formatting.Indented);
		    switch (options.OutputFormat)
		    {
		        case OutputFormat.RequireJs:
                    return "define(" + text + ");";
                default:
		            return text;
		    }
		}

		private static JsonResources generateJsonResources(ResourceBundle bundle, ResxToJsonConverterOptions options)
		{
			var result = new JsonResources();
			// root resoruce
			IDictionary<string, string> baseValues = bundle.GetValues(null);
			JObject jBaseValues = convertValues(baseValues, options);
		    switch (options.OutputFormat)
		    {
		        case OutputFormat.RequireJs:
                    // When dealing with require.js i18n the root resource contains a "root" subnode that contains all 
                    // of the base translations and then a bunch of nodes like the following for each supported culture:
                    //   "en-US" : true
                    //   "fr" : true
                    //   ...
                    var jRoot = new JObject();
                    jRoot["root"] = jBaseValues;
                    foreach (CultureInfo culture in bundle.Cultures)
                    {
                        if (culture.Equals(CultureInfo.InvariantCulture))
                            continue;
                        jRoot[culture.Name] = true;
                    }
                    result.BaseResources = jRoot;
		            break;
                default:
                    // In the simplest case our output format is plain vanilla json (just a kvp dictionary)
		            result.BaseResources = jBaseValues;
		            break;
		    }

			// culture specific resources
			foreach (CultureInfo culture in bundle.Cultures)
			{
				if (culture.Equals(CultureInfo.InvariantCulture))
					continue;
				IDictionary<string, string> values = bundle.GetValues(culture);
				JObject jCultureValues = convertValues(values, options);
				result.LocalizedResources[culture.Name] = jCultureValues;
			}
			return result;
		}

		private static JObject convertValues(IDictionary<string, string> values, ResxToJsonConverterOptions options)
		{
			var json = new JObject();
			foreach (KeyValuePair<string, string> pair in values)
			{
				string fieldName = pair.Key;
				switch (options.Casing)
				{
					case JsonCasing.Camel:
						char[] chars = fieldName.ToCharArray();
						chars[0] = Char.ToLower(chars[0]);
						fieldName = new string(chars);
						break;
					case JsonCasing.Lower:
						fieldName = fieldName.ToLowerInvariant();
						break;
				}
				json[fieldName] = pair.Value;
			}
			return json;
		}
	}
}