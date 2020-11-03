using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace backend
{
    
	public interface IMailHelper
	{
		void SendMail(string @from, string to, string subject, string body, string cc = null, string bcc = null,
			Attachment[] attachments = null, string smtpServer = null, string smtpUserName = null, string smtpPassword = null,
			int smtpServerPortNumber = -1, bool removeCurrentUser = false, string currentUserEmail = null);
	};
	
}
