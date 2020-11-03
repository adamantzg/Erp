using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using erp.Model;

namespace backend
{
    public class QuotationUtils
    {
        public static QuotationLineRow ComputeQuotationTableRow(Cust_products product, double? container_price, double? margin, double? exchange_rate, double? commission_percent, List<Currencies> currencies,List<Tariffs> tariffs,double vat)
        {
            QuotationLineRow result = new QuotationLineRow();
            if(product.cprod_retail != null)
                result.retail_price = product.cprod_retail.Value * (1+vat );
            if(product.MastProduct.units_per_40nopallet_gp > 0)
                result.qty_per_Container = product.MastProduct.units_per_40nopallet_gp.Value;
            if(product.MastProduct.price_dollar != null)
                result.priceUSD = product.MastProduct.price_dollar.Value;
            if(product.MastProduct.price_pound != null)
                result.priceGBP = product.MastProduct.price_pound.Value;
            var gbpusdRatio = GetCurrencyGBPRatio(currencies, "USD");
            if(result.priceGBP == 0)
            {
                result.priceGBP = result.priceUSD / gbpusdRatio;
            }
            
            if(result.qty_per_Container > 0 && container_price != null)
                result.costPerUnit = container_price.Value/gbpusdRatio/result.qty_per_Container;
            var eu_tax = tariffs.Where(t=>t.tariff_id == product.MastProduct.tariff_code).ToList();
            if(eu_tax.Count > 0)
            {
                if(eu_tax[0].tariff_rate != null)
                    result.tariff_rate = eu_tax[0].tariff_rate.Value;
                result.tariff_code = eu_tax[0].tariff_code;
            }

            if (margin != null)
                result.cifGBP = result.retail_price * margin.Value / 100;
            
            if(exchange_rate != null)
                result.cifLocalCurr = result.cifGBP * exchange_rate.Value;
            if(commission_percent != null)
                result.commission = result.cifGBP * commission_percent.Value/100;

            return result;
            
        }

        public static double GetCurrencyGBPRatio(List<Currencies> currencies, string curr_symbol)
        {
            var curr =  currencies.FirstOrDefault(c=>c.curr_symbol == curr_symbol);
            if(curr != null && curr.curr_exch2 != null)
                return curr.curr_exch2.Value;
            else
                return 0;
        }



        //public static double GetFloatValue(svalue)
        //{
        //    var retval = parseFloat(svalue);
        //    if (isNaN(retval))
        //        retval = 0;
        //    return retval;
        //}

        public static double Round(double value, int num)
        {
            //if (value != null)
                return Math.Round(value, num);
            //else
            //    return null;
        }
    }

    public class QuotationLineRow
    {
        public double retail_price {get;set;}
        public int qty_per_Container {get;set;} 
        public double priceUSD {get;set;}
        public double priceGBP {get;set;}
        public double costPerUnit {get;set;}
        public double tariff_rate {get;set;}
        public string tariff_code {get;set;}
        public double costNoCommission
        {
            get
            {
                return priceGBP*(1+tariff_rate) + costPerUnit;
            }
        }
        public double cifGBP
        {
            get;
            set;
        }
        public double cifLocalCurr {get;set;}
        public double commission {get;set;}
        public double grossmargin
        {
            get
            {
                if (cifGBP > 0)
                    return (cifGBP - costNoCommission - commission) / cifGBP;
                else
                    return 0;
            }
        }
    }
}