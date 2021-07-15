using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PortgateLib.Graphics
{
	public static class GraphicsUtility
	{
		public static Resolution WindowResolution
		{
			get
			{
				return GraphicsUtility.CreateResolution(Screen.width, Screen.height, Screen.currentResolution.refreshRate);
			}
		}

		public static void SetupWindow(int width, int height, FullScreenMode fullScreenMode, int refreshRate)
		{
			Screen.SetResolution(width, height, fullScreenMode, refreshRate);
		}

		public static void SetTargetFrameRate(int targetFrameRate)
		{
			Application.targetFrameRate = targetFrameRate;
		}

		public static void SetVerticalSync(bool enabled)
		{
			QualitySettings.vSyncCount = enabled ? 1 : 0;
		}

		public static Resolution CreateResolution(string resolutionString)
		{
			resolutionString = resolutionString.Replace('@', 'x');
			var dimensions = resolutionString.Split('x');
			var width = int.Parse(dimensions[0]);
			var height = int.Parse(dimensions[1]);
			var refreshRate = int.Parse(dimensions[2]);
			return GraphicsUtility.CreateResolution(width, height, refreshRate);
		}

		public static Resolution CreateResolution(int width, int height, int refreshRate)
		{
			var resolution = new Resolution();
			resolution.width = width;
			resolution.height = height;
			resolution.refreshRate = refreshRate;
			return resolution;
		}

		public static Vector2Int GetDimensions(this Resolution resolution)
		{
			return new Vector2Int(resolution.width, resolution.height);
		}

		public static bool HasDimensions(this Resolution resolution, Vector2Int dimensions)
		{
			return resolution.width == dimensions.x && resolution.height == dimensions.y;
		}

		public static bool HasSameDimensions(this Resolution resolution, Resolution otherResolution)
		{
			var dimensions = new Vector2Int(otherResolution.width, otherResolution.height);
			return resolution.HasDimensions(dimensions);
		}

		public static bool AreRefreshRatesEqual(int refreshRate1, int refreshRate2)
		{
			var delta = refreshRate1 - refreshRate2;
			return Mathf.Abs(delta) <= 1;
		}

		public static int GetScreenRefreshRate()
		{
			var screenDimensions = Screen.currentResolution.GetDimensions();
			var screenRefreshRate = Screen.currentResolution.refreshRate;
			return screenRefreshRate;
		}

		public static int GetRefreshRateAsItsInTheSupportedList(FullScreenMode fullScreenMode, Vector2Int dimensions, int refreshRate)
		{
			var supportedRefreshRates = GetSupportedRefreshRates(fullScreenMode, dimensions);
			var index = supportedRefreshRates.FindIndex(refRate => GraphicsUtility.AreRefreshRatesEqual(refRate, refreshRate));
			if (index == -1)
			{
				index = 0;
			}
			return supportedRefreshRates[index];
		}

		public static int[] GetSupportedRefreshRates(FullScreenMode fullScreenMode, Vector2Int dimensions)
		{
			if (fullScreenMode == FullScreenMode.ExclusiveFullScreen)
			{
				return GetSupportedExclusiveFullScreenModeRefreshRates(dimensions);
			}
			else
			{
				return new int[1] { GraphicsUtility.GetScreenRefreshRate() };
			}
		}

		public static int[] GetSupportedExclusiveFullScreenModeRefreshRates(Vector2Int dimensions)
		{
			var supportedResolutions = Screen.resolutions;
			var resolutionsWithTheSameDimensions = supportedResolutions.Where(res => res.HasDimensions(dimensions));
			var refreshRates = resolutionsWithTheSameDimensions.Select(res => res.refreshRate).ToList();

			var duplicates = new List<int>();
			for (int i = 0; i < refreshRates.Count - 1; i++)
			{
				var current = refreshRates[i];
				var next = refreshRates[i + 1];
				if (AreRefreshRatesEqual(current, next))
				{
					duplicates.Add(current); // remove the current because we want 60 Hz instead of 59 Hz.
				}
			}
			refreshRates.RemoveAll(refreshRate => duplicates.Contains(refreshRate));

			return refreshRates.ToArray();
		}

		public static Vector2Int[] GetSupportedDimensions()
		{
			var supportedResolutions = Screen.resolutions;
			var supportedDimensionsMultipleTimes = supportedResolutions.Select(resolution => new Vector2Int(resolution.width, resolution.height));
			var supportedDimensions = new HashSet<Vector2Int>(supportedDimensionsMultipleTimes);
			return supportedDimensions.ToArray();
		}

		public static FullScreenMode[] GetSupportedFullScreenModes()
		{
			// we ignore MaximizedWindow because it's unsupported on Windows currently. So let's just don't care about it everywhere.
			var maximizedWindow = new FullScreenMode[1] { FullScreenMode.MaximizedWindow };
			return Utility.GetEnumValues<FullScreenMode>().Except(maximizedWindow).ToArray();
		}
	}
}