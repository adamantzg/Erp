using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace backend
{
	public interface IRegistryReader
	{
		object GetValue(string key, string value, object defaultValue = null);
		object GetHKLMValue(string subKey, string name, object defaultValue = null);
	}

	public class RegistryReader : IRegistryReader
	{
		public object GetValue(string key, string value, object defaultValue = null)
		{
			return Registry.GetValue(key, value, defaultValue);
		}

		public object GetHKLMValue(string subKey, string name, object defaultValue = null)
		{
			return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
				?.OpenSubKey(subKey)?.GetValue(name, null);
		}
	}
}