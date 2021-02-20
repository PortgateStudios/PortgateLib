using System;
using System.Linq;
using UnityEngine;
using PortgateLib.Downloader;

namespace PortgateLib.Slideshow
{
	public class PortgateSlideshow : MonoBehaviour
	{
		private static readonly string CONTROL_FILE_FOLDER = "http://portgatestudios.com/portgateslideshow";

		[Serializable]
		class SlideJSON
		{
			public string source = "";
			public string link = "";
		}

		class ControlJSON
		{
			public SlideJSON[] slides;
		};

		public float SlideDuration;
		public float SwapDuration;
		public string ControlFileName;

		private Slideshow slideshow;
		private Action onSuccess;
		private Action<string> onFail;

		public void Init(Action onSuccess = null, Action<string> onFail = null)
		{
			slideshow = gameObject.AddComponent<Slideshow>();
			slideshow.SlideDuration = SlideDuration;
			slideshow.SwapDuration = SwapDuration;
			this.onSuccess = onSuccess;
			this.onFail = onFail;
			var controlFileUrl = $"{CONTROL_FILE_FOLDER}/{ControlFileName}.json";
			StartCoroutine(JSONDownloader.StartDownload(controlFileUrl, OnJSONDownloadSuccess, onFail));
		}

		private void OnJSONDownloadSuccess(string json)
		{
			slideshow.Slides = ParseFromJSON(json);
			slideshow.Init(OnSlideShowInitSuccess, onFail);
		}

		private Slide[] ParseFromJSON(string jsonText)
		{
			try
			{
				Debug.Log(jsonText);
				var controlJSON = JsonUtility.FromJson<ControlJSON>(jsonText);
				var slides = controlJSON.slides.Select(json => new Slide() { Source = json.source, Link = json.link }).ToArray();
				return slides;
			}
			catch (Exception exception)
			{
				if (onFail != null)
				{
					onFail(exception.ToString() + '\n' + exception.StackTrace);
				}
				return new Slide[0];
			}
		}

		private void OnSlideShowInitSuccess()
		{
			if (onSuccess != null)
			{
				onSuccess();
			}
		}
	}
}