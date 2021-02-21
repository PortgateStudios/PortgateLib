using System;
using System.IO;
using System.Net;
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

		public enum SubscribeResult
		{
			PENDING,
			RESET_PENDING,
			ALREADY_SUBSCRIBED,
			WAS_UNSUBSCRIBED_NOW_PENDING,
			WAS_ARCHIVED_NOW_PENDING,
			UNHANDLED_CASE
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

		public async void TrySubscribing(string listID, string email, string firstName, string lastName, EmailType emailType, Action<SubscribeResult> onFinished)
		{
			SubscribeResult? result = null;
			await Task.Run(() =>
			{
				CheckListID(listID);
				result = TrySubscribing(listID, email, firstName, lastName, EmailType.HTML);
			});
			onFinished(result.Value);
		}

		private void CheckListID(string listID)
		{
			var path = $"lists/{listID}";
			CallMailChimpAPI("GET", path);
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
					return SubscribeResult.RESET_PENDING;
				}
				else if (member_status == member_status.subscribed)
				{
					return SubscribeResult.ALREADY_SUBSCRIBED;
				}
				else if (member_status == member_status.unsubscribed)
				{
					ChangeMemberStatus(path, email, member_status.pending, firstName, lastName, emailType);
					return SubscribeResult.WAS_UNSUBSCRIBED_NOW_PENDING;
				}
				else if (member_status == member_status.archived)
				{
					ChangeMemberStatus(path, email, member_status.pending, firstName, lastName, emailType);
					return SubscribeResult.WAS_ARCHIVED_NOW_PENDING;
				}
				return SubscribeResult.UNHANDLED_CASE;
			}
			catch (WebException e)
			{
				var errorResponse = e.Response as HttpWebResponse;
				var memberNotFound = errorResponse.StatusCode == HttpStatusCode.NotFound;
				if (memberNotFound)
				{
					ChangeMemberStatus(path, email, member_status.pending, firstName, lastName, emailType);
					return SubscribeResult.PENDING;
				}
				else // some problematic exception
				{
					throw e;
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