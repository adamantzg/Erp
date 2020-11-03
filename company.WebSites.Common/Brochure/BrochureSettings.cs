using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asaq2.Model;

namespace asaq2.WebSites.Common
{
    public class BrochureSettings
    {
        public string MailToDistributorTo { get; set; }
        public string MailToDistributorCc { get; set; }
        public string MailToDistributorBcc { get; set; }
        public string MailToDistributorSubject { get; set; }
        public string MailToDistributorBody { get; set; }
        public Web_site WebSite { get; set; }
        public string MailToClientTo { get; set; }
        public string MailToClientCc { get; set; }
        public string MailToClientBcc { get; set; }
        public string MailToClientSubject { get; set; }
        public string MailToClientBody { get; set; }

        public string SmtpServer { get; set; }
        public string SmtpAccount { get; set; }
        public string SmtpPassword { get; set; }
        public string FromAccount { get; set; }
        public string SiteUrl { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string OptInMail { get; set; }
        public string[] ProcessedCountries { get; set; }

    }
}
