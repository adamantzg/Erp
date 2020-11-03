using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF.Mappings
{
    public class ConfigSettingMappings : EntityTypeConfiguration<ConfigSetting>
    {
        public ConfigSettingMappings()
        {
            ToTable("config_setting");
            HasRequired(s => s.Application).WithMany().HasForeignKey(s => s.idApplication);
            HasOptional(s => s.WebSite).WithMany().HasForeignKey(s => s.idWebSite);
        }
    }
}
