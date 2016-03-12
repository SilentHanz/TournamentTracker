using System;
using System.Net.Mail;

namespace TournamentReport.Services
{
	public class MessengerService : IMessengerService
	{
		public bool Send(string from, string to, string subject, string body, bool isBodyHtml)
		{
			var isSuccess = false;
			
			try
			{
				var msg = new MailMessage(from, to, subject, body);
				msg.IsBodyHtml = isBodyHtml;
				var smtp = new SmtpClient();
				smtp.Send(msg);
				isSuccess = true;
			}
			catch (Exception ex)
			{
				//todo: Log exception
			}
		
			return isSuccess;
		}
	}
}