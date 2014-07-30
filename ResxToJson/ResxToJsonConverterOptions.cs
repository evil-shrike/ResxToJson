// ******************************************************************************
//  Copyright (C) CROC Inc. 2014. All rights reserved.
// ******************************************************************************

namespace Croc.DevTools.ResxToJson
{
	/// <summary>
	/// Options for <see cref="ResxToJsonConverter"/>.
	/// </summary>
	public class ResxToJsonConverterOptions
	{
		public ResxToJsonConverterOptions()
		{
			//OutputFileName = "resources.js";
		}

		/// <summary>
		/// Input folder full path.
		/// </summary>
		public string InputFolder { get; set; }

		/// <summary>
		/// Output folder full path.
		/// </summary>
		public string OutputFolder { get; set; }

		//public string OutputFileName { get; set; }

		/// <summary>
		/// Options for formating resources keys names.
		/// </summary>
		public JsonCasing Casing { get; set; }
	}
}