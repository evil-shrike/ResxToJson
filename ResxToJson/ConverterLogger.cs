using System;
using System.Collections.Generic;

namespace Croc.DevTools.ResxToJson
{
	public class ConverterLogger
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

}