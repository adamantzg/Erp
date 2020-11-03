using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;
using RefactorThis.GraphDiff;

namespace erp.DAL.EF.New
{
	public class InspectionV2Repository : GenericRepository<Inspection_v2>, IInspectionV2Repository
	{
		public InspectionV2Repository(DbContext context) : base(context)
		{
		}

		public void UpdateLoading(Inspection_v2 insp)
        {
            
            var oldInsp = 
                    Get(i => i.id == insp.id, includeProperties: "Containers.Images, Controllers, MixedPallets,Lines.Loadings").FirstOrDefault();
            if (oldInsp != null)
            {
                context.Entry(oldInsp).CurrentValues.SetValues(insp);
                
                if (insp.Containers != null) {
                    foreach (var c in oldInsp.Containers) {
                        var newcont = insp.Containers.FirstOrDefault(co => co.id == c.id);
                        if (newcont != null) {
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

                            deletedImages.AddRange(oldLine.Images.Where(i => i.insp_image == null));

                            foreach (var d in deletedImages) {
                                context.Entry(d).State = EntityState.Deleted;
                            }

                        }
                    } 
                }
                    
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

		public void UpdateFinal(Inspection_v2 insp)
        {
            
            var oldInsp = dbSet.Include("Controllers")
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
                                context.Entry(d).State = EntityState.Deleted;
                            }
                        }

                        context.Entry(oldLine).Collection("Images").Load();
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
                                context.Entry(d).State = EntityState.Deleted;
                            }

                        }
                        
                    }
                    
                }

            }

            context.SaveChanges();
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
