using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Channels;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.WebPages;
using erp.Model;
using System.IO;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using AegisImplicitMail;
using System.Text;

namespace backend
{
    class ErrorMailSending
    {
        public string ErrorMessage { get; set; }
    }
    public class MailHelper : IMailHelper
    {
        private IMimeMailer mailer;
        private IRegistryReader registryReader;

        public MailHelper(IMimeMailer mailer, IRegistryReader registryReader)
        {
            this.mailer = mailer;
            this.registryReader = registryReader;
        }

        public void SendMail(string from, string to, string subject, string body, string cc = null, string bcc = null, Attachment[] attachments = null, string smtpServer = null,
                                string smtpUserName = null, string smtpPassword = null, int smtpServerPortNumber = -1, bool removeCurrentUser = false, string currentUserEmail = null)
        {
            ErrorMailSending errorMailSending = new ErrorMailSending();


            var myMessage = new MimeMailMessage();
            myMessage.BodyEncoding = Encoding.UTF8;
            myMessage.SubjectEncoding = Encoding.ASCII;
            //var myMessage = new MailMessage();
            myMessage.From = new MailAddress(from);

            if (removeCurrentUser)
            {
                to = RemoveCurrentUser(currentUserEmail, to);
                cc = RemoveCurrentUser(currentUserEmail, cc);
                bcc = RemoveCurrentUser(currentUserEmail, bcc);
            }


            var debugMailReceiver = Convert.ToString(registryReader.GetHKLMValue("SOFTWARE\\BB", "DebugMailReceiver"));
            if (string.IsNullOrEmpty(debugMailReceiver))
                debugMailReceiver = Properties.Settings.Default.TestMails;
            if (string.IsNullOrEmpty(debugMailReceiver))
                debugMailReceiver = null;
#if DEBUG
            try
            {
                myMessage.To.Add(registryReader.GetValue("HKEY_CURRENT_USER\\BB", "DebugMailReceiver").ToString());
            }
            catch (Exception ex)
            {
                string m1 = string.Format("Email is not valid");
                string m2 = string.Format("E-mail must be supplied (nn@nn.nn)");

                string message = string.Format("{0} Error in sending mail to: {1}   {2}", ex, to, Properties.Settings.Default.DebugMailReceiver.IsEmpty() ? m2 : m1);
                throw new Exception(message);
            }

#else
            var toAddress = debugMailReceiver ?? to;            
            myMessage.To.Add(toAddress);
#endif

            if (!string.IsNullOrEmpty(cc))
            {
#if !DEBUG
                myMessage.CC.Add(debugMailReceiver ?? cc);
#endif
            }

            if (!string.IsNullOrEmpty(bcc))
            {
#if !DEBUG
                myMessage.Bcc.Add(debugMailReceiver ?? bcc);
#endif
            }

            myMessage.Subject = subject;
            if (attachments != null)
            {
                foreach (var a in attachments)
                    myMessage.Attachments.Add(new MimeAttachment(a.ContentStream, a.Name));
            }

            myMessage.IsBodyHtml = true;
            myMessage.Body = string.Format("<body style=\"font-family: Verdana, arial;font-size:11px\">{0}</body>", body);

#if DEBUG
            myMessage.Body += string.Format("<br> Original recipient: {0} CC: {1}  BCC: {2} ", to, cc, bcc);
#else
            if(!string.IsNullOrEmpty(debugMailReceiver))
            {
                myMessage.Body += string.Format("<br> Original recipient: {0} CC: {1}  BCC: {2} ", to, cc, bcc);
            }
#endif
            var mySmtpClient = mailer;

            if (smtpServer == null)
                smtpServer = Properties.Settings.Default.SMTPServer;
            if (smtpUserName == null)
                smtpUserName = Properties.Settings.Default.SMTPAccount;
            if (smtpPassword == null)
                smtpPassword = Properties.Settings.Default.SMTPPassword;



            if (smtpServer != null)
            {
                mySmtpClient.Host = smtpServer;

                if (smtpServerPortNumber == -1)
                {
                    mySmtpClient.Port = Properties.Settings.Default.SMTPServerPortNumber;
                }

                if (smtpUserName != null)
                {
                    //NetworkCredential myCredential = new NetworkCredential(smtpUserName,smtpPassword);
                    //mySmtpClient.UseDefaultCredentials = false;
                    //mySmtpClient.Credentials = myCredential;
                    mySmtpClient.User = smtpUserName;
                    mySmtpClient.Password = smtpPassword;
                    mySmtpClient.AuthenticationMode = AuthenticationType.Base64;
                }
                //mySmtpClient.ServicePoint.MaxIdleTime = 1;
                if (Properties.Settings.Default.SMTPSslEnable)
                    mySmtpClient.SslType = SslMode.Ssl;
            }



            try
            {
                mySmtpClient.SendCompleted += MySmtpClient_SendCompleted;
                mySmtpClient.SendMailAsync(myMessage);
            }
            catch (Exception sException)
            {
                string m1 = string.Format("Email is not valid");
                string m2 = string.Format("E-mail must be supplied (nn@nn.nn)");


                string message = string.Format("Error in sending mail to: {0}   {1}", to, to.IsEmpty() ? m2 : m1);
                throw new Exception(message, sException);

            }

        }

        private static void MySmtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        private static string RemoveCurrentUser(string currentUserEmail, string mailAddresses)
        {
            var parts = mailAddresses.Split(',');
            if (currentUserEmail != null && mailAddresses != currentUserEmail)
                return string.Join(",", parts.Where(p => p.Trim() != currentUserEmail));
            return string.Join(",", parts);
        }


    }
}