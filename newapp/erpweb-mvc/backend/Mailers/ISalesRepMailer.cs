using Mvc.Mailer;
using backend.Models;

namespace backend.Mailers
{ 
    public interface ISalesRepMailer
    {
			MvcMailMessage Analysis(SalesRepParameterObjectEx o);
	}
}