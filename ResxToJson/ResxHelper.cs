// ******************************************************************************
//  Copyright (C) CROC Inc. 2014. All rights reserved.
// ******************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;

namespace Croc.DevTools.ResxToJson
{
	public class ResxHelper
	{
		private static readonly CultureInfo[] s_cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

		private static CultureInfo getCulture(string name)
		{
			return s_cultures.FirstOrDefault(ci => ci.Name == name);
		}

		/// <summary>
		/// Get resources from set of resx files. 
		/// Resources are groupped in bundles for each set of resx-files with same base name (like messages.resx, messages.en.resx, message.ru.resx)
		/// </summary>
		public static IDictionary<string, ResourceBundle> GetResources(IList<string> inputFiles, ConverterLogger logger)
		{
			var fileBundles = new Dictionary<string, ResourceFileBundle>();

			foreach (var filePath in inputFiles)
			{
				string fileName = Path.GetFileName(filePath);
				string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
				bool isBaseFile;
				int idx = fileNameWithoutExt.IndexOf(".", StringComparison.InvariantCulture);
				// All files with the same base name form a bundle
				string baseName = fileNameWithoutExt;
				CultureInfo culture = null;
				ResourceFileBundle bundle;
				if (idx == -1)
				{
					isBaseFile = true;
				}
				else
				{
					// file name contains "." - it can culture name or something else
					string suffix = fileNameWithoutExt.Substring(idx + 1);
					culture = getCulture(suffix);
					if (culture != null)
					{
						// the file is a culture-specific resource file
						isBaseFile = false;
						baseName = fileNameWithoutExt.Substring(0, idx);
					}
					else
					{
						isBaseFile = true;
					}
				}
				if (String.IsNullOrEmpty(baseName))
				{
					throw new Exception("Could not extract baseName from the file name: '" + fileName + "'");
				}

				if (!fileBundles.TryGetValue(baseName, out bundle))
				{
					bundle = new ResourceFileBundle();
					fileBundles[baseName] = bundle;
				}
				if (isBaseFile)
				{
					bundle.BaseName = baseName;
					bundle.BaseFile = filePath;
				}
				else
				{
					bundle.AddCultureFile(culture, filePath);
				}
			}

			var bundles = new Dictionary<string, ResourceBundle>();
			// read values from resx files grouped in bundles
			foreach (ResourceFileBundle fileBundle in fileBundles.Values)
			{
				if (String.IsNullOrEmpty(fileBundle.BaseFile) || String.IsNullOrEmpty(fileBundle.BaseName))
				{
					string locFiles = null;
					if (fileBundle.CultureFiles.Count > 0)
					{
						locFiles = string.Join(", ", fileBundle.CultureFiles.Select(p => p.Value));
					}
					logger.AddMsg(Severity.Error, "Ignoring localized resources without base resx-file {0}", locFiles != null ? ": " + locFiles : "");
					continue;
				}
				var bundle = new ResourceBundle(fileBundle.BaseName);
				bundle.AddResources(null, getKeyValuePairsFromResxFile(fileBundle.BaseFile));
				foreach (KeyValuePair<CultureInfo, string> pair in fileBundle.CultureFiles)
				{
					var values = getKeyValuePairsFromResxFile(pair.Value);
					bundle.AddResources(pair.Key, values);
				}
				bundles[fileBundle.BaseName] = bundle;
			}
			return bundles;
		}

		/// <summary>
		/// Get resources from resx files found in specified folders.
		/// Resources with groupped in bundles for each set of resx-files with same base name (like messages.resx, messages.en.resx, message.ru.resx)
		/// </summary>
		public static IDictionary<string, ResourceBundle> GetResources(ICollection<string> directories, bool recursive, ConverterLogger logger)
		{
			var files = new List<string>();
			foreach (string directory in directories)
			{
				if (!Directory.Exists(directory))
				{
					continue;
				}
				string[] resourceFiles = Directory.GetFiles(directory, "*.resx",
					recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
				files.AddRange(resourceFiles);
			}

			return GetResources(files, logger);
		}

		private static Dictionary<string, string> getKeyValuePairsFromResxFile(string filePath)
		{
			var resourceFileDict = new Dictionary<string, string>();
			// NOTE: here we're using ResXResourceReader from System.Windows.Forms 
			var resourceReader = new ResXResourceReader(filePath);
			try
			{
				foreach (DictionaryEntry d in resourceReader)
				{
					var key = d.Key as string;
					if (key != null)
					{
						resourceFileDict.Add(key, d.Value.ToString());
					}
				}
			}
			finally
			{
				resourceReader.Close();
			}

			return resourceFileDict;
		}
	}
}