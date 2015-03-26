// ******************************************************************************
//  Copyright (C) CROC Inc. 2014. All rights reserved.
// ******************************************************************************

using System;
using System.Collections.Generic;

namespace Croc.DevTools.ResxToJson
{
	/// <summary>
	/// Options for <see cref="ResxToJsonConverter"/>.
	/// </summary>
	public class ResxToJsonConverterOptions
	{
		public ResxToJsonConverterOptions()
		{
			Inputs = new List<string>();
			InputFiles = new List<string>();
			InputFolders  = new List<string>();
            OutputFormat = OutputFormat.RequireJs;
		    FallbackCulture = "dev";
		}

		/// <summary>
		/// Raw inputs (files and dirs).
		/// </summary>
		public List<string> Inputs { get; private set; }

		/// <summary>
		/// Input folders full pathes.
		/// </summary>
		public List<string> InputFolders { get; private set; }

		public Boolean Recursive { get; set; }

		/// <summary>
		/// Input files.
		/// </summary>
		public List<string> InputFiles { get; private set; }

		/// <summary>
		/// Output folder full path.
		/// </summary>
		public string OutputFolder { get; set; }

		/// <summary>
		/// Output file path.
		/// </summary>
		public string OutputFile { get; set; }

        /// <summary>
        /// The output format.
        /// </summary>
        public OutputFormat OutputFormat { get; set; }

        /// <summary>
        /// When outputing i18next files the "root" culture goes in 
        /// it's own subdirectory - effectively forming it's own custom 
        /// culture which, by default, is "dev". A better option is to 
        /// call this something like "en" (if our original resources are 
        /// in English) or "fr" or something. That way the fallback will 
        /// also be a logical fallback for specific locales such as en-US
        /// or fr-FR.
        /// </summary>
	    public string FallbackCulture { get; set; }

		/// <summary>
		/// Options for formating resources keys names.
		/// </summary>
		public JsonCasing Casing { get; set; }

		/// <summary>
		/// Overwrite existing files. 
		/// </summary>
		public OverwriteModes Overwrite { get; set; }
	}

	public enum OverwriteModes
	{
		Skip = 0,
		Ask,
		Force
	}

    public enum OutputFormat
    {
        RequireJs,
        i18next
    }
}