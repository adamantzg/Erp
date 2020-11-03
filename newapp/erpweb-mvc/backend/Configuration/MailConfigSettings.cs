using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace backend.Properties
{
	public class MailConfigSettings
	{
		public string to { get; set;  }
		public string cc { get; set; }
		public string bcc { get; set; }
		public string subject { get; set; }
		public string body { get; set; }

		public static bool TryParse(string value, out MailConfigSettings settings)
		{
			try
			{
				settings = JsonConvert.DeserializeObject<MailConfigSettings>(value);
				return true;
			}
			catch
			{
				settings = null;
				return false;
			}
		}
	}
}