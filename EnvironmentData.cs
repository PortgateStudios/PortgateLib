using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace PortgateLib
{
	public class EnvironmentData
	{
		public string ApplicationName => Application.productName;
		public string ApplicationVersion => Application.version;
		public string ApplicationPlatform => Application.platform.ToString();
		public string ApplicationScene => SceneManager.GetActiveScene().name;
		public string Time => System.DateTime.Now.ToUniversalTime().ToString();
		public string DeviceModel => SystemInfo.deviceModel;
		public string DeviceName => SystemInfo.deviceName;
		public string DeviceType => SystemInfo.deviceType.ToString();
		public string DeviceID => SystemInfo.deviceUniqueIdentifier;
		public string DeviceOperatingSystem => SystemInfo.operatingSystem;
		public string DeviceBatteryInfo => SystemInfo.batteryStatus.ToString() + " | " + SystemInfo.batteryLevel;
		public string SystemLanguage => Application.systemLanguage.ToString();
		public int SystemMemorySize => SystemInfo.systemMemorySize;
		public int ProcessorCount => SystemInfo.processorCount;
		public string ProcessorType => SystemInfo.processorType;
		public int ProcessorFrequency => SystemInfo.processorFrequency;
		public int ScreenResolutionWidth => Screen.currentResolution.width;
		public int ScreenResolutionHeight => Screen.currentResolution.height;
		public float ScreenDPI => Screen.dpi;
		public bool ScreenFullscreen => Screen.fullScreen;
		public string GraphicsDeviceName => SystemInfo.graphicsDeviceName;
		public string GraphicsDeviceVendor => SystemInfo.graphicsDeviceVendor;
		public int GraphicsMemorySize => SystemInfo.graphicsMemorySize;
		public GraphicsDeviceType GraphicsDeviceType => SystemInfo.graphicsDeviceType;
		public string GraphicsDeviceVersion => SystemInfo.graphicsDeviceVersion;
		public bool GraphicsSupportsInstancing => SystemInfo.supportsInstancing;
		public int GraphicsShaderLevel => SystemInfo.graphicsShaderLevel;
		public bool GraphicsUsesReversedZBuffer => SystemInfo.usesReversedZBuffer;
		public bool GraphicsUVStartsAtTop => SystemInfo.graphicsUVStartsAtTop;
		public int MaxTextureSize => SystemInfo.maxTextureSize;
		public string UnityVersion => Application.unityVersion;

		public virtual Dictionary<string, object> Get()
		{
			var data = new Dictionary<string, object>
			{
				{ "Application Name", ApplicationName },
				{ "Application Version", ApplicationVersion },
				{ "Application Platform", ApplicationPlatform },
				{ "Application Scene", ApplicationScene },
				{ "Time", Time },
				{ "Device Model", DeviceModel },
				{ "Device Name", DeviceName },
				{ "Device Type", DeviceType },
				{ "Device ID", DeviceID },
				{ "Device Operating System", DeviceOperatingSystem },
				{ "Device Battery Level", DeviceBatteryInfo },
				{ "System Language", SystemLanguage },
				{ "System Memory Size", SystemMemorySize },
				{ "Processor Count", ProcessorCount },
				{ "Processor Type", ProcessorType },
				{ "Processor Frequency", ProcessorFrequency },
				{ "Screen Resolution Width", ScreenResolutionWidth },
				{ "Screen Resolution Height", ScreenResolutionHeight },
				{ "Screen DPI", ScreenDPI },
				{ "Screen Fullscreen", ScreenFullscreen },
				{ "Graphics Device Name", GraphicsDeviceName },
				{ "Graphics Device Vendor", GraphicsDeviceVendor },
				{ "Graphics Memory Size", GraphicsMemorySize },
				{ "Graphics Device Type", GraphicsDeviceType.ToString() },
				{ "Graphics Device Version", GraphicsDeviceVersion },
				{ "Graphics Supports Instancing", GraphicsSupportsInstancing },
				{ "Graphics Shader Level", GraphicsShaderLevel },
				{ "Graphics Uses Reversed Z Buffer", GraphicsUsesReversedZBuffer },
				{ "Graphics UV Starts At Top", GraphicsUVStartsAtTop },
				{ "Max Texture Size", MaxTextureSize },
				{ "Unity Version", UnityVersion }
			};
			return data;
		}
	}
}