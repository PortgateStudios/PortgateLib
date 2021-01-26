namespace PortgateLib.Mailer
{
	public class GMailerOptions
	{
		public string Name;
		public string Username;
		public string Password;
	}

	public static class GMailer
	{
		public static void Send(GMailerOptions gMailerOptions, string recipient, string subject, string body)
		{
			var mailerOptions = new MailerOptions()
			{
				SmtpHost = "smtp.gmail.com",
				SmtpPort = 587,
				EnableSsl = true,
				IsBodyHtml = true,
				Name = gMailerOptions.Name,
				Email = $"{gMailerOptions.Username}@gmail.com",
				Password = gMailerOptions.Password
			};

			GenericMailer.Send(mailerOptions, recipient, subject, body);
		}
	}
}