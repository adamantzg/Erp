using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using company.Common;
using erp.Model;
using LinqKit;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.Repositories.InspectionV2
{
    public class InspectionV2Repository : GenericRepository<Inspection_v2>,IInspectionV2Repository
    {
        public InspectionV2Repository(Model context) : base(context)
        {
        }

        public Inspection_v2 GetById(int id, bool loadRejections = false)
        {
            using (var m = Model.CreateModel())
            {
                var insp = 
                  m.InspectionsV2.Include("Lines")
                  .Include(i=>i.Factory).Include(i=>i.Client)
                  .Include(i=>i.InspectionType)
                  .Include(i=>i.MixedPallets)
                  .Include("Lines.Loadings")
                  .Include("Lines.OrderLine")
                  .Include("Lines.OrderLine.Cust_Product")
                  .Include("Lines.OrderLine.Cust_Product.MastProduct")
                  .Include("Lines.OrderLine.Header")
                  .Include(i=>i.Containers)
                  .Include("Containers.Images")
                  .Include(i=>i.Controllers)
                  .Include("Controllers.Controller")
                  .FirstOrDefault(i => i.id == id);
                if (insp != null && insp.Lines != null)
                {
                    foreach (var l in insp.Lines)
                    {
                        //Can't load first time, SQL gets f.. up
                        m.Entry(l).Collection("Images").Load();
                        if(loadRejections)
                            m.Entry(l).Collection("Rejections").Load();
                        foreach(var lo in l.Loadings) {
                            m.Entry(lo).Collection("Areas").Load();
                            m.Entry(lo).Collection("QtyMixedPallets").Load();
                        }
                    }
                }
                return insp;
            }
        }

        public void Create(Inspection_v2 insp)
        {
            using (var m = Model.CreateModel())
            {
				insp.dateCreated = DateTime.Now;
				m.InspectionsV2.Add(insp);		
                m.SaveChanges();
            }
        }

        public void CreateFromOrders(Inspection_v2 insp, IList<int> orderids, bool includeCombinedOrders = false)
        {
            using (var m = Model.CreateModel())
            {
				insp.dateCreated = DateTime.Now;
				m.InspectionsV2.Add(insp);
                if(insp.Lines == null)
                    insp.Lines = new List<Inspection_v2_line>();
                //var lines = Order_linesDAL.GetByOrderIds(orderids);
                var lines = m.OrderLines.Include(l=>l.Header).Include("Cust_Product.MastProduct").
                    Where(l => l.orderid != null && orderids.Contains(l.orderid.Value)).ToList();
                if (!includeCombinedOrders)
                    lines = lines.Where(l => l.Header.combined_order <= 0).ToList();
                foreach (var g in lines.GroupBy(l => l.orderid))
                {
                    insp.Lines.AddRange(g.Select(l=>
                            new Inspection_v2_line { 
                                insp_custproduct_code = l.IfNotNull(li=>li.Cust_Product.cprod_code1), 
                                insp_custproduct_name = l.IfNotNull(li=>li.Cust_Product.cprod_name),
                                insp_mastproduct_code = l.IfNotNull(li=>li.Cust_Product.MastProduct.IfNotNull(mp=>mp.factory_ref)),
                                qty = /*l.orderqty != null ? (int?) Convert.ToInt32(l.orderqty) : null,*/ null,
                                orderlines_id = l.linenum,
                                Loadings = insp.type == Inspection_v2_type.Loading ? 
                                            new List<Inspection_v2_loading>(){new Inspection_v2_loading{Container = company.Common.Utilities.SafeGetElement(insp.Containers,0)}} : null }));
                    
                }

                //m.SaveChanges();

                try
                {
                    Debug.WriteLine("Save: ");
                    m.SaveChanges();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Greška save: ");

                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.Data);
                    throw;
                }
               
            }
        }

        public void UpdateLoading(Inspection_v2 insp)
        {
            //using (var m = Model.CreateModel())
            //{                
            //UpdateGraph crashes - TODO: debug
            /*context.UpdateGraph(insp, 
                            map => map.OwnedCollection(i => i.Containers, 
                                        map1=>map1.OwnedCollection(c=>c.Images))
                                      .OwnedCollection(i=>i.Lines, 
                                        map1=>map1.OwnedCollection(l=>l.Loadings, 
                                             map2=>map2.OwnedCollection(lo=>lo.QtyMixedPallets)
                                                       .AssociatedCollection(lo=>lo.Areas))
                                       .OwnedCollection(l=>l.Images))
                          );*/
            var oldInsp = /*context.InspectionsV2.Include(i=>i.Containers).Include(i=>i.Controllers).Include(i=>i.MixedPallets)
                .Include("Containers.Images").Include("Lines.Loadings").FirstOrDefault(i => i.id == insp.id);*/
                    Get(i => i.id == insp.id, includeProperties: "Containers.Images, Controllers, MixedPallets,Lines.Loadings").FirstOrDefault();
            if (oldInsp != null)
            {
                context.Entry(oldInsp).CurrentValues.SetValues(insp);
                /*oldInsp.startdate = insp.startdate;
                oldInsp.duration = insp.duration;
                oldInsp.comments_admin = insp.comments_admin;*/
                if (insp.Containers != null) {
                    foreach (var c in oldInsp.Containers) {
                        var newcont = insp.Containers.FirstOrDefault(co => co.id == c.id);
                        if (newcont != null) {
                            /*c.container_no = newcont.container_no;
                            c.seal_no = newcont.seal_no;
                            c.container_size = newcont.container_size;
                            c.container_space = newcont.container_space;*/
                            context.Entry(c).CurrentValues.SetValues(newcont);

                            if (newcont.Images != null) {
                                foreach (var newIm in newcont.Images) {
                                    if (newIm.id <= 0)
                                        c.Images.Add(newIm);
                                    else {
                                        var oldim = c.Images.FirstOrDefault(i => i.id == newIm.id);
                                        if (oldim != null)
                                            oldim.insp_image = newIm.insp_image;
                                    }
                                }
                            }

                            //Delete missing
                            var deletedImages =
                                    c.Images.Where(im => newcont.Images == null || newcont.Images.Count(i => i.id == im.id) == 0).ToList();
                            foreach (var d in deletedImages) {
                                context.Entry(d).State = EntityState.Deleted;
                            }

                        }
                    }
                }                

                if (insp.Controllers != null) {
                    foreach (var c in insp.Controllers) {
                        if (c.id <= 0)
                            oldInsp.Controllers.Add(c);
                        else {
                            var oldC = oldInsp.Controllers.FirstOrDefault(co => co.id == c.id);
                            if (oldC != null) {
                                context.Entry(oldC).CurrentValues.SetValues(c);
                                /*oldC.startdate = c.startdate;
                                oldC.duration = c.duration;
                                oldC.controller_id = c.controller_id;*/
                            }
                        }

                    } 
                }
                if (oldInsp.Controllers != null) {
                    var deletedControllers =
                               oldInsp.Controllers.Where(co => insp.Controllers == null || insp.Controllers.Count(cnt => cnt.id == co.id) == 0).ToList();
                    foreach (var d in deletedControllers) {
                        context.Entry(d).State = EntityState.Deleted;
                    }
                }
                

                if (insp.MixedPallets != null) {
                    foreach (var p in insp.MixedPallets) {
                        if (p.id <= 0)
                            oldInsp.MixedPallets.Add(p);
                        else {
                            var oldP = oldInsp.MixedPallets.FirstOrDefault(pa => pa.id == p.id);
                            if (oldP != null) {
                                context.Entry(oldP).CurrentValues.SetValues(p);
                                /*oldC.startdate = c.startdate;
                                oldC.duration = c.duration;
                                oldC.controller_id = c.controller_id;*/
                            }
                        }

                    }
                }
                if(oldInsp.MixedPallets != null) {
                    var deletedMixedPallets =
                               oldInsp.MixedPallets.Where(mp => insp.MixedPallets == null || insp.MixedPallets.Count(mpa => mpa.id == mp.id) == 0).ToList();
                    foreach (var d in deletedMixedPallets) {
                        context.Entry(d).State = EntityState.Deleted;
                    }
                }
                var areas = context.Set<Inspection_v2_area>().ToList();

                if (oldInsp.Lines != null && insp.Lines != null) {
                    foreach (var oldLine in oldInsp.Lines) { 
                    var newLine = insp.Lines.FirstOrDefault(l => l.id == oldLine.id);
                        if (newLine != null) {
                            foreach (var oldLoading in oldLine.Loadings) {
                                var newLoading = newLine.Loadings?.FirstOrDefault(lo => lo.id == oldLoading.id);
                                if (newLoading != null) {
                                /*oldLoading.full_pallets = newLoading.full_pallets;
                                oldLoading.loose_load_qty = newLoading.loose_load_qty;
                                oldLoading.mixed_pallet_qty = newLoading.mixed_pallet_qty;
                                oldLoading.mixed_pallet_qty2 = newLoading.mixed_pallet_qty2;
                                oldLoading.mixed_pallet_qty3 = newLoading.mixed_pallet_qty3;
                                oldLoading.area_id = newLoading.area_id;
                                oldLoading.qty_per_pallet = newLoading.qty_per_pallet;*/
                                    context.Entry(oldLoading).CurrentValues.SetValues(newLoading);
                                    context.Entry(oldLoading).Collection("Areas").Load();
                                    if(newLoading.Areas != null && newLoading.Areas.Count > 0)
                                        oldLoading.Areas.AddRange(areas.Where(a=>newLoading.Areas.Count(newAr=>newAr.id == a.id) > 0 && oldLoading.Areas.Count(oldAr => oldAr.id == a.id) == 0));

                                    var deletedAreas =
                                    oldLoading.Areas.Where(ar => newLoading.Areas == null || newLoading.Areas.Count(i => i.id == ar.id) == 0).ToList();
                                    foreach (var d in deletedAreas) {
                                        oldLoading.Areas.Remove(d);
                                    }

                                    context.Entry(oldLoading).Collection("QtyMixedPallets").Load();
                                    if (newLoading.QtyMixedPallets != null) {
                                        foreach(var q in newLoading.QtyMixedPallets) {
                                            if (q.id <= 0)
                                                oldLoading.QtyMixedPallets.Add(q);
                                            else {
                                                var oldQ = oldLoading.QtyMixedPallets.FirstOrDefault(p => p.id == q.id);
                                            if (oldQ != null)
                                                context.Entry(oldQ).CurrentValues.SetValues(q);
                                            }
                                        }
                                    }                                        

                                    var deletedPallets =
                                    oldLoading.QtyMixedPallets.Where(q => newLoading.QtyMixedPallets == null || 
                                            newLoading.QtyMixedPallets.Count(i => i.id == q.id) == 0).ToList();
                                    foreach (var d in deletedPallets) {
                                        context.Entry(d).State = EntityState.Deleted;
                                    }
                                }
                            }
                        }
                    }

                    foreach (var oldLine in oldInsp.Lines) {
                        context.Entry(oldLine).Collection("Images").Load();
                        var newLine = insp.Lines.FirstOrDefault(l => l.id == oldLine.id);
                        if (newLine != null) {
                            if (newLine.Images != null) {
                                foreach (var newIm in newLine.Images) {
                                    if (newIm.id <= 0)
                                        oldLine.Images.Add(newIm);
                                    else {
                                        var oldim = oldLine.Images.FirstOrDefault(i => i.id == newIm.id);
                                        if (oldim != null) {
                                            oldim.insp_image = newIm.insp_image;
                                            oldim.order = newIm.order;
                                        }                                                
                                    }
                                }
                            }

                            //Delete missing
                            var deletedImages =
                                    oldLine.Images.Where(im => newLine.Images == null || newLine.Images.Count(i => i.id == im.id) == 0).ToList();
                            foreach (var d in deletedImages) {
                                context.Entry(d).State = EntityState.Deleted;
                            }

                        }
                    } 
                }
                    
            }

                //m.SaveChanges();

            //}
        }

        public void UpdateFinal(Inspection_v2 insp)
        {
            using (var m = Model.CreateModel())
            {
                var oldInsp = m.InspectionsV2.Include("Controllers")
                  .Include("Lines.Rejections").FirstOrDefault(i => i.id == insp.id);
                if (oldInsp != null) {
                    oldInsp.startdate = insp.startdate;
                    oldInsp.duration = insp.duration;
                    oldInsp.comments_admin = insp.comments_admin;
                    
                    if (oldInsp.Lines != null) {
                        foreach (var oldLine in oldInsp.Lines) {
                            var newLine = insp.Lines.FirstOrDefault(l => l.id == oldLine.id);
                            if (newLine != null)
                            {
                                oldLine.inspected_qty = newLine.inspected_qty;
                                if (newLine.Rejections != null)
                                {
                                    foreach (var r in newLine.Rejections) {
                                        if (r.id <= 0)
                                            oldLine.Rejections.Add(r);
                                        else {
                                            var oldRejection = oldLine.Rejections.FirstOrDefault(or => or.id == r.id);
                                            if (oldRejection != null) {
                                                oldRejection.action = r.action;
                                                oldRejection.ca = r.ca;
                                                oldRejection.permanentaction = r.permanentaction;
                                                oldRejection.reason = r.reason;
                                                oldRejection.rejection = r.rejection;
                                                oldRejection.comments = r.comments;
                                                oldRejection.document = r.document;
                                            }
                                        }

                                    }                                    
                                }

                                //Delete missing
                                var deletedRejections =
                                        oldLine.Rejections.Where(r => newLine.Rejections == null || newLine.Rejections.Count(i => i.id == r.id) == 0).ToList();
                                foreach (var d in deletedRejections) {
                                    m.Entry(d).State = EntityState.Deleted;
                                }
                            }

                            m.Entry(oldLine).Collection("Images").Load();
                            newLine = insp.Lines.FirstOrDefault(l => l.id == oldLine.id);
                            if (newLine != null) {
                                if (newLine.Images != null) {
                                    foreach (var newIm in newLine.Images) {
                                        if (newIm.id <= 0)
                                            oldLine.Images.Add(newIm);
                                        else {
                                            var oldim = oldLine.Images.FirstOrDefault(i => i.id == newIm.id);
                                            if (oldim != null)
                                            {
                                                oldim.insp_image = newIm.insp_image;
                                                oldim.comments = newIm.comments;
                                            }
                                                
                                        }
                                    }
                                }

                                //Delete missing
                                var deletedImages =
                                        oldLine.Images.Where(im => newLine.Images == null || newLine.Images.Count(i => i.id == im.id) == 0).ToList();
                                foreach (var d in deletedImages) {
                                    m.Entry(d).State = EntityState.Deleted;
                                }

                            }
                            
                        }
                        
                    }

                }

                m.SaveChanges();
            }
        }

        public List<Inspection_v2_image_type> GetImageTypes()
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionImageTypes.ToList();
            }
        }

        public List<Inspection_v2> GetByCriteria(DateTime from , DateTime to , int? location_id)
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionsV2.Include("Factory").Include("Client").Include("Controllers").Include("Controllers.Controller").Where(i => ((i.startdate >= from && i.startdate <= to)
                    ||
                    (DbFunctions.AddDays(i.startdate,i.duration.Value) >=
                    from &&
                    DbFunctions.AddDays(i.startdate, i.duration.Value) <= to)
                    || i.Controllers.Any(c=>c.startdate >= from && c.startdate <= to)
                    || i.Controllers.Any(c=>
                    (DbFunctions.AddDays(c.startdate, c.duration) >=
                    from &&
                    DbFunctions.AddDays(c.startdate, c.duration) <= to)
                    ))
                    && (i.Factory.consolidated_port2 == location_id || location_id == null || i.Factory.consolidated_port_mix == 1 )
                    && i.insp_status != InspectionV2Status.Cancelled)
                    .ToList();
            }
        }

        public List<Inspection_v2> GetByCriteria(IList<int> factory_ids, IList<int> client_ids, string custpo, DateTime? from,
            DateTime? to, IList<InspectionV2Status?> statuses = null, int? inspectorId = null)
        {
            using (var m = Model.CreateModel())
            {
                var predicate = PredicateBuilder.New<Inspection_v2>(true);
                predicate = predicate.And(i =>
                    //(i.insp_batch_inspection == 0) &&
                    (i.startdate >= from || from == null) &&
                    (i.startdate <= to || to == null) &&
                    (custpo == null ||
                     i.Lines.Any(
                         l =>
                             l.OrderLine != null && l.OrderLine.Header != null &&
                             l.OrderLine.Header.custpo.Contains(custpo))));
                if (factory_ids != null)
                    predicate = predicate.And(i => factory_ids.Contains(i.factory_id.Value));
                if (client_ids != null)
                    predicate = predicate.And(i => client_ids.Contains(i.client_id.Value));
                if (inspectorId != null)
                    predicate = predicate.And(i => i.Controllers.Any(c => c.controller_id == inspectorId));
                if (statuses != null)
                {
                    predicate = predicate.And(i => statuses.Contains(i.insp_status));
                }
                return m.InspectionsV2.Include("Factory").Include("Client").Include("Controllers").Include("InspectionType").Include("Lines.OrderLine.Header").AsExpandable()
                        .Where(predicate)
                        .ToList();
            }
        }

        public void Delete(int id)
        {
            using (var m = Model.CreateModel())
            {
                m.Database.ExecuteSqlCommand("DELETE FROM Inspection_V2 WHERE id = ?", id);
            }
        }

        public List<Inspection_v2_area> GetAreas()
        {
            using (var m = Model.CreateModel()) {
                return m.InspectionV2Areas.ToList();
            }
        }

        public void UpdateStatus(int id, InspectionV2Status status)
        {
            using (var m = Model.CreateModel())
            {
                var insp = m.InspectionsV2.FirstOrDefault(i => i.id == id);
                if (insp != null)
                {
                    insp.insp_status = status;
                    m.SaveChanges();
                }
            }
        }

        public void BulkUpdateLines(IList<Inspection_v2_line> lines)
        {
            if(lines != null && lines.Count > 0) {
                using (var m = Model.CreateModel()) {
                    foreach (var g in lines.Where(l=>l.insp_id != null).GroupBy(l=>l.insp_id)) {
                        var insp = m.InspectionsV2.Include(ins => ins.Lines).FirstOrDefault(ins => ins.id == g.Key);
                        foreach(var l in g) {
                            if (l.id <= 0)
                                insp.Lines.Add(l);
                            else {
                                var oldLine = insp.Lines.FirstOrDefault(ol => ol.id == l.id);
                                if (oldLine != null) {
                                    oldLine.qty = l.qty;
                                }
                            }
                        }                        
                    }
                    m.SaveChanges();
                    
                }
            }            
        }

        public void UpdateSimple(int id, DateTime? startDate)
        {
            using (var m = Model.CreateModel()) {

                var insp = m.InspectionsV2.FirstOrDefault(ins => ins.id == id);
                insp.startdate = startDate;
                m.Entry(insp).State = EntityState.Modified;
                m.SaveChanges();
            }
        }
                
        public void UpdateCombinedLoadings(List<Inspection_v2_loading> loadings)
        {
            
            foreach (var l in loadings) {
                //m.InspectionV2Loadings.Attach(l);
                //m.Entry(l).State = EntityState.Modified;
                context.UpdateGraph(l, map => map.OwnedCollection(lo=>lo.QtyMixedPallets).AssociatedCollection(lo => lo.Areas));                
            }            
            
            
        }

        public List<Inspection_v2_line> GetLinesForOrders(IList<int> orderIds, int? excludedId = null )
        {
            var result = new List<Inspection_v2_line>();

            var inspections =
                    context.Set<Inspection_v2>().Include("Lines.OrderLine").Include("Containers").Include("Lines.Loadings")
                        .Where(i => i.Lines.Any(l => orderIds.Contains(l.OrderLine.orderid.Value)) && (excludedId == null || i.id != excludedId))
                        .ToList();
                foreach (var i in inspections) {
                    if (i.Lines != null)
                        result.AddRange(i.Lines);
                } 
            
            return result;
        }

        public override void Insert(Inspection_v2 entity)
        {
            if(entity.Controllers != null)
                foreach (var c in entity.Controllers) {
                    c.Controller = null;
                }
            base.Insert(entity);
        }

        public override void Update(Inspection_v2 entity)
        {
            var oldInsp = Get(i => i.id == entity.id,includeProperties: "Lines.SiDetails").FirstOrDefault();
            context.UpdateGraph(entity, map => map.OwnedCollection(i => i.Controllers)
                                            .OwnedCollection(i=>i.Lines, m=>m.OwnedCollection(l=>l.Images)));

            //Separate update of SIDetails because more than one sub-collection  can't be handled by EF/mySQL driver
            foreach(var l in oldInsp.Lines) {
                var newLine = entity.Lines.FirstOrDefault(nl => nl.id == l.id);
                               
                var removed = l.id > 0 ?  l.SiDetails.Where(d => newLine.SiDetails.Count(nd => nd.id == d.id) == 0).ToList() : null;
                foreach (var nd in newLine.SiDetails) {
                    if (nd.id <= 0) {
                        if (l.SiDetails == null)
                            l.SiDetails = new List<Inspection_v2_line_si_details>();
                        l.SiDetails.Add(nd);
                    }                        
                    else {
                        var oldNd = l.SiDetails.FirstOrDefault(d => d.id == nd.id);
                        context.Entry(oldNd).CurrentValues.SetValues(nd);
                    }
                        
                }
                if(removed != null) {
                    foreach (var r in removed)
                        l.SiDetails.Remove(r);
                }
                
            }
            

        }

        public Inspection_v2 GetSiById(int id, bool loadAllSubobjects)
        {
            var insp = Get(i => i.id == id, includeProperties: "Controllers.Controller, Lines.Product.MastProduct,Lines.Images" + (loadAllSubobjects  ? ",Factory,Client,Subject,InspectionType" : "")).FirstOrDefault();
            if(insp != null) {
                //Separate load of SIDetails because more than one sub-collection  can't be handled by EF/mySQL driver
                foreach (var l in insp.Lines) {
                    context.Entry(l).Collection(li => li.SiDetails).Load();
                }
            }
            return insp;
        }

        public List<KpiReportInspectionRow> GetInspectionsForKpi(int qc_id, int? month21)
        {
            return context.Database.SqlQuery<KpiReportInspectionRow>(
                @"SELECT CONCAT(i.factory_code, '-', insp_type , '-' , date_format(i.insp_start,'%y%m%d') , '-', i.customer_code) AS insp_no , 
                    i.insp_unique AS insp_id,
                    i.insp_start AS startdate,
                    i.factory_code,
                    'Final' as type,
                    0 as New,
                    (SELECT COUNT(DISTINCT r.returnsid) 
	                    FROM returns r INNER JOIN order_lines ol ON r.order_id = ol.orderid
	                    INNER JOIN inspection_lines_tested lt ON ol.linenum = lt.order_linenum
                        WHERE r.claim_type = 7 AND lt.insp_id = i.insp_unique) AS ca
                    FROM inspections i INNER JOIN inspection_controller ic ON i.insp_unique = ic.inspection_id
                    WHERE insp_type = 'FI' AND fn_Month21(i.insp_start) = @p1 AND ic.controller_id = @p0
                    UNION
                    SELECT CONCAT(f.factory_code, '-', it.name , '-' , date_format(i.startdate,'%y%m%d') , '-', c.customer_code) AS insp_no , 
                    i.id AS insp_id,
                    i.startdate,
                    f.factory_code,
                    IF(i.type = 1, 'Loading', 'Final') AS type,
                    1 as New,
                    (SELECT COUNT(DISTINCT r.returnsid) 
	                    FROM returns r INNER JOIN order_lines ol ON r.order_id = ol.orderid
	                    INNER JOIN inspection_v2_line il ON ol.linenum = il.orderlines_id
                        WHERE r.claim_type = 7 AND il.insp_id = i.id) AS ca
                    FROM inspection_v2 i INNER JOIN users f ON  i.factory_id = f.user_id 
                    INNER JOIN users c ON i.client_id = c.user_id
                    INNER JOIN inspection_v2_controller ic ON i.id = ic.inspection_id
                    INNER JOIN inspection_v2_type it ON i.type = it.id
                    WHERE i.type = 1 AND fn_Month21(i.startdate) = @p1 AND ic.controller_id = @p0
                    ORDER BY startdate", qc_id, month21).ToList();
        }

    }
}
