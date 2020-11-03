using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace asaq2.Model.Dal.New.Test
{
	public class Utils
	{
		public static bool CompareObjects(object compareTo, object compare)
		{
			var result = true;
			var properties = compare.GetType().GetProperties();
			foreach (var p in properties)
			{
				var destProp = compareTo.GetType().GetProperty(p.Name);
				if (destProp != null)
				{
					var value = p.GetValue(compare);
					var destValue = destProp.GetValue(compareTo);
					if (value == null && destValue == null)
						continue;
					if (value == null || destValue == null || !value.Equals(destValue))
					{
						result = false;
						break;
					}
				}
			}

			return result;
		}
	}
}
