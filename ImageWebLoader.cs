using System;
using UnityEngine;
using UnityEngine.UI;
using PortgateLib.Downloader;

namespace PortgateLib
{
	[RequireComponent(typeof(Image))]
	public class ImageWebLoader : MonoBehaviour
	{
		private Action onSuccess;

		public void StartLoadingImage(string url, Action onSuccess = null, Action<string> onFail = null)
		{
			this.onSuccess = onSuccess;
			StartCoroutine(TextureDownloader.StartDownload(url, OnTextureDownloaded, onFail));
		}

		private void OnTextureDownloaded(Texture2D texture)
		{
			var rect = new Rect(0, 0, texture.width, texture.height);
			var sprite = Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100);
			GetComponent<Image>().sprite = sprite;
			if (onSuccess != null)
			{
				onSuccess();
			}
		}
	}
}