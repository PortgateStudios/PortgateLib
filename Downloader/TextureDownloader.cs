using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace PortgateLib.Downloader
{
	public static class TextureDownloader
	{
		public static IEnumerator StartDownload(string url, Action<Texture2D> onSuccess, Action<string> onFail = null)
		{
			if (Application.internetReachability != NetworkReachability.NotReachable)
			{
				using (var webRequest = UnityWebRequestTexture.GetTexture(url))
				{
					yield return webRequest.SendWebRequest();

					if (webRequest.result == UnityWebRequest.Result.Success)
					{
						var texture = DownloadHandlerTexture.GetContent(webRequest);
						var textureWithMipMap = GenerateMipMappedTexture(texture);
						onSuccess(textureWithMipMap);
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

		private static Texture2D GenerateMipMappedTexture(Texture2D texture)
		{
			var textureWithMipMap = new Texture2D(texture.width, texture.height);
			textureWithMipMap.SetPixels(texture.GetPixels(0));
			textureWithMipMap.Apply();
			return textureWithMipMap;
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