namespace erp.Model
{
	public interface ISalesForecast
	{
		int? cprod_id { get; set; }
		int? month21 { get; set; }
		int? sales_qty { get; set; }
		int sales_unique { get; set; }
	}
}