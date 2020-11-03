using company.Common;
using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace backend.Controllers
{
    [Authorize]
    [RoutePrefix("api/budget")]
    public class BudgetActualApiController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IBrandsDAL brandsDAL;

        public BudgetActualApiController(IUnitOfWork unitOfWork, IBrandsDAL brandsDAL)
        {
            this.unitOfWork = unitOfWork;
            this.brandsDAL = brandsDAL;
        }

        [Route("")]
        [HttpGet]
        public object GetAll()
        {
            return unitOfWork.BudgetActualDataRepository.Get()
                    .GroupBy(b => b.month21)
                    .OrderBy(g=>g.Key)
                    .Select(g => GetTotals(g));
        }

        [Route("distributors")]
        [HttpGet]
        public object getDistributors()
        {
            return unitOfWork.CompanyRepository.Get(c => c.distributor > 0).Select(c => new
            {
                c.user_id,
                c.user_name,
                c.customer_code,
                c.showForBudgetActual
            });
        }

        [Route("getModel")]
        [HttpGet]
        public object getModel(int? month21 = null, DateTime? date = null )
        {
            if (month21 == null)
            {
                if (date == null)
                {
                    month21 = (new Month21(unitOfWork.BudgetActualDataRepository.GetQuery().Max(d => d.month21)) + 1).Value;
                }                    
                else 
                    month21 = new Month21(date.Value).Value;
            }
            var data = unitOfWork.BudgetActualDataRepository.Get(d => d.month21 == month21).ToList();
            return new
            {
                data = data.Select(d=> new {
                    d.brand_id,
                    d.distributor_id,
                    d.id,
                    d.month21,
                    d.record_type,
                    d.ukflag,
                    d.value
                }),
                distributorActualData = data.Where(d=>d.distributor_id != null && d.record_type == "A").GroupBy(d=>d.distributor_id).ToDictionary(g=>g.Key, g=>g.Sum(d=>d.value)),
                distributorBudgetData = data.Where(d => d.distributor_id != null && d.record_type == "B").GroupBy(d => d.distributor_id).ToDictionary(g => g.Key, g => g.Sum(d => d.value)),
                UkbrandActualData = data.Where(d=>(d.brand_id != null || (d.brand_id == null && d.distributor_id == null)) && d.ukflag == 1 && d.record_type == "A").GroupBy(d=>d.brand_id ?? 0).ToDictionary(g=>g.Key, g=>g.Sum(d=>d.value)),
                UkbrandBudgetData = data.Where(d => (d.brand_id != null || (d.brand_id == null && d.distributor_id == null)) && d.ukflag == 1 && d.record_type == "B").GroupBy(d => d.brand_id ?? 0).ToDictionary(g => g.Key, g => g.Sum(d => d.value)),
                NonUkbrandActualData = data.Where(d => (d.brand_id != null || (d.brand_id == null && d.distributor_id == null)) && d.ukflag != 1 && d.record_type == "A").GroupBy(d => d.brand_id ?? 0).ToDictionary(g => g.Key, g => g.Sum(d => d.value)),
                NonUkbrandBudgetData = data.Where(d => (d.brand_id != null || (d.brand_id == null && d.distributor_id == null)) && d.ukflag != 1 && d.record_type == "B").GroupBy(d => d.brand_id ?? 0).ToDictionary(g => g.Key, g => g.Sum(d => d.value)),
                distributors = unitOfWork.CompanyRepository.Get(d=>d.showForBudgetActual == true),
                brands = brandsDAL.GetAll().Union(new[] { new Brand { brandname = "Unbranded", brand_id = 0} }).OrderBy(b=>b.brandname)
            };
        }

        [Route("create")]
        [HttpPost]
        public object Create(List<BudgetActualData> data)
        {
            
            
            unitOfWork.Save();
            return data.GroupBy(d => d.month21).Select(g => GetTotals(g));
        }

        [Route("update")]
        [HttpPost]
        public object Update(List<BudgetActualData> data)
        {
            foreach (var d in data)
            {
                if (d.id <= 0)
                    unitOfWork.BudgetActualDataRepository.Insert(d);
                else
                    unitOfWork.BudgetActualDataRepository.Update(d);
            }
            unitOfWork.Save();
            return data.GroupBy(d => d.month21).Select(g => GetTotals(g));
        }

        [Route("updatedist")]
        [HttpPost]
        public object UpdateDist(int id, bool show)
        {
            var dist = unitOfWork.CompanyRepository.GetByID(id);
            if(dist != null)
            {
                dist.showForBudgetActual = show;
                unitOfWork.Save();
            }
            return true;
        }

        private object GetTotals(IGrouping<int, BudgetActualData> g)
        {
            return new
            {
                month21 = g.Key,
                month21Formatted = new Month21(g.Key).Date.ToString("MM/yyyy"),
                actualUKTotal = g.Where(b => b.record_type == "A" && b.ukflag == 1 && (b.brand_id != null || b.distributor_id == null)).Sum(b => b.value),
                actualNonUKTotal = g.Where(b => b.record_type == "A" && b.ukflag != 1 && (b.brand_id != null || b.distributor_id == null)).Sum(b => b.value),
                budgetUKTotal = g.Where(b => b.record_type == "B" && b.ukflag == 1 && (b.brand_id != null || b.distributor_id == null)).Sum(b => b.value),
                budgetNonUKTotal = g.Where(b => b.record_type == "B" && b.ukflag != 1 && (b.brand_id != null || b.distributor_id == null)).Sum(b => b.value)
            };
        }
        
    }
}