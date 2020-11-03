using erp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefactorThis.GraphDiff;
using System.Data.Entity;

namespace erp.DAL.EF.New
{
	public class NrHeaderRepository : GenericRepository<Nr_header>, INrHeaderRepository
	{
		public NrHeaderRepository(DbContext context) : base(context)
		{
		}

		public override void Update(Nr_header nr)
        {
            context.UpdateGraph(nr, map => map.OwnedCollection(n => n.Lines).OwnedCollection(n=>n.Images));
        }

        public List<NrReportInspectionRow> GetInspectionsForNtReport_NoNrHeaders(DateTime change_notice_from, DateTime etdReadyDate)
        {
            return context.Database.SqlQuery<NrReportInspectionRow>(
						@"SELECT
                            inspection_v2_line.id insp_line_id,
                            inspection_v2_line.insp_id,
                            inspection_v2.factory_id,
                            factory.factory_code,
                            inspection_v2.custpo as insp_custpo,
                            inspection_v2_line.insp_custproduct_code,
                            v_lines.cprod_id,
                            v_lines.orderid,
                            v_lines.custpo order_custpo,
                            v_lines.userid1,
                            v_lines.customer_code,
                            v_lines.req_eta,
                            v_lines.po_req_etd,
                            IF(orders.change_notice_allocation_id IS NOT NULL,1,0) orderAllocated, 
                            inspection_v2.startdate,
                            change_notice.id as change_notice_id
                           FROM v_lines
                            INNER JOIN change_notice_allocation ON v_lines.cprod_id = change_notice_allocation.cprod_id 
                            INNER JOIN change_notice ON change_notice_allocation.notice_id = change_notice.id
                            INNER JOIN 
                                (SELECT change_notice_allocation_order.change_notice_allocation_id, change_notice_allocation_order.orderid, NULL AS cprod_id, MIN(po_req_etd) AS po_req_etd
                                 FROM  change_notice_allocation_order INNER JOIN order_header ON change_notice_allocation_order.orderid = order_header.orderid 
                                 INNER JOIN porder_header ON porder_header.soorderid = order_header.orderid 
								GROUP BY order_header.orderid,change_notice_allocation_order.change_notice_allocation_id
                                 UNION 
                                 SELECT NULL AS change_notice_allocation_id, l.orderid, l.cprod_id, NULL AS po_req_etd
                                 FROM v_lines l LEFT OUTER JOIN change_notice_allocation_order ON l.orderid = change_notice_allocation_order.orderid 
                                 WHERE change_notice_allocation_order.change_notice_allocation_id IS NULL AND l.po_req_etd >= @p1
                                ) orders
                             ON (change_notice_allocation.id = orders.change_notice_allocation_id OR v_lines.cprod_id = orders.cprod_id) AND orders.orderid = v_lines.orderid
                            LEFT OUTER JOIN inspection_v2_line ON v_lines.linenum = inspection_v2_line.orderlines_id
                            LEFT OUTER JOIN inspection_v2 ON inspection_v2_line.insp_id = inspection_v2.id
                            LEFT OUTER JOIN users factory ON inspection_v2.factory_id = factory.user_id
                            WHERE (change_notice.expectedReadyDate <= @p1 OR orders.po_req_etd > NOW())
                            AND change_notice.datecreated >= @p0
                            AND NOT EXISTS (SELECT insp_v2_id FROM nr_header WHERE nr_type_id = 2 AND change_notice_id = change_notice.id);", change_notice_from,etdReadyDate).ToList();
        }

		/// <summary>
		/// Returns inspection rows for lines that already have nr_headers but maybe some order later got inspection record and it should be now visible
		/// </summary>
		/// <param name="change_notice_from"></param>
		/// <param name="etdReadyDate"></param>
		/// <returns></returns>
		public List<NrReportInspectionRow> GetInspectionsForNtReport_NrHeaders(DateTime change_notice_from, DateTime etdReadyDate)
		{
			return context.Database.SqlQuery<NrReportInspectionRow>(
				@"			SELECT 
							inspection_v2_line.id insp_line_id,
                            inspection_v2_line.insp_id,
                            inspection_v2.factory_id,
                            factory.factory_code,
                            inspection_v2.custpo as insp_custpo,
                            inspection_v2_line.insp_custproduct_code,
                            v_lines.cprod_id,
                            v_lines.orderid,
                            v_lines.custpo order_custpo,
                            v_lines.userid1,
                            v_lines.customer_code,
                            v_lines.req_eta,
                            v_lines.po_req_etd,
                            IF(orders.change_notice_allocation_id IS NOT NULL,1,0) orderAllocated, 
                            inspection_v2.startdate,
                            change_notice.id as change_notice_id/*,
                            nr_headers.insp_v2_id,
                            nr_headers.change_notice_id*/
                           FROM v_lines
                            INNER JOIN change_notice_allocation ON v_lines.cprod_id = change_notice_allocation.cprod_id 
                            INNER JOIN change_notice ON change_notice_allocation.notice_id = change_notice.id
                            INNER JOIN 
                                (SELECT change_notice_allocation_order.change_notice_allocation_id, change_notice_allocation_order.orderid, NULL AS cprod_id, MIN(po_req_etd) AS po_req_etd
                                 FROM  change_notice_allocation_order INNER JOIN order_header ON change_notice_allocation_order.orderid = order_header.orderid 
                                 INNER JOIN porder_header ON porder_header.soorderid = order_header.orderid 
								 GROUP BY order_header.orderid,change_notice_allocation_order.change_notice_allocation_id
                                 UNION 
                                 SELECT NULL AS change_notice_allocation_id, l.orderid, l.cprod_id, NULL AS po_req_etd
                                 FROM v_lines l LEFT OUTER JOIN change_notice_allocation_order ON l.orderid = change_notice_allocation_order.orderid 
                                 WHERE change_notice_allocation_order.change_notice_allocation_id IS NULL AND l.po_req_etd >= @p1
                                ) orders
                             ON (change_notice_allocation.id = orders.change_notice_allocation_id OR v_lines.cprod_id = orders.cprod_id) AND orders.orderid = v_lines.orderid
                            INNER JOIN inspection_v2_line ON v_lines.linenum = inspection_v2_line.orderlines_id
                            INNER JOIN inspection_v2 ON inspection_v2_line.insp_id = inspection_v2.id
                            INNER JOIN users factory ON inspection_v2.factory_id = factory.user_id
                            WHERE (change_notice.expectedReadyDate <= @p1 OR orders.po_req_etd > NOW())
                            AND change_notice.datecreated >= @p0                            
                            AND NOT EXISTS (SELECT insp_v2_id FROM nr_header WHERE nr_type_id = 2 AND insp_v2_id = inspection_v2_line.insp_id)
                            AND inspection_v2.datecreated > change_notice.datecreated
                            AND change_notice.status = 0                            
                            AND NOT EXISTS (SELECT nr_header.id
										FROM nr_header INNER JOIN nr_lines ON nr_header.id = nr_lines.NR_id 
										INNER JOIN inspection_v2_line ON nr_lines.inspection_lines_v2_id = inspection_v2_line.id
										INNER JOIN v_lines vl ON inspection_v2_line.orderlines_id = vl.linenum
										WHERE nr_header.change_notice_id = change_notice.id AND vl.cprod_id = v_lines.cprod_id AND vl.userid1 = v_lines.userid1)
                            ORDER BY change_notice.id, inspection_v2.id, v_lines.cprod_id", change_notice_from, etdReadyDate).ToList();
		}
	}
}
