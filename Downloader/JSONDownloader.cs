using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace PortgateLib.Downloader
{
	public static class JSONDownloader
	{
		public static IEnumerator StartDownload(string url, Action<string> onSuccess, Action<string> onFail = null)
		{
			if (Application.internetReachability != NetworkReachability.NotReachable)
			{
				using (var webRequest = UnityWebRequest.Get(url))
				{
					yield return webRequest.SendWebRequest();

					if (webRequest.result == UnityWebRequest.Result.Success)
					{
						var jsonText = webRequest.downloadHandler.text;
						onSuccess(jsonText);
					}
					else
					{
						CallFailCallback(onFail, webRequest.error);
					}
				}
			}
			else
			{
				CallFailCallback(onFail, "Network not reachable.");
			}
		}

		private static void CallFailCallback(Action<string> onFail, string error)
		{
			if (onFail != null)
			{
				onFail(error);
			}
		}
	}
}