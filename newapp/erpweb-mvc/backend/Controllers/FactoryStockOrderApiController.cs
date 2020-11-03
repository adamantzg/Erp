using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using erp.Model.Dal.New;
using erp.DAL.EF.New;
using backend.Models;
using erp.Model;
using backend.ApiServices;

namespace backend.Controllers
{
	[Authorize(Roles = "Administrator")]
	[RoutePrefix("api/factorystockorder")]
    public class FactoryStockOrderApiController : ApiController
    {
		private readonly IUnitOfWork unitOfWork;
		private readonly ICompanyDAL companyDAL;
		private readonly ICurrenciesDAL currenciesDAL;
		private readonly ICustproductsDAL custproductsDAL;
		
		private readonly IProductService productService;
		private readonly IAccountService accountService;

		public FactoryStockOrderApiController(IUnitOfWork unitOfWork, ICompanyDAL companyDAL, ICurrenciesDAL currenciesDAL,
			ICustproductsDAL custproductsDAL, IProductService productService,
			IAccountService accountService)
		{
			this.unitOfWork = unitOfWork;
			this.companyDAL = companyDAL;
			this.currenciesDAL = currenciesDAL;
			this.custproductsDAL = custproductsDAL;			
			this.productService = productService;
			this.accountService = accountService;
		}

		[HttpGet]
		[Route("")]
		public FactoryStockOrderListModel GetListModel()
		{
			return new FactoryStockOrderListModel { 
				Factories = companyDAL.GetFactories().OrderBy(f=>f.factory_code).ToList(), 
				Currencies = currenciesDAL.GetAll() 
			};
		}

		[HttpGet]
		[Route("getEditModel")]
		public FactoryStockOrderEditModel GetEditModel(int id)
		{
			var order = id > 0 ? unitOfWork.FactoryStockOrderRepository.Get(o=>o.id == id, includeProperties: "Lines.MastProduct").FirstOrDefault() : 
				new Factory_stock_order { Lines = new List<Factory_stock_order_lines>() };
			var clients = new[] { new Company { user_id = -1, customer_code = "Brands" } }.
				Union(companyDAL.GetNonBrandClients()
					.Where(c => !string.IsNullOrEmpty(c.customer_code))
					.OrderBy(c => c.customer_code)
					.Select(c => new Company { user_id = c.user_id, customer_code = c.customer_code })).ToList();
			var factories = order.id > 0 ? companyDAL.GetFactories().OrderBy(f=>f.factory_code).ToList() : new List<Company>();
			return new FactoryStockOrderEditModel { 
				Order = order, 
				Clients = clients, 
				Currencies = currenciesDAL.GetAll(), 
				Factories = factories,
				Orders = id > 0 ? 
					unitOfWork.FactoryStockOrderRepository.Get(o=>o.factory_id == order.factory_id && o.id != order.id,o=>o.OrderBy(x=>x.etd)).ToList() : 
					new List<Factory_stock_order>()
				};
		}

		[HttpGet]
		[Route("getOrders")]
		public List<Factory_stock_order> GetOrders(int factoryId)
		{
			var products = unitOfWork.MastProductRepository.Get(p => p.factory_id == factoryId && p.factory_stock >= 0).ToList();
			var orders = new Dictionary<int, Factory_stock_order>();
			//Load orders
			var prodIds = products.Select(p => (int?)p.mast_id).ToList();
			var groupedLinesDict = unitOfWork.FactoryStockOrderRepository.Get(o => o.Lines.Any(l => prodIds.Contains(l.mast_id)), includeProperties: "Lines")
				.SelectMany(o=>o.Lines)
				.GroupBy(l => l.mast_id)
				.ToDictionary(g => g.Key);
			foreach (var p in products)
			{
				if (groupedLinesDict.ContainsKey(p.mast_id))
				{
					var balance = p.factory_stock;
					var lines = groupedLinesDict[p.mast_id].OrderByDescending(l => l.linedate).ToList();
					var i = 0;
					Factory_stock_order order = null;
					while (balance > 0 && i < lines.Count)
					{
						if (lines[i].orderid != null)
						{
							var orderid = lines[i].orderid.Value;
							if (!orders.ContainsKey(orderid))
							{
								order = lines[i].Order;
								order.Balance = 0;
								order.BalanceValue = 0;
								order.Currency = lines[i].currency;
								orders[orderid] = order;
							}
							else
								order = orders[orderid];
							var lineBalance = Math.Min(balance ?? 0, lines[i].qty ?? 0);
							order.Balance += lineBalance;
							order.BalanceValue += lineBalance * lines[i].price;
							balance -= lines[i].qty;
						}
						i++;
					}
				}
			}
			return orders.Values.ToList();
		}

		[HttpPost]
		[Route("")]
		public Factory_stock_order Update(Factory_stock_order o)
		{
			if (o.id > 0)
				unitOfWork.FactoryStockOrderRepository.Update(o);
			else
			{
				o.creator_id = accountService.GetCurrentUser()?.userid;
				o.datecreated = DateTime.Now;
				unitOfWork.FactoryStockOrderRepository.Insert(o);
			}
			unitOfWork.Save();
			return o;
		}

		[HttpDelete]
		[Route("")]
		public void Delete(int id)
		{
			unitOfWork.FactoryStockOrderRepository.DeleteByIds(new[] { id });
		}

		[HttpGet]
		[Route("getProducts")]
		public object GetProductsForFactory(int factoryId, string prefixText)
		{
			var products = custproductsDAL.GetForFactories(new[] { factoryId }, prefixText);

			return products.Select(p => new {
				p.cprod_code1,
				p.cprod_name,
				p.cprod_mast,
				p.MastProduct.factory_ref,
				p.MastProduct.factory_name,
				p.cprod_user,
				p.MastProduct.price_dollar,
				p.MastProduct.price_euro,
				p.MastProduct.price_pound,
				combined_code = productService.GetCombinedCode(p.cprod_code1, p.MastProduct.factory_ref)
			}).ToList();
		}

		[HttpPost]
		[Route("moveLines")]
		public void MoveLines(MoveLinesData data)
		{
			if(data.Order.id <= 0)
			{
				unitOfWork.FactoryStockOrderRepository.Insert(data.Order);
				unitOfWork.Save();
			} 
			
			if(data.Lines != null && data.Lines.Count > 0)
			{
				var linesIds = data.Lines.Select(l => l.id).ToList();
				var sourceId = data.Lines[0].orderid;
				var sourceOrder = unitOfWork.FactoryStockOrderRepository.Get(o => o.id == sourceId, includeProperties: "Lines").FirstOrDefault();
				if(sourceOrder != null)
				{
					foreach(var l in sourceOrder.Lines.Where(x=>linesIds.Contains(x.id)))
					{
						l.orderid = data.Order.id;
					}
					unitOfWork.Save();
				}

			}
			

		}
    }

	public class MoveLinesData
	{
		public List<Factory_stock_order_lines> Lines { get; set; }
		public Factory_stock_order Order { get; set; }
	}
}
