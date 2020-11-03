using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using erp.Model;

namespace erp.DAL.EF
{
    public class InspectionCriteriaRepository
    {
        public static List<Inspv2_template> GetTemplates()
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionCriteriaTemplates.ToList();
            }
        }

        public static Inspv2_template GetTemplateById(int templateId)
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionCriteriaTemplates.Include("Criteria").Include("Criteria.Point").Include("Products").FirstOrDefault(t => t.id == templateId);
            }
        }

        public static List<Inspv2point> GetPoints()
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionCriteriaPoints.ToList();
            }
        }

        public static List<Inspv2_criteriacategory> GetCategories()
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionCriteriaCategories.ToList();
            }
        }

        public static void UpdateTemplate(Inspv2_template template, int[] deletedCriteria, int[] deletedProducts)
        {
            using (var m = Model.CreateModel())
            {
                var t = m.InspectionCriteriaTemplates.Include("Criteria").Include("Products").FirstOrDefault(tp => tp.id == template.id);
                if (t != null)
                {
                    t.name = template.name;
                    if (template.Criteria != null)
                    {
                        foreach (var c in template.Criteria.Where(c => c.IsNew))
                        {
                            t.Criteria.Add(c);
                        }
                        foreach (var crit in template.Criteria.Where(c => !c.IsNew && c.IsModified))
                        {
                            var orig_criteria = t.Criteria.FirstOrDefault(c => c.id == crit.id);
                            if (orig_criteria != null)
                            {
                                orig_criteria.category_id = crit.category_id;
                                orig_criteria.importance = crit.importance;
                                orig_criteria.number = crit.number;
                                orig_criteria.point_id = crit.point_id;
                                orig_criteria.requirements = crit.requirements;
                                orig_criteria.requirements_cn = crit.requirements_cn;
                            }
                        }    
                    }
                    
                    if (deletedCriteria != null)
                    {
                        foreach (var id in deletedCriteria)
                        {
                            var orig_criteria = t.Criteria.FirstOrDefault(c => c.id == id);
                            if (orig_criteria != null)
                            {
                                t.Criteria.Remove(orig_criteria);
                            }
                        }
                    }

                    
                    if (template.Products != null)
                    {
                        foreach (var p in template.Products.Where(p => p.IsNew))
                        {
                            m.CustProducts.Attach(p);
                            t.Products.Add(p);
                        }    
                    }

                    if (deletedProducts != null)
                    {
                        foreach (var id in deletedProducts)
                        {
                            var orig_prod = t.Products.FirstOrDefault(p => p.cprod_id == id);
                            if (orig_prod != null)
                            {
                                t.Products.Remove(orig_prod);
                            }
                        }
                    }
                    m.SaveChanges();

                }
            }
        }


        public static void CreateTemplate(Inspv2_template template)
        {
            using (var m = Model.CreateModel())
            {
                AttachProducts(template.Products, m);
                m.InspectionCriteriaTemplates.Add(template);
                m.SaveChanges();
            }
        }

        public static List<Inspv2_criteria> GetTemplateCriteria(int template_id)
        {
            using (var m = Model.CreateModel())
            {
                var t = m.InspectionCriteriaTemplates.Include("Criteria").Include("Criteria.Point").FirstOrDefault(te => te.id == template_id);
                if (t != null)
                    return t.Criteria.ToList();
                return null;
            }
        }

        public static List<Inspv2_customcriteria> GetCustomCriteria(IList<int> productIds)
        {
            using (var m = Model.CreateModel())
            {
                return m.InspectionCustomcriteria.Where(c => productIds.Contains(c.cprod_id)).ToList();
            }
        }

        public static void AssignTemplate(IList<int> ids, int template_id)
        {
            using (var m = Model.CreateModel())
            {
                var template = new Inspv2_template {id = template_id};
                m.InspectionCriteriaTemplates.Attach(template);
                var products = m.CustProducts.Include("Inspv2Templates").Where(p => ids.Contains(p.cprod_id)).ToList();
                foreach (var p in products)
                {
                    if (p.Inspv2Templates.Count(t => t.id == template_id) == 0)
                    {
                        p.Inspv2Templates.Add(template);
                    }
                }
                m.SaveChanges();
            }
            
        }

        private static void AttachProducts(IEnumerable<Cust_products> products , Model m)
        {
            foreach (var p in products)
            {
                m.CustProducts.Attach(p);
            }
        }

        public static void SaveCustomCriteria(List<Inspv2_customcriteria> customCriteria, List<int> deletedCriteria, List<int> productIds)
        {
            using (var m = Model.CreateModel())
            {
                foreach (var cprod_id in productIds)
                {
                    m.CustProducts.Attach(new Cust_products {cprod_id = cprod_id});
                    if (customCriteria != null)
                    {
                        foreach (var c in customCriteria)
                        {
                            if (c.id < 0)
                            {
                                AddNewCustomCriteria(m, c, cprod_id);
                            }
                            else
                            {
                                var crit =
                                    m.InspectionCustomcriteria.FirstOrDefault(
                                        cr => cr.cprod_id == cprod_id && cr.criteria_id == c.criteria_id);
                                if (crit == null || c.IsDeleted)
                                {
                                    AddNewCustomCriteria(m, c, cprod_id);
                                }
                                else
                                {

                                    crit.category_id = c.category_id;
                                    crit.importance = c.importance;
                                    crit.point_id = c.point_id;
                                    crit.requirements = c.requirements;
                                    crit.requirements_cn = c.requirements_cn;
                                    crit.number = c.number;

                                }
                            }
                        }
                    }


                }

                if (deletedCriteria != null)
                {
                    var toDelete = m.InspectionCustomcriteria.Where(c => deletedCriteria.Contains(c.id));
                    m.InspectionCustomcriteria.RemoveRange(toDelete);
                }
                m.SaveChanges();
            }
        }

        private static void AddNewCustomCriteria(Model m, Inspv2_customcriteria c, int cprod_id)
        {
            m.InspectionCustomcriteria.Add(new Inspv2_customcriteria
            {
                category_id = c.category_id,
                cprod_id = cprod_id,
                criteria_id = c.criteria_id,
                importance = c.importance,
                point_id = c.point_id,
                requirements = c.requirements,
                requirements_cn = c.requirements_cn,
                number = c.number,
                IsDeleted = c.IsDeleted
            });
        }
    }
}
