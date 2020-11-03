using System.Collections.Generic;
using erp.Model;

namespace backend.ApiServices
{
	public interface IClaimsService
	{
		List<Lookup> GetResolutions();
		void SendEmails(Returns returns, List<User> recipients, Cust_products product);
		string SubstituteMacros(string recipients, string subscriberEmails, string creatorEmails);
		string GetTypeText(int claim_type);
		string MailOrganiser(string existing_emails, string new_emails);
	}
}