using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;


namespace asaq2.WebSites.Common
{
    class ErrorMailSending
    {
        public string ErrorMessage { get; set; }
    }

    public class MailHelper
    {
        public static void SendMail(string from, string to, string subject, string body, string cc = null, string bcc = null, Attachment[] attachments = null, string smtpServer = null,
                                string smtpUserName = null, string smtpPassword = null)
        {
            ErrorMailSending errorMailSending = new ErrorMailSending();

            MailMessage msgMail = new MailMessage();

            MailMessage myMessage = new MailMessage();
            myMessage.From = new MailAddress(from);
#if DEBUG

            try
            {
                myMessage.To.Add(Properties.Settings.Default.DebugMailReceiver);
            }
            catch (Exception ex)
            {
                string m1 = string.Format("Email is not valid");
                string m2 = string.Format("E-mail must be supplied (nn@nn.nn)");

                string message = string.Format("{0} Error in sending mail to: {1}   {2}", ex, to, string.IsNullOrEmpty(Properties.Settings.Default.DebugMailReceiver) ? m2 : m1);
                throw new Exception(message);
            }

#else
            myMessage.To.Add(to);
#endif

            if (!string.IsNullOrEmpty(cc))
            {
#if !DEBUG
                myMessage.CC.Add(cc);
#endif
            }

            if (!string.IsNullOrEmpty(bcc))
            {
#if !DEBUG
                myMessage.Bcc.Add(bcc);
#endif
            }

            myMessage.Subject = subject;
            if (attachments != null)
            {
                foreach (var a in attachments)
                    myMessage.Attachments.Add(a);
            }

            myMessage.IsBodyHtml = true;

            myMessage.Body = string.Format("<body style=\"font-family: Verdana, arial;font-size:11px\">{0}</body>", body);

#if DEBUG
            myMessage.Body += string.Format("<br> Original recipient: {0} CC: {1}  BCC: {2} ", to, cc, bcc);
#endif

            SmtpClient mySmtpClient = new SmtpClient();

            if (smtpServer == null)
                smtpServer = Properties.Settings.Default.SMTPServer;
            if (smtpUserName == null)
                smtpUserName = Properties.Settings.Default.SMTPAccount;
            if (smtpPassword == null)
                smtpPassword = Properties.Settings.Default.SMTPPassword;

            if (smtpServer != null)
            {
                mySmtpClient.Host = smtpServer;
                if (smtpUserName != null)
                {
                    NetworkCredential myCredential = new NetworkCredential(smtpUserName, smtpPassword);
                    mySmtpClient.UseDefaultCredentials = false;
                    mySmtpClient.Credentials = myCredential;
                }
                mySmtpClient.ServicePoint.MaxIdleTime = 1;
            }

            //EventLog.WriteEntry("asaqback",string.Format("Sent message to {0}, cc {1}, bcc {2}, subject {3}",myMessage.To.ToString(),myMessage.CC.ToString(),myMessage.Bcc.ToString(),myMessage.Subject),EventLogEntryType.Information);


            try
            {
                mySmtpClient.Send(myMessage);
            }
            catch (Exception sException)
            {
                string m1 = string.Format("Email is not valid");
                string m2 = string.Format("E-mail must be supplied (nn@nn.nn)");


                string message = string.Format("Error in sending mail to: {0}   {1}", to, string.IsNullOrEmpty(to) ? m2 : m1);
                throw new Exception(message, sException);

            }


            myMessage.Dispose();


        }


    }

}