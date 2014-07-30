// ******************************************************************************
//  Copyright (C) CROC Inc. 2014. All rights reserved.
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Croc.DevTools.ResxToJson
{
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

		public static void Convert(ResxToJsonConverterOptions options)
		{
			IDictionary<string, ResourceBundle> bundles = ResxHelper.GetResources(options.InputFolder);
			/*if (options.SingleOutputFile)
			{
				
			}*/

			foreach (ResourceBundle bundle in bundles.Values)
			{
				JsonResources result = generateJsonResources(bundle, options);

				string baseFileName = bundle.BaseName.ToLowerInvariant() + ".js";
				string outputPath = Path.Combine(options.OutputFolder, baseFileName);
				string jsonText = stringifyJson(result.BaseResources, options);
				writeOutput(outputPath, jsonText);				

				if (result.LocalizedResources.Count > 0)
				{
					foreach (KeyValuePair<string, JObject> pair in result.LocalizedResources)
					{
						string dirPath = Path.Combine(options.OutputFolder, pair.Key);
						outputPath = Path.Combine(dirPath, baseFileName);
						jsonText = stringifyJson(pair.Value, options);
						writeOutput(outputPath, jsonText);						
					}
				}
			}
		}

		private static void writeOutput(string outputPath, string jsonText)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
			File.WriteAllText(outputPath, jsonText);
		}

		static string stringifyJson(JObject json, ResxToJsonConverterOptions options)
		{
			string text = json.ToString(Formatting.Indented);
			return "define(" + text + ");";
		}

		private static JsonResources generateJsonResources(ResourceBundle bundle, ResxToJsonConverterOptions options)
		{
			var result = new JsonResources();
			// root resoruce
			IDictionary<string, string> baseValues = bundle.GetValues(null);
			JObject jBaseValues = convertValues(baseValues, options);
			var jRoot = new JObject();
			jRoot["root"] = jBaseValues;
			foreach (CultureInfo culture in bundle.Cultures)
			{
				jRoot[culture.Name] = true;
			}
			result.BaseResources = jRoot;

			// culture specific resources
			foreach (CultureInfo culture in bundle.Cultures)
			{
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