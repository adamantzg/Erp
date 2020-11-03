using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;

namespace erp.Model.DAL
{
    public class InspectionsV2DAL
    {
        /// <summary>
        /// Optimized version. Fills only some lists and fields
        /// </summary>
        /// <param name="factory_ids"></param>
        /// <param name="client_ids"></param>
        /// <param name="custpo"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="statuses"></param>
        /// <param name="inspectorId"></param>
        /// <returns></returns>
        public static List<Inspection_v2> GetByCriteria(IList<int> factory_ids, IList<int> client_ids, string custpo,
            DateTime? from,
            DateTime? to, IList<InspectionV2Status?> statuses = null, int? inspectorId = null)
        {

            var result = new List<Inspection_v2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString))
            {
                conn.Open();
                var cmd = new MySqlCommand("", conn);
                cmd.CommandText =
                    string.Format(@"SELECT  inspection_v2.id, inspection_v2.startdate, GROUP_CONCAT(DISTINCT order_header.custpo) AS custpos,
                                    inspection_v2.type, inspection_v2.custpo,inspection_v2.factory_id,inspection_v2.`code`,inspection_v2.client_id,
                                    inspection_v2.duration,inspection_v2.comments,inspection_v2.qc_required, inspection_v2.comments_admin,
                                    inspection_v2.insp_status,inspection_v2.acceptance_fc,inspection_v2.insp_batch_inspection, factory.factory_code, client.customer_code
                                    FROM inspection_v2
                                    INNER JOIN inspection_v2_line ON inspection_v2_line.insp_id = inspection_v2.id
                                    INNER JOIN order_lines ON inspection_v2_line.orderlines_id = order_lines.linenum
                                    INNER JOIN order_header ON order_header.orderid = order_lines.orderid
                                    INNER JOIN users factory ON inspection_v2.factory_id = factory.user_id
                                    INNER JOIN users client ON inspection_v2.client_id = client.user_id
                                    WHERE (startdate >= @from OR @from IS NULL) AND (startdate <= @to OR @to IS NULL) {0} {1} {2} {3} {4}
                                    GROUP BY inspection_v2.id",
                        factory_ids != null
                            ? string.Format(" AND inspection_v2.factory_id IN ({0})",
                                Utils.CreateParametersFromIdList(cmd, factory_ids, "factoryId"))
                            : "",
                        client_ids != null
                            ? string.Format(" AND inspection_v2.client_id IN ({0})",
                                Utils.CreateParametersFromIdList(cmd, factory_ids, "clientId"))
                            : "",
                        !string.IsNullOrEmpty(custpo) ? " AND order_header.custpo LIKE '%' + @custpo + '%'" : "",
                        statuses != null
                            ? string.Format(" AND insp_status IN ({0})",
                                Utils.CreateParametersFromIdList(cmd, statuses, "status"))
                            : "",
                        inspectorId != null
                            ? " AND EXISTS (SELECT * FROM inspection_v2_controller WHERE inspection_id = inspection_v2.id AND controller_id = @controllerid)"
                            : ""
                        );
                cmd.Parameters.AddWithValue("@from", from);
                cmd.Parameters.AddWithValue("@to", to);
                cmd.Parameters.AddWithValue("@custpo", custpo);
                cmd.Parameters.AddWithValue("@controllerid", inspectorId);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var insp = GetFromDataReader(dr);
                    insp.Lines = new List<Inspection_v2_line>();
                    var custPos = string.Empty + dr["custpos"];
                    var custPosList = custPos.Split(',');
                    foreach (var cpo in custPosList)
                    {
                        insp.Lines.Add(new Inspection_v2_line{OrderLine = new Order_lines{Header = new Order_header{custpo = cpo}}});
                    }
                    insp.Factory = new Company {factory_code = string.Empty + dr["factory_code"]};
                    insp.Client = new Company {customer_code = string.Empty + dr["customer_code"]};
                    result.Add(insp);
                }
                return result;
            }

        }

        public static Inspection_v2 GetFromDataReader(MySqlDataReader dr)
        {
            Inspection_v2 o = new Inspection_v2();

            o.id = (int)dr["id"];
            o.startdate = Utilities.FromDbValue<DateTime>(Utilities.GetReaderField(dr, "startdate"));
            o.type = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "type"));
            o.custpo = string.Empty + Utilities.GetReaderField(dr, "custpo");
            o.factory_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "factory_id"));
            o.code = string.Empty + Utilities.GetReaderField(dr, "code");
            o.client_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "client_id"));
            o.duration = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "duration"));
            o.comments = string.Empty + Utilities.GetReaderField(dr, "comments");
            o.qc_required = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "qc_required"));
            o.comments_admin = string.Empty + Utilities.GetReaderField(dr, "comments_admin");
            o.insp_status = (InspectionV2Status) Enum.Parse(typeof(InspectionV2Status),string.Empty + Utilities.GetReaderField(dr, "insp_status"));
            //o.acceptance_fc = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "acceptance_fc"));
            o.insp_batch_inspection = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "insp_batch_inspection"));
            o.drawingFile = Utilities.GetStringOrNull(dr["drawingFile"]);
            return o;

        }

        public static Inspection_v2 GetById(int id, bool loadLoadings = false,bool loadImages = false, IDbConnection conn = null)
        {
            /*m.InspectionsV2.Include("Lines")
                 .Include("Factory").Include("Client")
                 .Include("Lines.Loadings")
                 .Include("Lines.Loadings.Area")
                 .Include("Lines.OrderLine")
                 .Include("Lines.OrderLine.Cust_Product")
                 .Include("Lines.OrderLine.Cust_Product.MastProduct")
                 .Include("Lines.OrderLine.Header")
                 .Include("Containers")
                 .Include("Containers.Images")
                 .Include("Controllers")
                 .Include("Controllers.Controller")
                 .FirstOrDefault(i => i.id == id);
            if (insp != null && insp.Lines != null) {
                foreach (var l in insp.Lines) {
                    //Can't load first time, SQL gets f.. up
                    m.Entry(l).Collection("Images").Load();
                    if (loadRejections)
                        m.Entry(l).Collection("Rejections").Load();
                }
            }*/

            Inspection_v2 result = null;
            
            var dispose = false;
            if(conn == null) {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
                dispose = true;
            }
            
            {
                
                var cmd = new MySqlCommand(@"SELECT inspection_v2.*,factory.factory_code, client.customer_code, inspection_v2_type.name AS typeName
                                             FROM inspection_v2 
                                             INNER JOIN users factory ON inspection_v2.factory_id = factory.user_id
                                             INNER JOIN users client ON inspection_v2.client_id = client.user_id 
                                             INNER JOIN inspection_v2_type ON inspection_v2.type = inspection_v2_type.id
                                             WHERE inspection_v2.id = @id", (MySqlConnection) conn);
                cmd.Parameters.AddWithValue("@id", id);
                var dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    result = GetFromDataReader(dr);
                    result.Factory = new Company {factory_code = string.Empty + dr["factory_code"]};
                    result.Client = new Company {customer_code = string.Empty + dr["customer_code"]};
                    result.InspectionType = new Inspection_v2_type { id = Utilities.FromDbValue<int>(dr["type"]) ?? 0, name = string.Empty + dr["typeName"] };

                    dr.Close();

                    result.Lines = LoadLines(conn, id, loadLoadings: loadLoadings, insp: result, loadImages: loadImages);

                    cmd.CommandText = @"SELECT * FROM inspection_v2_container WHERE insp_id = @id";
                    dr = cmd.ExecuteReader();
                    result.Containers = new List<Inspection_v2_container>();
                    while (dr.Read()) {
                        var c = GetContainerFromDataReader(dr);
                        result.Containers.Add(c);
                    }
                    dr.Close();

                    cmd.CommandText = @"SELECT * FROM inspection_v2_controller INNER JOIN userusers ON inspection_v2_controller.controller_id = userusers.useruserid WHERE inspection_id = @id";
                    dr = cmd.ExecuteReader();
                    result.Controllers = new List<Inspection_v2_controller>();
                    while (dr.Read()) {
                        var c = GetControllerFromDataReader(dr);
                        c.Controller = UserDAL.GetFromDataReader(dr);
                        result.Controllers.Add(c);
                    }
                    dr.Close();

                    var sql = @"SELECT * FROM inspection_v2_mixedpallet WHERE insp_id = @id";
                    result.MixedPallets = conn.Query<Inspection_v2_mixedpallet>(sql,new { id = id }).ToList();                    

                }
                if (dispose)
                    conn.Dispose();
            }
            return result;
        }

        public static List<Inspection_v2_line> LoadLines(IDbConnection conn = null, int? id= null, IList<int> orderids = null, bool loadImages = false, bool loadLoadings = false, Inspection_v2 insp = null)
        {
            var dispose = conn == null;
            var result = new List<Inspection_v2_line>();
            if(conn == null)
            {
                conn = new MySqlConnection(Properties.Settings.Default.ConnString);
                conn.Open();
            }                

            var cmd = new MySqlCommand("", (MySqlConnection) conn);

            cmd.CommandText = $@"SELECT inspection_v2_line.*,order_lines.*,order_lines.cprod_id AS ol_cprod_id,order_header.custpo as oh_custpo,cust_products.cprod_code1,cust_products.cprod_name, cust_products.cprod_mast,
                                           mast_products.factory_ref, mast_products.factory_id,mast_products.special_comments,mast_products.units_per_40nopallet_hc, mast_products.units_per_40pallet_hc,mast_products.category1,
                                             factory.factory_code AS mastproduct_factorycode
                                             FROM  
                                             order_lines 
                                             INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                             INNER JOIN cust_products ON order_lines.cprod_id = cust_products.cprod_id
                                             INNER JOIN mast_products ON cust_products.cprod_mast = mast_products.mast_id
                                             INNER JOIN users factory ON mast_products.factory_id = factory.user_id
                                             RIGHT OUTER JOIN inspection_v2_line ON inspection_v2_line.orderlines_id = order_lines.linenum
                                             WHERE {(orderids != null ? $"order_lines.orderid IN ({string.Join(",", orderids)})" : "inspection_v2_line.insp_id = @id" )}";
            cmd.Parameters.AddWithValue("@id", id);
            var dr = cmd.ExecuteReader();
            
            while (dr.Read())
            {
                var line = GetLineFromDataReader(dr);
                line.Inspection = insp ?? GetSimple(line.insp_id ?? 0, conn);
                result.Add(line);
            }
            dr.Close();
            foreach (var l in result.Where(l => l.OrderLine == null && l.cprod_id != null))
            {
                l.Product = Cust_productsDAL.GetById(l.cprod_id.Value, true);
            }
            foreach (var l in result.Where(l => l.OrderLine != null && l.OrderLine.Cust_Product != null))
            {
                l.OrderLine.Cust_Product.Parents =
                    Cust_productsDAL.GetSpareParents(l.OrderLine.Cust_Product.cprod_id, conn);
            }

            if (loadImages && result.Count > 0)
            {
                cmd.CommandText = string.Format("SELECT * FROM inspection_v2_image WHERE insp_line IN ({0})", Utils.CreateParametersFromIdList(cmd, result.Select(l => l.id).ToList()));
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var img = GetImageFromDataReader(dr);
                    var line = result.FirstOrDefault(l => l.id == img.insp_line);
                    if (line != null)
                    {
                        img.Line = line;
                        if (line.Images == null)
                            line.Images = new List<Inspection_v2_image>();
                        line.Images.Add(img);
                    }
                }
                dr.Close();
            }

            if (loadLoadings && result.Count > 0)
            {
                cmd.CommandText = "SELECT * FROM inspection_v2_area";
                dr = cmd.ExecuteReader();
                var areas = new List<Inspection_v2_area>();
                while (dr.Read())
                {
                    areas.Add(new Inspection_v2_area { id = (int)dr["id"], name = string.Empty + dr["name"] });
                }
                dr.Close();
                
                var lineIds = result.Select(l => l.id).ToList();
                var cmdList = Utils.CreateParametersFromIdList(cmd, lineIds);

                
                
                cmd.CommandText = $@"SELECT inspection_v2_loading.*, GROUP_CONCAT(inspection_v2_loading_area.area_id) AS areas, inspection_v2_container.* FROM inspection_v2_loading LEFT OUTER JOIN inspection_v2_loading_area
                                    ON inspection_v2_loading_area.loading_id = inspection_v2_loading.id LEFT OUTER JOIN inspection_v2_container ON inspection_v2_loading.container_id = 
                                inspection_v2_container.id
                                WHERE inspection_v2_loading.insp_line IN ({cmdList}) 
                                GROUP BY inspection_v2_loading.id";
                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    var loading = GetLoadingFromDataReader(dr, areas);
                    loading.Container = loading.container_id != null ? GetContainerFromDataReader(dr) : null;
                    var line = result.FirstOrDefault(l => l.id == loading.insp_line);
                    if (line != null)
                    {
                        loading.Line = line;
                        if (line.Loadings == null)
                            line.Loadings = new List<Inspection_v2_loading>();
                        line.Loadings.Add(loading);
                    }
                }
                dr.Close();
                
                
                cmd.CommandText = $@"SELECT * FROM inspection_v2_loading_mixedpallet WHERE loading_id = @id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", 0);
                foreach (var l in result.Where(r=>r.Loadings != null).SelectMany(r=>r.Loadings))
                {
                    cmd.Parameters[0].Value = l.id;
                    dr = cmd.ExecuteReader();
                    l.QtyMixedPallets = new List<Inspection_v2_loading_mixedpallet>();
                    while (dr.Read())
                    {
                        l.QtyMixedPallets.Add(
                            new Inspection_v2_loading_mixedpallet
                            {
                                pallet_id = Utilities.FromDbValue<int>(dr["pallet_id"]),
                                loading_id = l.id,
                                qty = Utilities.FromDbValue<int>(dr["qty"])
                            });
                    }
                    dr.Close();
                }
            }

            if (dispose)
                conn.Dispose();
            return result;
        }

        //public static List<Inspection_v2> GetCombinedOrderInspections(int orderid, bool loadLoadings = false)
        //{
        //    var result = new List<Inspection_v2>();
        //    using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
        //        conn.Open();
        //        var cmd = new MySqlCommand(@"SELECT DISTINCT insp_id FROM inspection_v2_line INNER JOIN order_lines ON inspection_v2_line.orderlines_id = order_lines.linenum 
        //                                     INNER JOIN order_header ON order_lines.orderid = order_header.orderid
        //                                     WHERE order_header.combined_order = @orderid", conn);
        //        cmd.Parameters.AddWithValue("@orderid", orderid);
        //        var dr = cmd.ExecuteReader();
        //        var ids = new List<int>();
        //        while(dr.Read()) {
        //            ids.Add((int)dr["insp_id"]);                    
        //        }
        //        dr.Close();
        //        foreach(var id in ids) {
        //            result.Add(GetById(id, loadLoadings: loadLoadings, conn: conn));
        //        }
        //    }
        //    return result;
        // }

        public static List<Inspection_v2> GetOrderInspections(int orderid, bool loadLoadings = false)
        {
            var result = new List<Inspection_v2>();
            using (var conn = new MySqlConnection(Properties.Settings.Default.ConnString)) {
                conn.Open();
                var cmd = new MySqlCommand(@"SELECT DISTINCT insp_id FROM inspection_v2_line INNER JOIN order_lines ON inspection_v2_line.orderlines_id = order_lines.linenum 
                                             INNER JOIN order_header ON order_lines.orderid = order_header.orderid
                                             WHERE order_header.orderid = @orderid", conn);
                cmd.Parameters.AddWithValue("@orderid", orderid);
                var dr = cmd.ExecuteReader();
                var ids = new List<int>();
                while (dr.Read()) {
                    ids.Add((int)dr["insp_id"]);
                }
                dr.Close();
                var orderLineNumbers = conn.Query<int?>("SELECT linenum FROM order_lines WHERE orderid = @id", new { id = orderid }).ToList();
                foreach(var id in ids) { 
                    var insp = GetById(id,loadLoadings: loadLoadings);
                    insp.Lines = insp.Lines.Where(l => orderLineNumbers.Contains(l.orderlines_id)).ToList();
                    result.Add(insp);
                }
            }
            return result;
        }

        public static Inspection_v2_line GetLineFromDataReader(MySqlDataReader dr)
        {
            Inspection_v2_line o = new Inspection_v2_line();

            o.id = (int)dr["id"];
            o.insp_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "insp_id"));
            o.orderlines_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "orderlines_id"));
            o.insp_mastproduct_code = string.Empty + Utilities.GetReaderField(dr, "insp_mastproduct_code");
            o.insp_custproduct_code = string.Empty + Utilities.GetReaderField(dr, "insp_custproduct_code");
            o.insp_custproduct_name = string.Empty + Utilities.GetReaderField(dr, "insp_custproduct_name");
            o.qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "qty"));
            o.custpo = string.Empty + Utilities.GetReaderField(dr, "custpo");
            o.cprod_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "cprod_id"));
            o.inspected_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "inspected_qty"));
            o.comments = string.Empty + Utilities.GetReaderField(dr, "comments");
            o.factory_code = string.Empty + string.Empty + Utilities.GetReaderField(dr, "factory_code");

            if (o.orderlines_id != null) {
                o.OrderLine = new Order_lines
                {
                    cprod_id = Utilities.FromDbValue<int>(dr["ol_cprod_id"]),
                    orderid = Utilities.FromDbValue<int>(dr["orderid"]),
                    Cust_Product = new Cust_products
                    {
                        cprod_id = Utilities.FromDbValue<int>(dr["ol_cprod_id"]) ?? 0,
                        cprod_code1 = string.Empty + dr["cprod_code1"],
                        cprod_name = string.Empty + dr["cprod_name"],
                        cprod_mast = Utilities.FromDbValue<int>(dr["cprod_mast"]),
                        MastProduct = new Mast_products
                        {
                            factory_ref = string.Empty + dr["factory_ref"],
                            factory_id = Utilities.FromDbValue<int>(dr["factory_id"]),
                            special_comments = string.Empty + dr["special_comments"],
                            units_per_40nopallet_hc = Utilities.FromDbValue<int>(dr["units_per_40nopallet_hc"]),
                            units_per_40pallet_hc = Utilities.FromDbValue<int>(dr["units_per_40pallet_hc"]),
                            Factory = new Company { factory_code = string.Empty + (Utilities.ColumnExists(dr, "mastproduct_factorycode") ? dr["mastproduct_factorycode"] : null) },
                            category1 = Utilities.FromDbValue<int>(dr["category1"])
                        }
                    },
                    Header = new Order_header { custpo = string.Empty + dr["oh_custpo"], orderid = Utilities.FromDbValue<int>(dr["orderid"]) ?? 0 },
                    orderqty = Utilities.FromDbValue<double>(dr["orderqty"])
                };
            }
            

            return o;

        }

        public static Inspection_v2_container GetContainerFromDataReader(MySqlDataReader dr)
        {
            Inspection_v2_container o = new Inspection_v2_container();

            o.id = (int)dr["id"];
            o.insp_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "insp_id"));
            o.container_no = string.Empty + Utilities.GetReaderField(dr, "container_no");
            o.seal_no = string.Empty + Utilities.GetReaderField(dr, "seal_no");
            o.container_size = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "container_size"));
            o.container_count = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "container_count"));
            o.container_space = Utilities.FromDbValue<double>(Utilities.GetReaderField(dr, "container_space"));
            o.container_comments = string.Empty + Utilities.GetReaderField(dr, "container_comments");

            return o;

        }

        public static Inspection_v2_controller GetControllerFromDataReader(MySqlDataReader dr)
        {
            Inspection_v2_controller o = new Inspection_v2_controller();

            o.id = (int)dr["id"];
            o.inspection_id = (int)dr["inspection_id"];
            o.controller_id = (int)dr["controller_id"];
            o.startdate = (DateTime)dr["startdate"];
            o.duration = (int)dr["duration"];

            return o;
        }

        

        public static Inspection_v2_image GetImageFromDataReader(MySqlDataReader dr)
        {
            Inspection_v2_image o = new Inspection_v2_image();

            o.id = (int)dr["id"];
            o.insp_line = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "insp_line"));
            o.insp_image = string.Empty + Utilities.GetReaderField(dr, "insp_image");
            o.type_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "type_id"));
            o.rej_flag = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "rej_flag"));
            o.order = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "order"));
            o.comments = string.Empty + Utilities.GetReaderField(dr, "comments");

            return o;
        }

        public static Inspection_v2_loading GetLoadingFromDataReader(MySqlDataReader dr, List<Inspection_v2_area> areas)
        {
            Inspection_v2_loading o = new Inspection_v2_loading();

            o.id = (int)dr["id"];
            o.insp_line = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "insp_line"));
            o.container_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "container_id"));
            o.full_pallets = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "full_pallets"));
            o.loose_load_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "loose_load_qty"));
            o.mixed_pallet_qty = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "mixed_pallet_qty"));
            o.mixed_pallet_qty2 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "mixed_pallet_qty2"));
            o.mixed_pallet_qty3 = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "mixed_pallet_qty3"));
            o.area_id = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "area_id"));
            o.qty_per_pallet = Utilities.FromDbValue<int>(Utilities.GetReaderField(dr, "qty_per_pallet"));
            /*if (o.area_id != null)
            {
                o.Area = new Inspection_v2_area {id = o.area_id.Value, name = string.Empty + dr["name"]};
            }*/
            var areas_id = string.Empty + Utilities.GetReaderField(dr, "areas");
            if(!string.IsNullOrEmpty(areas_id)) {
                var ids = areas_id.Split(',').Select(int.Parse).ToList();
                o.Areas = ids.Select(id => new Inspection_v2_area { id = id, name = areas.FirstOrDefault(a=>a.id == id)?.name }).ToList();
            }

            return o;

        }

        public static Inspection_v2 GetSimple(int id, IDbConnection conn)
        {
            Inspection_v2 result = null;
            var cmd = new MySqlCommand("SELECT * FROM inspection_v2 WHERE id = @id", (MySqlConnection)conn);
            cmd.Parameters.AddWithValue("@id", id);
            var dr = cmd.ExecuteReader();
            if(dr.Read())
            {
                result = GetFromDataReader(dr);
            }
            dr.Close();
            return result;
        }

       

    }
}
