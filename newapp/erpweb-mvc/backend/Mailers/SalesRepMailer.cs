using Mvc.Mailer;
using backend.Models;

namespace backend.Mailers
{ 
    public class SalesRepMailer : MailerBase, ISalesRepMailer 	
	{
		public SalesRepMailer()
		{
			MasterName="_Layout";
		}

        public virtual MvcMailMessage Analysis(SalesRepParameterObjectEx o)
		{
			//ViewBag.Data = someObject;
            ViewData.Model = o;
			return Populate(x =>
			{
				x.Subject = o.Base.EmailModel.Subject;
				x.ViewName = "Analysis";
				x.To.Add(o.Base.EmailModel.To);
                if(o.Base.EmailModel.Cc != null)
                    x.CC.Add(o.Base.EmailModel.Cc);
                if(o.Base.EmailModel.Bcc != null)
                    x.Bcc.Add(o.Base.EmailModel.Bcc);
			});
		}
 	}
}