using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using erp.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace erp.Model.DAL
{
    public class Info
    {
        public static List<Tables> GetTablesColumns()
        {
            var result = new List<Tables>();
            using(var conn=new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();

                var cmd = Utils.GetCommand("SELECT table_name, column_name FROM information_schema.columns WHERE table_schema= \'asaq\' ", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add( new Tables
                    {
                        Name= dr["table_name"].ToString(),                        
                        ColumnNames=dr["column_name"].ToString(),
                        //Rows=(int)dr["table_rows"]

                    });
                }
                




                //DataTable t = conn.GetSchema("Tables");
                //foreach (var name in t.Rows)
                //{
                //    var tablename = name;
                //   // result.Add(tablename);
                //}
            }
            
            return result;
        }

        public static List<Tables> GetTablesInfo()
        {
            var result =new List<Tables>();
            using (var conn= new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = Utils.GetCommand("SELECT table_name,table_type,table_rows FROM information_schema.tables WHERE table_schema= \'asaq\' ",conn);
                var dr=cmd.ExecuteReader();
                while(dr.Read())
                {
                    result.Add(new Tables{
                        Name=dr["table_name"].ToString(),
                        Type=dr["table_type"].ToString(),
                        Rows = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr,"table_rows"))

                    });
                }
            }

            return result;
        }
        public static List<Tables> Check()
        {


            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                var name = "inspections";
                     var result =new List<Tables>();
                conn.Open();
                var cmd = Utils.GetCommand(string.Format("SELECT * table_name FROM {0}  ",name), conn);
                
                cmd.Parameters.AddWithValue("@table_name",name);

                var dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    result.Add(new Tables
                    {
                       

                    });
                }
                return result;
            }



            
        }
    }

    public class Tables
    {
        public string Name { get; set; }
        public int? Rows { get; set; }
        public string Type { get; set; }
        public string ColumnNames { get; set; }


    } 
}
