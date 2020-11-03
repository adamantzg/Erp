using erp.DAL.EF.Repositories;
using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace erp.DAL.EF
{
    public class WebProductFileTypeRepository : GenericRepository<Web_product_file_type>
    {
        public WebProductFileTypeRepository(Model context) : base(context)
        {
        }

        public List<Web_product_file_type> GetForSite(int site_id, bool overrideUniversalTypes = true)
        {
            var result = Get(wpft => wpft.site_id == site_id || wpft.site_id == null).ToList();
            var types = context.Set<File_type>().ToList();
            
            return GetForSite(site_id, result, types, overrideUniversalTypes);
        }

        public List<Web_product_file_type> GetForSite(int site_id, List<Web_product_file_type> all, List<File_type> fileTypes, bool overrideUniversalTypes = true)
        {
            var result = all.Where(ft => ft.site_id == site_id || ft.site_id == null).ToList();
            foreach (var webProductFileType in result) {
                if (string.IsNullOrEmpty(webProductFileType.path)) {
                    var ft = fileTypes.FirstOrDefault(t => t.id == webProductFileType.file_type_id);
                    if (ft != null)
                        webProductFileType.path = ft.path;
                }
                if (string.IsNullOrEmpty(webProductFileType.previewpath)) {
                    var ft = fileTypes.FirstOrDefault(t => t.id == webProductFileType.file_type_id);
                    if (ft != null)
                        webProductFileType.previewpath = ft.previewpath;
                }
            }

            var tobeRemoved = new List<Web_product_file_type>();
            //Eliminate default if there is an override
            if (overrideUniversalTypes) {
                foreach (var type in result.Where(r => r.site_id == null)) {
                    if (result.Count(r => r.site_id == site_id && r.code == type.code) > 0) {
                        tobeRemoved.Add(type);
                    }
                }
                foreach (var webProductFileType in tobeRemoved) {
                    result.Remove(webProductFileType);
                }
            }
            return result;
        }
    }
}
