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

		public static IDictionary<string, ResourceBundle> GetResources(string directory)
		{
			var bundles = new Dictionary<string, ResourceBundle>();

			if (Directory.Exists(directory))
			{
				var resourceFiles = Directory.GetFiles(directory, "*.resx");
				// All files with the same base name form a bundle
				foreach (var filePath in resourceFiles)
				{
					string fileName = Path.GetFileName(filePath);
					string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
					bool isBaseFile;
					int idx = fileNameWithoutExt.IndexOf(".");
					string baseName = fileNameWithoutExt;
					CultureInfo culture = null;
					ResourceBundle bundle;
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
					if (!bundles.TryGetValue(baseName, out bundle))
					{
						bundle = new ResourceBundle();
						bundles[baseName] = bundle;
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
			}
			foreach (ResourceBundle bundle in bundles.Values)
			{
				if (String.IsNullOrEmpty(bundle.BaseFile))
				{
					throw new Exception("Base resource file was found:" + bundle.BaseName);
				}
				bundle.AddResources(null, getKeyValuePairsFromResxFile(bundle.BaseFile));
				foreach (KeyValuePair<CultureInfo, string> pair in bundle.CultureFiles)
				{
					var values = getKeyValuePairsFromResxFile(pair.Value);
					bundle.AddResources(pair.Key, values);
				}
			}
			return bundles;
		}

		private static Dictionary<string, string> getKeyValuePairsFromResxFile(string filePath)
		{
			var resourceFileDict = new Dictionary<string, string>();
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