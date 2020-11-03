using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using asaq2.Model;
using asaq2.Model.DAL;
using asaq2.Common;
using asaq2.WebSites.Common.Properties;
using Utilities = asaq2.Common.Utilities;

namespace asaq2.WebSites.Common
{
    public class SimpleBrochureRequestHandler : IBrochureRequest
    {
        public void ProcessRequest(BrochureRequest br, BrochureSettings settings)
        {
            
            // now we have the closest store we must create the letter attachments to send AND save them somewhere temporarily
            // we will save the filename as brochure request & the catalogue request id
            Dealer dealer = null;
            if (settings.ProcessedCountries.Contains(br.user_country, StringComparer.OrdinalIgnoreCase))
            {
                //Find nearest dealer
                var dealers = DealerDAL.GetNearestDealers(settings.WebSite.code, br.latitude ?? 0, br.longitude ?? 0,null).Where(d=>d.hide_1 == 1).ToList();
                if (dealers != null && dealers.Count > 0)
                {
                    int index = 0;
                    
                    while (index < dealers.Count && !RegexUtilities.IsValidEmail(dealers[index].user_email))
                    {
                        index++;
                    }
                    if (index < dealers.Count)
                    {
                        dealer = dealers[index];
                        br.dealer_id = dealer.user_id;
                    }
                }

            }

            //Save brochure request
            BrochureRequestDAL.Create(br);

            //Mail to distributor (The person who dispatches brochure)
            string body = settings.MailToDistributorBody;
            var siteName = settings.WebSite != null ? settings.WebSite.name : "";
            var address = Utilities.ArrayToString(new[] {string.Format("{0} {1}",br.user_firstname,br.user_surname), br.user_address1, br.user_address2, br.user_address3, br.user_address4, br.user_address5, br.postcode, br.user_country, br.user_tel, br.user_email },
                                                       "<br/>");
            if (string.IsNullOrEmpty(body))
            {
                var hasCoordinates = ((br.longitude != null && br.longitude != 0) && (br.latitude != null && br.latitude !=0));
                
                    body = string.Format(Resources.MailToDistributorBody, siteName, address, dealer != null && br.contact_optout != 1 && hasCoordinates?
                        "<br/>Our nearest dealer to this customer is:<br/> " + Utilities.ArrayToString(new[] { dealer.user_name, dealer.user_address1, dealer.user_address2, dealer.user_address3, dealer.user_address4, dealer.postcode, dealer.user_country, dealer.user_tel, dealer.user_email }, "<br/>") : "");
               
                if (br.contact_optout == 1)
                    body +=
                        "<br/>The customer ticked the box indicated that they prefer NOT to be contacted by bathroom brands.";
            }
            var subject = settings.MailToDistributorSubject;
            if (string.IsNullOrEmpty(subject))
            {
                subject = string.Format(Resources.MailToDistributorSubject, siteName, br.user_id);
            }
            var cc = (br.user_country == "UK" || br.user_country == "GB" || br.user_country == "IE" ? settings.OptInMail : "") + (settings.MailToClientCc != null ? (br.user_country == "UK" || br.user_country == "GB" || br.user_country == "IE") ? "," + settings.MailToClientCc : settings.MailToClientCc : "");
            var to = settings.MailToDistributorTo; //"Kayleigh.walsh@crosswater.co.uk";
            MailHelper.SendMail(settings.FromAccount, to, subject, body, cc, settings.MailToDistributorBcc, null,
                settings.SmtpServer,settings.SmtpAccount,settings.SmtpPassword);

            //Send to client
            if (RegexUtilities.IsValidEmail(br.user_email))
            {
                body = settings.MailToClientBody;
                if (string.IsNullOrEmpty(body))
                {
                    body = string.Format(Resources.MailToClientBody, siteName, address);
                }
                subject = settings.MailToClientSubject;
                if (string.IsNullOrEmpty(subject))
                {
                    subject = string.Format(Resources.MailToClientSubject, siteName, br.user_id);
                }
                MailHelper.SendMail(settings.FromAccount, br.user_email, subject, body,
                                    settings.MailToClientCc, settings.MailToClientBcc, null,
                                    settings.SmtpServer, settings.SmtpAccount, settings.SmtpPassword);
            }
        }
    }
}
