using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using MySql.Data.MySqlClient;

namespace erp.Model.Dal.New
{
    public class WebProductFileTypeDAL : GenericDal<Web_product_file_type>, IWebProductFileTypeDAL
    {
	    private IFileTypeDal fileTypeDal;

	    public WebProductFileTypeDAL(IDbConnection conn, IFileTypeDal fileTypeDal) : base(conn)
	    {
		    this.fileTypeDal = fileTypeDal;
	    }

        public List<Web_product_file_type> GetForSite(int site_id, bool overrideUniversalTypes = true)
        {
	        var result = conn.Query<Web_product_file_type>(
		        "SELECT * FROM web_product_file_type WHERE site_id = @site_id OR site_id IS NULL",
		        new {site_id}).ToList();

            var types = fileTypeDal.GetAll();
			
            return GetForSite(site_id,result,types,overrideUniversalTypes);
        }

        public List<Web_product_file_type> GetForSite(int site_id, List<Web_product_file_type> all, List<File_type> fileTypes, 
	        bool overrideUniversalTypes = true )
        {
            var result = all.Where(ft => ft.site_id == site_id || ft.site_id == null).ToList();
            foreach (var webProductFileType in result)
            {
                if (string.IsNullOrEmpty(webProductFileType.path))
                {
                    var ft = fileTypes.FirstOrDefault(t => t.id == webProductFileType.file_type_id);
                    if (ft != null)
                        webProductFileType.path = ft.path;
                }
                if (string.IsNullOrEmpty(webProductFileType.previewpath))
                {
                    var ft = fileTypes.FirstOrDefault(t => t.id == webProductFileType.file_type_id);
                    if (ft != null)
                        webProductFileType.previewpath = ft.previewpath;
                }
            }

            var tobeRemoved = new List<Web_product_file_type>();
            //Eliminate default if there is an override
            if (overrideUniversalTypes)
            {
                foreach (var type in result.Where(r => r.site_id == null))
                {
                    if (result.Count(r => r.site_id == site_id && r.code == type.code) > 0)
                    {
                        tobeRemoved.Add(type);
                    }
                }
                foreach (var webProductFileType in tobeRemoved)
                {
                    result.Remove(webProductFileType);
                }
            }
            return result;
        }


	    
	    protected override string GetAllSql()
	    {
		    return "SELECT * FROM web_product_file_type";
	    }

	    protected override string GetByIdSql()
	    {
		    return "SELECT * FROM web_product_file_type WHERE id = @id";
	    }

	    protected override string GetCreateSql()
	    {
		    return
			    @"INSERT INTO web_product_file_type (id,name,code,site_id,file_type_id,path,previewpath,suffix) 
					VALUES(@id,@name,@code,@site_id,@file_type_id,@path,@previewpath,@suffix)";
	    }

	    protected override string GetUpdateSql()
	    {
		    return
			    @"UPDATE web_product_file_type SET name = @name,code = @code,site_id = @site_id,file_type_id = @file_type_id,path = @path,
					previewpath = @previewpath,suffix = @suffix WHERE id = @id";
	    }

	    protected override string GetDeleteSql()
	    {
		    return "DELETE FROM web_product_file_type WHERE id = @id";
	    }
    }
}
