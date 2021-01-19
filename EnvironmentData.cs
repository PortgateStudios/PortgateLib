using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PortgateLib
{
	public static class EnvironmentData
	{
		public static Dictionary<string, object> Get()
		{
			var data = new Dictionary<string, object>
		{
			{ "Application Name", Application.productName },
			{ "Application Version", Application.version },
			{ "Application Platform", Application.platform.ToString() },
			{ "Application Scene", GetSceneName() },
			{ "Time", System.DateTime.Now.ToUniversalTime().ToString() },
			{ "Device Model", SystemInfo.deviceModel },
			{ "Device Name", SystemInfo.deviceName },
			{ "Device Type", SystemInfo.deviceType.ToString() },
			{ "Device ID", SystemInfo.deviceUniqueIdentifier },
			{ "Device Operating System", SystemInfo.operatingSystem },
			{ "Device Battery Level", SystemInfo.batteryStatus.ToString() + " | " + SystemInfo.batteryLevel },
			{ "System Language", Application.systemLanguage.ToString() },
			{ "System Memory Size", SystemInfo.systemMemorySize },
			{ "Processor Count", SystemInfo.processorCount },
			{ "Processor Type", SystemInfo.processorType },
			{ "Processor Frequency", SystemInfo.processorFrequency },
			{ "Screen Resolution Width", Screen.currentResolution.width},
			{ "Screen Resolution Height", Screen.currentResolution.height },
			{ "Screen DPI", Screen.dpi },
			{ "Screen Fullscreen", Screen.fullScreen },
			{ "Graphics Device Name", SystemInfo.graphicsDeviceName },
			{ "Graphics Device Vendor", SystemInfo.graphicsDeviceVendor },
			{ "Graphics Memory Size", SystemInfo.graphicsMemorySize },
			{ "Graphics Device Type", SystemInfo.graphicsDeviceType },
			{ "Graphics Device Version", SystemInfo.graphicsDeviceVersion },
			{ "Graphics Supports Instancing", SystemInfo.supportsInstancing },
			{ "Graphics Shader Level", SystemInfo.graphicsShaderLevel },
			{ "Graphics Uses Reversed Z Buffer", SystemInfo.usesReversedZBuffer },
			{ "Graphics UV Starts At Top", SystemInfo.graphicsUVStartsAtTop },
			{ "Max Texture Size", SystemInfo.maxTextureSize },
			{ "Unity Version", Application.unityVersion }
		};
			return data;
		}

		private static string GetSceneName()
		{
			return SceneManager.GetActiveScene().name;
		}
	}
}