using System;
using UnityEngine;
using UnityEngine.UI;

namespace PortgateLib.UI
{
	public class FadeSystem : PersistentSystem
	{
		public static FadeSystem ins
		{
			get { return GetInstance(typeof(FadeSystem)) as FadeSystem; }
		}

		private static readonly string CANVAS_NAME = "FadeCanvas";
		private static readonly int CANVAS_SORTING_ORDER = 999;

		[SerializeField]
		private FadeableGroup fadePrefab;
		[SerializeField]
		private float defaultFadeDuration = 0.33f;
		[SerializeField]
		private Vector2 canvasReferenceResolution = new Vector2(1920, 1080);

		private FadeableGroup fade;
		private Action onFadeFinishedCallback;

		public void StartFading(FadeType fadeType, Action onFadeFinishedCallback = null, float duration = -1, bool reset = false)
		{
			if (fade == null)
			{
				fade = CreateFade();
			}

			this.onFadeFinishedCallback = onFadeFinishedCallback;

			if (duration < 0)
			{
				duration = defaultFadeDuration;
			}
			fade.OverrideDuration(duration);

			if (reset)
			{
				var shouldFadeBeVisibleAtStart = fadeType == FadeType.In ? true : false;
				fade.SetVisible(shouldFadeBeVisibleAtStart);
			}

			// Because if we want the _Screen_ to fade _in_, then we want the _Fade Image_ to fade _out_.
			var invertedFadeType = fadeType == FadeType.In ? FadeType.Out : FadeType.In;
			fade.StartFading(invertedFadeType, OnFadeFinished);
		}

		private FadeableGroup CreateFade()
		{
			var canvas = CreateCanvas();
			var fadeObject = GameObject.Instantiate(fadePrefab.gameObject, canvas.transform);
			return fadeObject.GetComponent<FadeableGroup>();
		}

		private Transform CreateCanvas()
		{
			var go = new GameObject(CANVAS_NAME);
			var canvas = AddCanvas(go);
			AddCanvasScaler(go);
			go.AddComponent<GraphicRaycaster>();
			return canvas.transform;
		}

		private Canvas AddCanvas(GameObject gameObject)
		{
			var canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = CANVAS_SORTING_ORDER;
			return canvas;
		}

		private void AddCanvasScaler(GameObject gameObject)
		{
			var canvasScaler = gameObject.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			canvasScaler.referenceResolution = canvasReferenceResolution;
		}

		private void OnFadeFinished()
		{
			onFadeFinishedCallback?.Invoke();
		}
	}
}