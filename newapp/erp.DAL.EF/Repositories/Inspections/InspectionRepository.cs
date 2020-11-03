using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.DAL.EF.Repositories;
using erp.Model;

namespace erp.DAL.EF
{
    public class InspectionRepository : GenericRepository<Inspections>
    {
        public InspectionRepository(Model context) : base(context)
        {
        }

        public static List<Qc_comments> GetQcCommentsInPeriod(DateTime? from, DateTime? to)
        {
            using (var m = Model.CreateModel())
            {
                return m.QcComments.Include("Inspection").Include("Creator").Where(c => c.insp_comments_date >= from && c.insp_comments_date <= to).ToList();
            }
        }

        public static inspection_notified_summary_table GetNotifiedSummaryById(int id)
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionNotifiedSummary.FirstOrDefault(i => i.insp_nr_unique == id);
            }
        }

        public static List<inspection_lines_notified_v2_table> GetLinesNotifiedByNrUnique(int nr_unique_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionLinesNotified.Where(l => l.insp_nr_unique == nr_unique_id).ToList();
            }
        }

        public static List<inspection_notified_summary_images_table> GetNotifiedSummaryImagesByNrId(int nr_unique_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionNotifiedSummaryImages.Where(im => im.insp_nr_unique == nr_unique_id).ToList();
            }
        }

        public static List<inspection_images_v2_table> GetInspectionImagesByLineId(int line_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionImages.Where(i => i.insp_line_unique == line_id).ToList();
            }
        }

        

        public List<Inspection_lines_tested> GetLinesForOrders(IList<int> orderIds)
        {
            var result = new List<Inspection_lines_tested>();

            var inspections =
                    (context as Model).Inspections.Include("LinesTested.OrderLine").Include("LinesTested.Inspection")
                        .Where(i => i.LinesTested.Any(l => orderIds.Contains(l.OrderLine.orderid.Value)))
                        .ToList();
            foreach (var i in inspections) {
                if (i.LinesTested != null)
                    result.AddRange(i.LinesTested);
            }

            return result;
        }

        
    }
}
