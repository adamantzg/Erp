using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using company.Common;
using erp.DAL.EF.New;
using erp.Model;
using erp.Model.Dal.New;

namespace backend.ApiServices
{
	public class ReturnsService : IReturnsService
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ICountriesDAL countriesDAL;

		public ReturnsService(IUnitOfWork unitOfWork, ICountriesDAL countriesDAL)
		{
			this.unitOfWork = unitOfWork;
			this.countriesDAL = countriesDAL;
		}

		public List<ReturnAggregateDataPrice> GetTotalsPerBrand(DateTime? dateFrom = null,
            DateTime? dateTo = null, IList<int> incClients = null,
            IList<int> exClients = null, CountryFilter countryFilter = CountryFilter.UKOnly)
        {
            var ukCountries = new[] { "GB", "IE", "UK" };
            
            
            var asianCountries =   countriesDAL.GetForContinent("AS").Select(c=>c.ISO2);
            int?[] decisions = {1, 999, 500};
            
			var useIncClients = incClients != null;
			var useExClients = incClients == null && exClients != null;
			
			            
            return
                unitOfWork.ReturnRepository.GetQuery(
					r => r.status1 == 1 && (dateFrom == null || r.request_date >= dateFrom) && r.Client.distributor > 0 && r.Client.hide_1 == 0 &&
                            (dateTo == null || r.request_date <= dateTo) && decisions.Contains(r.decision_final) &&
                            r.claim_type != null && r.claim_type != 5
					 && 
					(		(countryFilter == CountryFilter.UKOnly && ukCountries.Contains(r.Client.user_country))
                         || (countryFilter == CountryFilter.NonUK && !ukCountries.Contains(r.Client.user_country))
                         || (countryFilter == CountryFilter.NonUKExcludingAsia && !ukCountries.Contains(r.Client.user_country) 
								&& !asianCountries.Contains(r.Client.user_country)
							)
					) && 
					(!useIncClients || incClients.Contains(r.client_id.Value)) &&
					(!useExClients || !exClients.Contains(r.client_id.Value)),
					includeProperties: "Client,Product")
					.GroupBy(r => new {r.Product.brand_userid,r.claim_type})
                    .OrderBy(g => g.Key.brand_userid).ToList()
					.Select(g => new ReturnAggregateDataPrice {id = g.Key.brand_userid,claim_type = g.Key.claim_type ?? 0,
                        TotalAccepted = g.Where(r=>r.decision_final == 1).Sum(r=>ReturnsSelector(r)) ?? 0,
                        TotalAcceptedEBUK = g.Where(r => r.ebuk == 1 && r.decision_final == 1).Sum(r => ReturnsSelector(r)) ?? 0,
                        TotalRejected =  g.Where(r=>r.decision_final == 999).Sum(r=>ReturnsSelector(r)) ?? 0,
                        TotalReplacementParts = g.Where(r=>r.decision_final == 500).Sum(r=>ReturnsSelector(r)) ?? 0
                    }).ToList(); 
					

        }

        public List<ReturnAggregateDataByMonth> GetReturnsByMonth(DateTime? from = null, DateTime? to = null, bool acceptedOnly = true, IList<int> cprod_ids = null )
        {
            var useProds = cprod_ids != null;

            return unitOfWork.ReturnRepository.GetQuery(
				r => (r.request_date >= from || from == null) && (r.request_date <= to || to == null) &&
                        (!acceptedOnly || r.decision_final == 1) && r.request_date != null &&
						(!useProds || cprod_ids.Contains(r.cprod_id.Value))
				)
				.GroupBy(r => new {year = r.request_date.Value.Year, month = r.request_date.Value.Month}).ToList()
            .Select(
                g =>
                    new ReturnAggregateDataByMonth
                    {
                        created_month = Month21.FromDate(new DateTime(g.Key.year, g.Key.month, 1)).Value,
                        CountReturns = g.Count(),
                        ReturnValue = g.Sum(r=>r.TotalValue)
                    }).ToList();            
        }

        public List<ReturnAggregateDataProduct> GetReturnsByReason(DateTime? from = null, DateTime? to = null, IList<int> cprod_ids = null,
            bool acceptedOnly = true)
        {
			var useProds = cprod_ids != null;

            return unitOfWork.ReturnRepository.GetQuery(
				r => (r.request_date >= from || from == null) && (r.request_date <= to || to == null) &&
                        (!acceptedOnly || r.decision_final == 1) && r.request_date != null &&
						(!useProds || cprod_ids.Contains(r.cprod_id.Value))
				)
				.GroupBy(r => r.reason).ToList()
				.Select(
					g =>
						new ReturnAggregateDataProduct
						{
							Reason = g.Key,
							TotalAccepted = g.Sum(r=>r.return_qty) ?? 0
						}).ToList();        

        }

        public List<Returns> GetForPeriodAndBrand(int brand_id, DateTime? from = null, DateTime? to = null)
        {
			return unitOfWork.ReturnRepository.Get(
				r => (r.request_date >= from || from == null) && (r.request_date <= to || to == null) &&
                             (r.decision_final == 1) && r.Product.brand_id == brand_id,
				includeProperties: "Client, Product"
				).ToList();
        }

        public List<Returns> GetForPeriodAndProduct(int cprod_id, DateTime? from = null, DateTime? to = null)
        {
			return unitOfWork.ReturnRepository.Get(
				r => (r.request_date >= from || from == null) && (r.request_date <= to || to == null) &&
                             (r.decision_final == 1) && r.cprod_id == cprod_id,
				includeProperties: "Client, Product"
				).ToList();
        }

        public List<Returns> GetFeedbacks(IList<int> cprod_ids, int type)
        {
			return unitOfWork.ReturnRepository.Get(
				r => r.cprod_id != null && cprod_ids.Contains(r.cprod_id.Value) && r.claim_type == type,
				includeProperties: "Comments.Creator"
				).ToList();            
        }

        public List<Returns_comments> GetComments(int return_id)
        {
            var ret = unitOfWork.ReturnRepository.Get(r => r.returnsid == return_id, includeProperties: "Comments").FirstOrDefault();
            if (ret != null)
                return ret.Comments;
            return null;
            
        }

		public List<Returns> GetForFactories(IList<int> factoryIds, bool commentsOnly = false, DateTime? fromDate= null)
        {
            return unitOfWork.ReturnRepository.
				Get(
					r =>
						r.Product != null && r.Product.MastProduct != null &&
						r.Product.MastProduct.factory_id != null &&
						factoryIds.Contains(r.Product.MastProduct.factory_id.Value)

						&& (!commentsOnly || !string.IsNullOrEmpty(r.factory_comments))
						&& (fromDate == null || r.request_date >= fromDate),
					includeProperties: "Product.MastProduct"
						).ToList();
            
        }

        private double? ReturnsSelector(Returns r)
        {
            return r.claim_type == 2 ? r.claim_value : r.return_qty*r.credit_value;
        }

	}
}