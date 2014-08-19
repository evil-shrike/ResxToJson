// ******************************************************************************
//  Copyright (C) CROC Inc. 2014. All rights reserved.
// ******************************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Croc.DevTools.ResxToJson
{
	/// <summary>
	/// Descriptor for collection of resource files (a resx base file and its culture-specific siblings)
	/// I.e. all files with the same base name (Messages.resx + Messages.ru.resx + Message.es.resx) form a bundle.
	/// </summary>
	public class ResourceFileBundle
	{
		private readonly Dictionary<CultureInfo, string> m_cultureFiles;


		public ResourceFileBundle()
		{
			m_cultureFiles = new Dictionary<CultureInfo, string>();
		}

		/// <summary>
		/// Name w/o extension of base resource file.
		/// </summary>
		public string BaseName { get; set; }

		/// <summary>
		/// Base resource file path.
		/// </summary>
		public string BaseFile { get; set; }

		public void AddCultureFile(CultureInfo culture, string filePath)
		{
			Cultures.Add(culture);
			CultureFiles[culture] = filePath;
		}

		/// <summary>
		/// Collection of culture-specific resource files pathes.
		/// </summary>
		public IDictionary<CultureInfo, string> CultureFiles
		{
			get { return m_cultureFiles; }
		}

		/// <summary>
		/// Collection of cultures in the bundle.
		/// </summary>
		public IList<CultureInfo> Cultures
		{
			get { return m_cultureFiles.Keys.ToList(); }
		}
	}

	/// <summary>
	/// Resource bundle created from a set of resx files (<see cref="ResourceFileBundle"/>).
	/// Hold localazed values for different cultures and for base culture.
	/// </summary>
	public class ResourceBundle
	{
		private readonly Dictionary<CultureInfo, IDictionary<string, string>> m_resources;

		public ResourceBundle(string baseName)
		{
			BaseName = baseName;
			m_resources = new Dictionary<CultureInfo, IDictionary<string, string>>();
		}

		/// <summary>
		/// Base name of the bundle (usially it's base name of resx file it's created from)
		/// </summary>
		public string BaseName { get; private set; }

		/// <summary>
		/// Get resource values (collection of key/value pairs) for the specific culture or base resources (if culture is null).
		/// </summary>
		/// <param name="culture"></param>
		/// <returns></returns>
		public IDictionary<string, string> GetValues(CultureInfo culture)
		{
			if (culture == null)
			{
				culture = CultureInfo.InvariantCulture;
			}
			IDictionary<string, string> values;
			m_resources.TryGetValue(culture, out values);
			return values;
		}

		/// <summary>
		/// Append resource values for the specific resources or base resources *if culture is null).
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="values"></param>
		public void AddResources(CultureInfo culture, IDictionary<string, string> values)
		{
			if (culture == null)
			{
				culture = CultureInfo.InvariantCulture;
			}
			IDictionary<string, string> valuesCur;
			if (m_resources.TryGetValue(culture, out valuesCur))
			{
				foreach (var pair in values)
				{
					valuesCur[pair.Key] = pair.Value;
				}
			}
			else
			{
				m_resources[culture] = values;
			}
		}

		/// <summary>
		/// All culures in the bundle (not counting base culture).
		/// </summary>
		public IList<CultureInfo> Cultures
		{
			get { return m_resources.Keys.ToList(); }
		}

		public void MergeWith(ResourceBundle other)
		{
			// NOTE: Cultures includes InvarianCulture for root resources as well
			foreach (var culture in other.Cultures)
			{
				AddResources(culture, other.GetValues(culture));
			}
		}
	}
}