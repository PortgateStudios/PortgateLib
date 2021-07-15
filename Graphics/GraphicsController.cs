using System;
using System.Collections;
using UnityEngine;

namespace PortgateLib.Graphics
{
	public class GraphicsController
	{
		public event Action<FullScreenMode> OnFullScreenModeChanged;
		public event Action<Vector2Int> OnDimensionsChanged;
		public event Action<int> OnRefreshRateChanged;
		public event Action<bool> OnRefreshRateChangeable;
		public event Action<int> OnTargetFrameRateChanged;
		public event Action<bool> OnTargetFrameRateChangeable;
		public event Action<bool> OnVerticalSyncChanged;

		// Some fun facts about Screen.currentResolution:
		// ExclusiveFullScreen: dimensions are the appearing dimensions, refresh rate is the resolution's
		// Windowed: both dimensions and refresh rate is the screen's
		// FullScreenWindowed, MaximizedWindowed: dimensions is the appearing resolution, the refresh rate is the screen's

		private FullScreenMode _fullScreenMode;
		public FullScreenMode FullScreenMode
		{
			get { return _fullScreenMode; }
			private set
			{
				_fullScreenMode = value;
				if (OnFullScreenModeChanged != null)
				{
					OnFullScreenModeChanged(value);
				}

				var isGoingIntoExclusiveFullScreen = value == FullScreenMode.ExclusiveFullScreen;
				if (isGoingIntoExclusiveFullScreen)
				{
					// This changes the refresh rate, or at least changes it to the current one.
					// The "change" to the current one is needed so the event will be dispatched.
					// Thus the selected element can be updated on the UI. (because due to dimension change the refresh list has changed, thus the selected one is just the first one)
					RefreshRate = GetCurrentOrFirstAvailableRefreshRate();
				}
			}
		}

		private Vector2Int _dimensions;
		public Vector2Int Dimensions
		{
			get { return _dimensions; }
			private set
			{
				_dimensions = value;
				if (OnDimensionsChanged != null)
				{
					OnDimensionsChanged(value);
				}

				// This changes the refresh rate, or at least changes it to the current one.
				// The "change" to the current one is needed so the event will be dispatched.
				// Thus the selected element can be updated on the UI. (because due to dimension change the refresh list has changed, thus the selected one is just the first one)
				RefreshRate = GetCurrentOrFirstAvailableRefreshRate();
			}
		}

		private int _refreshRate;
		public int RefreshRate
		{
			get { return _refreshRate; }
			private set
			{
				_refreshRate = value;
				if (OnRefreshRateChanged != null)
				{
					OnRefreshRateChanged(value);
				}
			}
		}

		private int _targetFrameRate;
		public int TargetFrameRate
		{
			get { return _targetFrameRate; }
			private set
			{
				_targetFrameRate = value;
				GraphicsUtility.SetTargetFrameRate(value);
				if (OnTargetFrameRateChanged != null)
				{
					OnTargetFrameRateChanged(value);
				}
			}
		}

		private bool _verticalSync;
		public bool VerticalSync
		{
			get { return _verticalSync; }
			private set
			{
				_verticalSync = value;
				GraphicsUtility.SetVerticalSync(value);
				if (OnVerticalSyncChanged != null)
				{
					OnVerticalSyncChanged(value);
				}
			}
		}

		private Action onUpdateStarted;
		private MonoBehaviour owner;
		private Action onUpdateFinished;

		public GraphicsController(Action onUpdateStarted = null, MonoBehaviour owner = null, Action onUpdateFinished = null)
		{
			_fullScreenMode = FullScreenMode.FullScreenWindow;
			_dimensions = Screen.currentResolution.GetDimensions();
			_refreshRate = GraphicsUtility.GetScreenRefreshRate();
			_targetFrameRate = Application.targetFrameRate;
			_verticalSync = false;
			this.onUpdateStarted = onUpdateStarted;
			this.owner = owner;
			this.onUpdateFinished = onUpdateFinished;
		}

		public void Load(FullScreenMode fullScreenMode, Resolution resolution, int targetFrameRate, bool verticalSync)
		{
			DispatchOnUpdateStarted();
			FullScreenMode = fullScreenMode;
			Dimensions = resolution.GetDimensions();
			RefreshRate = GraphicsUtility.GetRefreshRateAsItsInTheSupportedList(FullScreenMode, Dimensions, resolution.refreshRate);
			TargetFrameRate = targetFrameRate;
			VerticalSync = verticalSync;
			UpdateGraphics();
			DispatchOnUpdateFinished();
		}

