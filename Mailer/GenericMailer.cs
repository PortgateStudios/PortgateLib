using System.Net;
using System.Net.Mail;

namespace PortgateLib.Mailer
{
	public class MailerOptions
	{
		public string SmtpHost;
		public int SmtpPort;
		public bool EnableSsl;
		public bool IsBodyHtml;
		public string Name;
		public string Email;
		public string Password;
	}

	public static class GenericMailer
	{
		public static void Send(MailerOptions mailerOptions, string recipient, string subject, string body)
		{
			var client = new SmtpClient(mailerOptions.SmtpHost, mailerOptions.SmtpPort)
			{
				Credentials = new NetworkCredential(mailerOptions.Email, mailerOptions.Password),
				EnableSsl = mailerOptions.EnableSsl,
			};

			var msg = new MailMessage()
			{
				From = new MailAddress(mailerOptions.Email, mailerOptions.Name),
				Subject = subject,
				Body = body,
				IsBodyHtml = mailerOptions.IsBodyHtml
			};

			msg.To.Add(recipient);

			client.Send(msg);
		}
	}
}