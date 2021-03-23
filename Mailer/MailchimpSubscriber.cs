using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PortgateLib.Mailer
{
	public class MailchimpSubscriber
	{
		public enum EmailType
		{
			HTML,
			TEXT
		}

		public enum ValidationError
		{
			EmailIllFormatted
		}

		public enum SubscribeResult
		{
			Pending,
			ResetPending,
			AlreadySubscribed,
			WasUnsubscribedNowPending,
			WasArchivedNowPending,
			UnhandledCase,
			Error
		}

		enum member_status
		{
			subscribed,
			unsubscribed,
			cleaned,
			pending,
			transactional,
			archived,
			unrecognisable
		}

		[Serializable]
		class SubscribeRequest
		{
			public string email_address;
			public string status;
			public string email_type;
			public MergeFields merge_fields;
		}

		[Serializable]
		class MergeFields
		{
			public string FNAME;
			public string LNAME;
		}

		[Serializable]
		class MemberResponse
		{
			public string status;
		}

		private readonly string apiKey;
		private readonly string dataCenter;

		public MailchimpSubscriber(string apiKey, string dataCenter)
		{
			this.apiKey = apiKey;
			this.dataCenter = dataCenter;
		}

		public async void TrySubscribing(string listID, string email, string firstName, string lastName, EmailType emailType, Action onValidationSuccess, Action<ValidationError> onValidationError, Action<SubscribeResult> onAPICallFinished)
		{
			var validated = ValidateInput(email, onValidationSuccess, onValidationError);
			if (validated)
			{
				SubscribeResult? result = null;
				await Task.Run(() =>
				{
					if (CheckListID(listID))
					{
						result = TrySubscribing(listID, email, firstName, lastName, EmailType.HTML);
					}
					else
					{
						result = SubscribeResult.Error;
					}
				});
				onAPICallFinished(result.Value);
			}
		}

		private bool ValidateInput(string email, Action onSuccess, Action<ValidationError> onError)
		{
			if (!ValidateEmail(email))
			{
				onError(ValidationError.EmailIllFormatted);
				return false;
			}
			else
			{
				onSuccess();
				return true;
			}
		}

		private bool ValidateEmail(string email)
		{
			try
			{
				var address = new MailAddress(email);
				return address.Address == email;
			}
			catch
			{
				return false;
			}
		}

		private bool CheckListID(string listID)
		{
			var path = $"lists/{listID}";
			try
			{
				CallMailChimpAPI("GET", path);
				return true;
			}
			catch (WebException e)
			{
				Debug.LogError(e.ToString());
				return false;
			}
		}

		private SubscribeResult TrySubscribing(string listID, string email, string firstName, string lastName, EmailType emailType)
		{
			var md5Hash = email.ComputeMD5Hash();
			var path = $"lists/{listID}/members/{md5Hash}";

			try
			{
				var memberResponse = CallMailChimpAPI("GET", path);
				var member_status = GetMemberStatus(memberResponse);
				if (member_status == member_status.pending)
				{
					ChangeMemberStatus(path, email, member_status.unsubscribed, firstName, lastName, emailType);
					ChangeMemberStatus(path, email, member_status.pending, firstName, lastName, emailType);
					return SubscribeResult.ResetPending;
				}
				else if (member_status == member_status.subscribed)
				{
					return SubscribeResult.AlreadySubscribed;
				}
				else if (member_status == member_status.unsubscribed)
				{
					ChangeMemberStatus(path, email, member_status.pending, firstName, lastName, emailType);
					return SubscribeResult.WasUnsubscribedNowPending;
				}
				else if (member_status == member_status.archived)
				{
					ChangeMemberStatus(path, email, member_status.pending, firstName, lastName, emailType);
					return SubscribeResult.WasArchivedNowPending;
				}
				return SubscribeResult.UnhandledCase;
			}
			catch (WebException e)
			{
				var errorResponse = e.Response as HttpWebResponse;
				var memberNotFound = errorResponse.StatusCode == HttpStatusCode.NotFound;
				if (memberNotFound)
				{
					ChangeMemberStatus(path, email, member_status.pending, firstName, lastName, emailType);
					return SubscribeResult.Pending;
				}
				else
				{
					// idea:
					// BadRequest can mean that the email got deleted, and in this case we can't resubscribe it via API because it counts as an import.
					// (Because if users quit the list on their own, we shouldn't be able to reimport them)
					// https://wordpress.org/support/topic/mailchimp-api-error-400-bad-request-forgotten-email-not-subscribed/
					return SubscribeResult.Error;
				}
			}
		}

		private void ChangeMemberStatus(string path, string email, member_status memberStatus, string firstName, string lastName, EmailType emailType)
		{
			var subscribeRequest = new SubscribeRequest()
			{
				email_address = email,
				status = memberStatus.ToString(),
				email_type = emailType == EmailType.HTML ? "html" : "text",
				merge_fields = new MergeFields()
				{
					FNAME = firstName,
					LNAME = lastName
				}
			};

			var requestJson = JsonUtility.ToJson(subscribeRequest);
			CallMailChimpAPI("PUT", path, requestJson);
		}

		private member_status GetMemberStatus(string jsonText)
		{
			var memberResponse = JsonUtility.FromJson<MemberResponse>(jsonText);
			var memberStatus = memberResponse.status;
			if (memberStatus == "subscribed") return member_status.subscribed;
			else if (memberStatus == "unsubscribed") return member_status.unsubscribed;
			else if (memberStatus == "cleaned") return member_status.cleaned;
			else if (memberStatus == "pending") return member_status.pending;
			else if (memberStatus == "transactional") return member_status.transactional;
			else if (memberStatus == "archived") return member_status.archived;
			else return member_status.unrecognisable;
		}

		private string CallMailChimpAPI(string method, string path, string requestJson = null)
		{
			var endpoint = $"https://{dataCenter}.api.mailchimp.com/3.0/{path}";
			var request = HttpWebRequest.Create(endpoint);

			request.Method = method;
			request.ContentType = "application/json";
			SetBasicAuthHeader(request, "anystring", apiKey);

			if (requestJson != null)
			{
				var dataStream = Encoding.UTF8.GetBytes(requestJson);
				request.ContentLength = dataStream.Length;
				var newstream = request.GetRequestStream();
				newstream.Write(dataStream, 0, dataStream.Length);
				newstream.Close();
			}

			using (var response = request.GetResponse())
			{
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var jsonText = reader.ReadToEnd();
					return jsonText;
				}
			}
		}

		private void SetBasicAuthHeader(WebRequest request, string username, string password)
		{
			var auth = $"{username}:{password}";
			auth = Convert.ToBase64String(Encoding.Default.GetBytes(auth));
			request.Headers["Authorization"] = $"Basic {auth}";
		}
	}
}