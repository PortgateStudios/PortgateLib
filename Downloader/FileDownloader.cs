using System;
using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace PortgateLib.Downloader
{
	public static class FileDownloader
	{
		public static IEnumerator StartDownload(string url, string destination, Action onSuccess, Action<string> onFail = null)
		{
			if (Application.internetReachability != NetworkReachability.NotReachable)
			{
				using (var webRequest = UnityWebRequestTexture.GetTexture(url))
				{
					yield return webRequest.SendWebRequest();

					if (webRequest.result == UnityWebRequest.Result.Success)
					{
						var bytes = webRequest.downloadHandler.data;
						File.WriteAllBytes(destination, bytes);
						onSuccess();
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