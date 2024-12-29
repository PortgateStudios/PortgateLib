using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

// https://stackoverflow.com/questions/64749385/predefined-type-system-runtime-compilerservices-isexternalinit-is-not-defined
namespace System.Runtime.CompilerServices
{
	internal static class IsExternalInit { }
}

namespace PortgateLib.Mailer
{
	public record MailerOptions
	{
		public string SmtpHost { get; init; }
		public int SmtpPort { get; init; }
		public bool EnableSsl { get; init; }
		public bool IsBodyHtml { get; init; }
		public string Name { get; init; }
		public string Email { get; init; }
		public string Password { get; init; }
	}

	public record AttachmentOptions
	{
		public string FileName { get; init; }
		public byte[] ContentBytes { get; init; } // Raw file content in bytes
		public string MimeType { get; init; } // e.g., "image/jpeg" or "application/zip" or "application/json"
	}

	public static class GenericMailer
	{
		public static void Send(MailerOptions mailerOptions, string recipient, string subject, string body, AttachmentOptions attachmentOptions = null)
		{
			using var client = CreateSmtpClient(mailerOptions);
			var msg = CreateMailMessage(mailerOptions, recipient, subject, body, attachmentOptions);
			client.Send(msg);
		}

		public static void Send(MailerOptions mailerOptions, string recipient, string subject, string body, IEnumerable<AttachmentOptions> attachmentOptions = null)
		{
			using var client = CreateSmtpClient(mailerOptions);
			var msg = CreateMailMessage(mailerOptions, recipient, subject, body, attachmentOptions);
			client.Send(msg);
		}

		public static async Task SendAsync(MailerOptions mailerOptions, string recipient, string subject, string body, AttachmentOptions attachmentOptions = null)
		{
			using var client = CreateSmtpClient(mailerOptions);
			var msg = CreateMailMessage(mailerOptions, recipient, subject, body, attachmentOptions);
			await client.SendMailAsync(msg);
		}

		public static async Task SendAsync(MailerOptions mailerOptions, string recipient, string subject, string body, IEnumerable<AttachmentOptions> attachmentOptions = null)
		{
			using var client = CreateSmtpClient(mailerOptions);
			var msg = CreateMailMessage(mailerOptions, recipient, subject, body, attachmentOptions);
			await client.SendMailAsync(msg);
		}

		private static SmtpClient CreateSmtpClient(MailerOptions mailerOptions)
		{
			var client = new SmtpClient(mailerOptions.SmtpHost, mailerOptions.SmtpPort)
			{
				Credentials = new NetworkCredential(mailerOptions.Email, mailerOptions.Password),
				EnableSsl = mailerOptions.EnableSsl
			};
			return client;
		}

		private static MailMessage CreateMailMessage(MailerOptions mailerOptions, string recipient, string subject, string body, AttachmentOptions attachmentOptions)
		{
			return CreateMailMessage(mailerOptions, recipient, subject, body, new List<AttachmentOptions> { attachmentOptions });
		}

		private static MailMessage CreateMailMessage(MailerOptions mailerOptions, string recipient, string subject, string body, IEnumerable<AttachmentOptions> attachmentOptionsCollection)
		{
			var msg = new MailMessage()
			{
				From = new MailAddress(mailerOptions.Email, mailerOptions.Name),
				Subject = subject,
				Body = body,
				IsBodyHtml = mailerOptions.IsBodyHtml
			};

			msg.To.Add(recipient);

			if (attachmentOptionsCollection != null)
			{
				foreach (var attachmentOptions in attachmentOptionsCollection)
				{
					if (attachmentOptions != null && attachmentOptions.ContentBytes != null)
					{
						var stream = new MemoryStream(attachmentOptions.ContentBytes);
						var attachment = new Attachment(stream, attachmentOptions.FileName, attachmentOptions.MimeType ?? "application/octet-stream");
						msg.Attachments.Add(attachment);
					}
				}
			}

			return msg;
		}
	}
}