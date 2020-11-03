
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;

namespace erp.Model.Dal.New
{
    public class InspectionsLoadingDal : GenericDal<Inspections_loading>, IInspectionsLoadingDal
    {
		protected override string GetAllSql()
		{
			return "SELECT * FROM inspections_loading";
		}

		protected override string GetByIdSql()
		{
			return "SELECT * FROM inspections_loading WHERE loading_line_unique = @id";
		}

		protected override string GetCreateSql()
		{
			return
				@"INSERT INTO inspections_loading (insp_line_unique,container,qty_per_pallet,full_pallets,loose_load_qty,mixed_pallet_qty,
				mixed_pallet_qty2,mixed_pallet_qty3) VALUES(@insp_line_unique,@container,@qty_per_pallet,@full_pallets,@loose_load_qty,
				@mixed_pallet_qty,@mixed_pallet_qty2,@mixed_pallet_qty3)";
		}

		protected override string GetUpdateSql()
		{
			return
				@"UPDATE inspections_loading SET insp_line_unique = @insp_line_unique,container = @container,qty_per_pallet = @qty_per_pallet,
			full_pallets = @full_pallets,loose_load_qty = @loose_load_qty,mixed_pallet_qty = @mixed_pallet_qty,mixed_pallet_qty2 = @mixed_pallet_qty2,
				mixed_pallet_qty3 = @mixed_pallet_qty3 WHERE loading_line_unique = @loading_line_unique";
		}

		protected override string GetDeleteSql()
		{
			return "DELETE FROM inspections_loading WHERE loading_line_unique = @id";
		}

		
        public  List<Inspections_loading> GetForInspection(int insp_id)
        {
            return GetForInspection(new[] { insp_id });
        }

        public  List<Inspections_loading> GetForInspection(IList<int> insp_ids)
        {
	        return conn.Query<Inspections_loading, Inspection_lines_tested, Containers, Inspections_loading>(
		        $@"SELECT inspections_loading.*,inspection_lines_tested.*, containers.*  FROM inspections_loading INNER JOIN inspection_lines_tested 
                 ON inspections_loading.insp_line_unique = inspection_lines_tested.insp_line_unique 
                INNER JOIN containers ON inspections_loading.container = containers.container_id 
                WHERE containers.insp_id IN @insp_ids AND inspection_lines_tested.insp_id IN @insp_ids",
		        (l, il, c) =>
		        {
			        l.LineTested = il;
			        l.Container = c;
			        return l;
		        }, new {insp_ids}, splitOn:"insp_line_unique, container_id").ToList();
			
        }
		
		
		public InspectionsLoadingDal(IDbConnection conn) : base(conn)
		{
		}

		protected override string IdField => "loading_line_unique";
	}
}
			
			