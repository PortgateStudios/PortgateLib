using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortgateLib.Mailer
{
	public record GMailerOptions
	{
		public string Name { get; init; }
		public string Username { get; init; }
		public string Password { get; init; }
	}

	public static class GMailer
	{
		public static void Send(GMailerOptions gMailerOptions, string recipient, string subject, string body, AttachmentOptions attachmentOptions = null)
		{
			var mailerOptions = GetMailerOptions(gMailerOptions);
			GenericMailer.Send(mailerOptions, recipient, subject, body, attachmentOptions);
		}

		public static void Send(GMailerOptions gMailerOptions, string recipient, string subject, string body, IEnumerable<AttachmentOptions> attachmentOptions = null)
		{
			var mailerOptions = GetMailerOptions(gMailerOptions);
			GenericMailer.Send(mailerOptions, recipient, subject, body, attachmentOptions);
		}

		public static async Task SendAsync(GMailerOptions gMailerOptions, string recipient, string subject, string body, AttachmentOptions attachmentOptions = null)
		{
			var mailerOptions = GetMailerOptions(gMailerOptions);
			await GenericMailer.SendAsync(mailerOptions, recipient, subject, body, attachmentOptions);
		}

		public static async Task SendAsync(GMailerOptions gMailerOptions, string recipient, string subject, string body, IEnumerable<AttachmentOptions> attachmentOptions = null)
		{
			var mailerOptions = GetMailerOptions(gMailerOptions);
			await GenericMailer.SendAsync(mailerOptions, recipient, subject, body, attachmentOptions);
		}

		private static MailerOptions GetMailerOptions(GMailerOptions gMailerOptions)
		{
			return new()
			{
				SmtpHost = "smtp.gmail.com",
				SmtpPort = 587,
				EnableSsl = true,
				IsBodyHtml = true,
				Name = gMailerOptions.Name,
				Email = $"{gMailerOptions.Username}@gmail.com",
				Password = gMailerOptions.Password
			};
		}
	}
}