		public void ChangeFullScreenMode(FullScreenMode fullScreenMode)
		{
			UpdateGraphics(() => FullScreenMode = fullScreenMode);
		}

		public void ChangeDimensions(Vector2Int dimensions)
		{
			UpdateGraphics(() => Dimensions = dimensions);
		}

		public void ChangeRefreshRate(int refreshRate)
		{
			UpdateGraphics(() => RefreshRate = refreshRate);
		}

		public void ChangeTargetFrameRate(int targetFrameRate)
		{
			UpdateGraphics(() => TargetFrameRate = targetFrameRate);
		}

		public void ChangeVerticalSync(bool enabled)
		{
			UpdateGraphics(() => VerticalSync = enabled);
		}

		private void UpdateGraphics(Action changeProperty = null)
		{
			DispatchOnUpdateStarted();

			if (changeProperty != null)
			{
				changeProperty();
			}

			GraphicsUtility.SetupWindow(Dimensions.x, Dimensions.y, FullScreenMode, RefreshRate);

			// if somebody is interested on the results, we should actualise the model once the screen setup has changed and notify it about it. (i.e. GraphicsSettings UI)
			if (owner != null)
			{
				owner.StartCoroutine(WaitForScreenToCatchUp());
			}
		}

		private IEnumerator WaitForScreenToCatchUp()
		{
			while (!HasScreenCaughtUp())
			{
				yield return new WaitForEndOfFrame();
			}
			OnScreenHasCaughtUp();
		}

		private bool HasScreenCaughtUp()
		{
			var resolution = GraphicsUtility.CreateResolution(Dimensions.x, Dimensions.y, RefreshRate);
			// In Windowed, currentResolution will be the Screen's, so most likely it won't match with the Window's.
			var areDimensionsMatchingIfItsNonWindowed = resolution.HasSameDimensions(Screen.currentResolution) || FullScreenMode == FullScreenMode.Windowed;
			var isFullScreenModeMatching = FullScreenMode == Screen.fullScreenMode;
			var hasScreenCaughtUp = areDimensionsMatchingIfItsNonWindowed && isFullScreenModeMatching;
			return hasScreenCaughtUp;
		}

		private void OnScreenHasCaughtUp()
		{
			var isExclusiveFullScreen = FullScreenMode == FullScreenMode.ExclusiveFullScreen;

			if (!isExclusiveFullScreen)
			{
				var screenRefresh = GraphicsUtility.GetScreenRefreshRate();
				RefreshRate = screenRefresh;
			}

			if (OnRefreshRateChangeable != null)
			{
				OnRefreshRateChangeable(isExclusiveFullScreen);
			}

			if (OnTargetFrameRateChangeable != null)
			{
				OnTargetFrameRateChangeable(!VerticalSync);
			}

			if (VerticalSync)
			{
				// UnityDocs:
				// Additionally if the QualitySettings.vSyncCount property is set, the targetFrameRate will be ignored and instead the game will use the vSyncCount and the platform's default render rate to determine the target frame rate.
				// For example, if the platform's default render rate is 60 frames per second and vSyncCount is set to 2, the game will target 30 frames per second.
				// Tudvari:
				// So setting the TargetFrameRate to the screen refresh rate is purely cosmetical, thus it's only needed to be actualised if somebody is listening. (That's why it's only in the optional OnScreenHasCaughtUp)
				TargetFrameRate = GetCurrentOrFirstAvailableRefreshRate();
			}

			DispatchOnUpdateFinished();
		}

		private int GetCurrentOrFirstAvailableRefreshRate()
		{
			if (FullScreenMode == FullScreenMode.ExclusiveFullScreen) // refresh rate is the resolution's
			{
				var supportedRefreshRates = GraphicsUtility.GetSupportedExclusiveFullScreenModeRefreshRates(Dimensions);
				var index = supportedRefreshRates.FindIndex(refreshRate => GraphicsUtility.AreRefreshRatesEqual(refreshRate, RefreshRate));
				if (index == -1)
				{
					index = 0;
				}
				return supportedRefreshRates[index];
			}
			else // in other cases refresh rate is the screen's (even if dimension can be screen's or window's based on the mode)
			{
				return Screen.currentResolution.refreshRate;
			}
		}

		private void DispatchOnUpdateStarted()
		{
			if (onUpdateStarted != null)
			{
				onUpdateStarted();
			}
		}

		private void DispatchOnUpdateFinished()
		{
			if (onUpdateFinished != null)
			{
				onUpdateFinished();
			}
		}
	}
}