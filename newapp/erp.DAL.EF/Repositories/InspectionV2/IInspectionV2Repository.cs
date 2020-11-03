using System;
using System.Collections.Generic;
using erp.Model;

namespace erp.DAL.EF.Repositories.InspectionV2
{
    public interface IInspectionV2Repository
    {
        Inspection_v2 GetById(int id, bool loadRejections = false);
        void CreateFromOrders(Inspection_v2 insp, IList<int> orderids, bool includeCombinedOrders = false);
        void Create(Inspection_v2 insp);
        void UpdateLoading(Inspection_v2 insp);
        void UpdateCombinedLoadings(List<Inspection_v2_loading> loadings);
        void UpdateFinal(Inspection_v2 insp);
        void UpdateSimple(int id, DateTime? startDate);
        void UpdateStatus(int id, InspectionV2Status status);
        void BulkUpdateLines(IList<Inspection_v2_line> lines);
        List<Inspection_v2_image_type> GetImageTypes();
        List<Inspection_v2> GetByCriteria(DateTime from , DateTime to , int? location_id);

        List<Inspection_v2> GetByCriteria(IList<int> factory_ids, IList<int> client_ids, string custpo, DateTime? from,
            DateTime? to,IList<InspectionV2Status?> statuses = null, int? inspectorId = null);

        void Delete(int id);

        List<Inspection_v2_area> GetAreas();
    }
}