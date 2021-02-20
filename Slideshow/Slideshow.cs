using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PortgateLib.Slideshow
{
	using PortgateLib.Timer;

	[Serializable]
	public class Slide
	{
		public string Source;
		public string Link;
	}

	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(RectMask2D))]
	public class Slideshow : MonoBehaviour
	{
		public float SlideDuration;
		public float SwapDuration;
		public Slide[] Slides;

		private RectTransform NextSlide
		{
			get { return slideRects[nextSlideIndex]; }
		}

		private Vector2 Size
		{
			get { return rectTransform.rect.size; }
		}

		private bool inited;
		private RectTransform rectTransform;
		private Timer slideTimer;
		private Timer swapTimer;
		private Action onSuccess;
		private List<RectTransform> slideRects = new List<RectTransform>();
		private int downloadedSlides;
		private int nextSlideIndex;
		private RectTransform currentSlide;

		public void Init(Action onSuccess = null, Action<string> onFail = null)
		{
			rectTransform = GetComponent<RectTransform>();
			slideTimer = new Timer(SlideDuration, OnDurationOver);
			swapTimer = new Timer(SwapDuration, OnSwapFinished);
			this.onSuccess = onSuccess;
			if (Slides.Length > 0)
			{
				CreateSlides(onFail);
				StepSlide();
				slideTimer.ResetStart();
			}
		}

		private void CreateSlides(Action<string> onFail)
		{
			for (int i = 0; i < Slides.Length; i++)
			{
				var slide = Slides[i];
				var source = slide.Source;
				var link = slide.Link;
				var slideRect = CreateSlide(source, link, onFail);
				slideRects.Add(slideRect);
			}
		}

		private RectTransform CreateSlide(string source, string link, Action<string> onFail)
		{
			var slideRect = CreateRectTransform(source);
			var rawImageWebLoader = slideRect.gameObject.AddComponent<ImageWebLoader>();
			rawImageWebLoader.StartLoadingImage(source, OnDownloadSuccess, onFail);
			if (link != "" && link != null)
			{
				AddButton(slideRect.gameObject, link);
			}
			return slideRect;
		}

		private RectTransform CreateRectTransform(string name)
		{
			var slide = new GameObject(name);
			slide.transform.SetParent(transform, false);
			var slideRect = slide.AddComponent<RectTransform>();
			slideRect.sizeDelta = Size;
			slideRect.anchoredPosition = new Vector2(Size.x, 0);
			return slideRect;
		}

		private void OnDownloadSuccess()
		{
			downloadedSlides++;
			if (downloadedSlides == slideRects.Count)
			{
				OnAllDownloadSuccess();
			}
		}

		private void OnAllDownloadSuccess()
		{
			if (onSuccess != null)
			{
				onSuccess();
			}
			inited = true;
		}

		private void AddButton(GameObject gameObject, string url)
		{
			var button = gameObject.AddComponent<Button>();
			button.onClick.AddListener(() => Application.OpenURL(url));
		}

		void Update()
		{
			if (inited)
			{
				slideTimer.Update();
				swapTimer.Update();
				if (swapTimer.IsRunning)
				{
					var x = Mathf.SmoothStep(Size.x, 0, swapTimer.ElapsedPercent);
					currentSlide.anchoredPosition = new Vector2(-Size.x + x, 0);
					NextSlide.anchoredPosition = new Vector2(x, 0);
				}
			}
		}

		private void OnDurationOver()
		{
			swapTimer.ResetStart();
		}

		private void OnSwapFinished()
		{
			StepSlide();
		}

		private void StepSlide()
		{
			currentSlide = NextSlide;
			currentSlide.anchoredPosition = Vector2.zero;
			nextSlideIndex = nextSlideIndex.ModifyInCyclicRange(1, 0, slideRects.Count - 1);
			slideTimer.ResetStart();
		}
	}
}