﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace erp.DAL.EF
{
    public class Configuration
    {
        private static Dictionary<string, string> values = new Dictionary<string, string>();

        public static Configuration Instance { get; set; }

        static Configuration()
        {
            Instance = new Configuration();
        }

        public static void Initialize(int applicationId, int? webSiteid = null)
        {
            lock(values)
            {
                var unitOfWork = new UnitOfWork();
                var settings =
                    unitOfWork.ConfigSettingGenericRepository.Get(
                        c => c.idApplication == applicationId && (c.idWebSite == null || c.idWebSite == webSiteid)).ToList();
                values =
                    settings.GroupBy(s => s.name)
                        .Where(g => g.Count() == 1)
                        .Select(g => g.First())
                        .Union(
                            settings.GroupBy(s => s.name)
                                .Where(g => g.Count() > 1)
                                .Select(g => g.FirstOrDefault(s => s.idWebSite != null)))
                        .ToDictionary(s => s.name, s => s.value);
            }
            
        }

        public static string GetValue(string name)
        {
            if (values.ContainsKey(name))
                return values[name];
            return null;
        }

        public static int GetIntValue(string name, int defaultValue = 0)
        {
            int result = defaultValue;
            var value = GetValue(name);
            int.TryParse(value, out result);
            return result;
        }

        public static DateTime? GetDateValue(string name)
        {
            var sDate = GetValue(name);
            DateTime result;
            if (DateTime.TryParse(sDate, out result))
                return result;
            return null;
        }
    }
}
