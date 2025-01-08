using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace PortgateLib.FileBasedPrefs
{
	public static class FilePrefs
	{
		private static readonly object fileLock = new();
		private static string saveFileName = "save.sfg";
		private static bool autoSave = true;
		private static bool prettyPrint = true;
		private static bool scrambleData;
		private static string scrambleKey;
		private static FilePrefsData latestData;

		private const string EMPTY_STRING = "";

		#region Initialisation

		public static void Init(string saveFileName, bool autoSave = true, bool prettyPrint = true)
		{
			FilePrefs.saveFileName = saveFileName;
			FilePrefs.autoSave = autoSave;
			FilePrefs.prettyPrint = prettyPrint;
		}

		public static void Init(string saveFileName, bool autoSave, bool prettyPrint, bool scrambleData, string scrambleKey)
		{
			Init(saveFileName, autoSave, prettyPrint);
			FilePrefs.scrambleData = scrambleData;
			FilePrefs.scrambleKey = scrambleKey;
		}

		#endregion

		#region Public Get, Set and util

		public static void SetString(string key, string value = EMPTY_STRING)
		{
			AddDataToSaveFile(key, value);
		}

		public static string GetString(string key, string defaultValue = EMPTY_STRING)
		{
			return (string)GetDataFromSaveFile(key, defaultValue);
		}

		public static void SetInt(string key, int value = default)
		{
			AddDataToSaveFile(key, value);
		}

		public static int GetInt(string key, int defaultValue = default)
		{
			return (int)GetDataFromSaveFile(key, defaultValue);
		}

		public static void SetFloat(string key, float value = default)
		{
			AddDataToSaveFile(key, value);
		}

		public static float GetFloat(string key, float defaultValue = default)
		{
			return (float)GetDataFromSaveFile(key, defaultValue);
		}

		public static void SetBool(string key, bool value = default)
		{
			AddDataToSaveFile(key, value);
		}

		public static bool GetBool(string key, bool defaultValue = default)
		{
			return (bool)GetDataFromSaveFile(key, defaultValue);
		}

		public static bool HasKey(string key)
		{
			return GetSaveFile().HasKey(key);
		}

		public static bool HasKeyForString(string key)
		{
			return GetSaveFile().HasKeyFromObject(key, string.Empty);
		}

		public static bool HasKeyForInt(string key)
		{
			return GetSaveFile().HasKeyFromObject(key, default(int));
		}

		public static bool HasKeyForFloat(string key)
		{
			return GetSaveFile().HasKeyFromObject(key, default(float));
		}

		public static bool HasKeyForBool(string key)
		{
			return GetSaveFile().HasKeyFromObject(key, default(bool));
		}

		public static void DeleteKey(string key)
		{
			GetSaveFile().DeleteKey(key);
			SaveSaveFile();
		}

		public static void DeleteString(string key)
		{
			GetSaveFile().DeleteString(key);
			SaveSaveFile();
		}

		public static void DeleteInt(string key)
		{
			GetSaveFile().DeleteInt(key);
			SaveSaveFile();
		}

		public static void DeleteFloat(string key)
		{
			GetSaveFile().DeleteFloat(key);
			SaveSaveFile();
		}

		public static void DeleteBool(string key)
		{
			GetSaveFile().DeleteBool(key);
			SaveSaveFile();
		}

		public static void DeleteAll()
		{
			CreateNewSaveFile();
			latestData = new FilePrefsData();
		}

		public static void OverwriteLocalSaveFile(string data)
		{
			WriteToSaveFile(data);
			latestData = null;
		}

		#endregion

		#region Read data

		private static FilePrefsData GetSaveFile()
		{
			CheckSaveFileExists();
			if (latestData == null)
			{
				var saveFileText = File.ReadAllText(GetSaveFilePath());
				if (scrambleData)
				{
					saveFileText = DataScrambler(saveFileText);
				}
				try
				{
					latestData = JsonUtility.FromJson<FilePrefsData>(saveFileText);
				}
				catch (ArgumentException e)
				{
					Debug.LogException(new Exception("SAVE FILE IN WRONG FORMAT, CREATING NEW SAVE FILE : " + e.Message));
					DeleteAll();
				}
			}
			return latestData;
		}

		public static string GetSaveFilePath()
		{
			return Path.Combine(Application.persistentDataPath, saveFileName);
		}

		public static string GetSaveFileAsJson()
		{
			CheckSaveFileExists();
			return File.ReadAllText(GetSaveFilePath());
		}

		private static object GetDataFromSaveFile(string key, object defaultValue)
		{
			return GetSaveFile().GetValueFromKey(key, defaultValue);
		}

		#endregion


		#region write data

		private static void AddDataToSaveFile(string key, object value)
		{
			GetSaveFile().UpdateOrAddData(key, value);
			SaveSaveFile();
		}

		public static void ManuallySave()
		{
			SaveSaveFile(true);
		}

		private static void SaveSaveFile(bool manualSave = false)
		{
			if (autoSave || manualSave)
			{
				WriteToSaveFile(JsonUtility.ToJson(GetSaveFile(), prettyPrint));
			}
		}

		private static void WriteToSaveFile(string data)
		{
			if (Monitor.TryEnter(fileLock, TimeSpan.FromMilliseconds(500)))
			{
				try
				{
					if (scrambleData)
					{
						data = DataScrambler(data);
					}

					var filePath = GetSaveFilePath();
					var tempFilePath = $"{filePath}.tmp";
					var backupFilePath = $"{filePath}.backup";

					var maxAttemptCount = 5;
					var attemptDelayMs = 100;

					for (var attempt = 1; attempt <= maxAttemptCount; attempt++)
					{
						try
						{
							using (var tw = new StreamWriter(tempFilePath))
							{
								tw.Write(data);
							}

							File.Replace(tempFilePath, filePath, backupFilePath);
							break;
						}
						catch (IOException ex)
						{
							if (attempt == maxAttemptCount)
							{
								Debug.LogError($"Failed to write to save file: {ex.Message}");
								throw;
							}

							Thread.Sleep(attemptDelayMs);
						}
					}
				}
				finally
				{
					Monitor.Exit(fileLock);
				}
			}
			else
			{
				Debug.LogError("Could not acquire file lock; skipping save operation.");
			}
		}

		#endregion

		#region File Utils

		private static void CheckSaveFileExists()
		{
			if (!DoesSaveFileExist())
			{
				CreateNewSaveFile();
			}
		}

		private static bool DoesSaveFileExist()
		{
			return File.Exists(GetSaveFilePath());
		}

		private static void CreateNewSaveFile()
		{
			WriteToSaveFile(JsonUtility.ToJson(new FilePrefsData(), prettyPrint));
		}

		private static string DataScrambler(string data)
		{
			string res = "";
			for (int i = 0; i < data.Length; i++)
			{
				res += (char)(data[i] ^ scrambleKey[i % scrambleKey.Length]);
			}
			return res;
		}
		#endregion
	}
}