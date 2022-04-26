using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif
namespace Funzilla
{
	public class Utils : Singleton<Utils>
	{
#if UNITY_EDITOR
		// Nothing
#elif UNITY_IOS
		[DllImport("__Internal")] static extern void vibrate(int level);
		[DllImport("__Internal")] static extern bool isVibrationCustomizable();
#elif UNITY_ANDROID
		private AndroidJavaObject activityContext;
		private AndroidJavaObject plugin;
#endif
		bool vibrationCustomizable = false;

		enum VibrationLevel
		{
			Flash,
			Light,
			Medium,
			Heavy,
		}

		public static Color ColorFromUint(uint hex)
		{
			var b = (hex >> 0) & 0xff;
			var g = (hex >> 8) & 0xff;
			var r = (hex >> 16) & 0xff;
			return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
		}

		private void Awake()
		{
#if UNITY_EDITOR
			// Nothing
#elif UNITY_ANDROID
			using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
			}

			using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.dunghn94.utilslibrary.Utils"))
			{
				plugin = pluginClass.CallStatic<AndroidJavaObject>("createInstance", activityContext);
			}

			using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
			{
				vibrationCustomizable = version.GetStatic<int>("SDK_INT") >= 26;
			}
#elif UNITY_IOS
			vibrationCustomizable = isVibrationCustomizable();
#endif
		}


		public void VibrateFlash()
		{
			if (Preference.Instance.VibrationOn && vibrationCustomizable)
			{
				Vibrate(VibrationLevel.Flash);
			}
		}

		public void VibrateLight()
		{
			if (Preference.Instance.VibrationOn && vibrationCustomizable)
			{
				Vibrate(VibrationLevel.Light);
			}
		}

		public void VibrateHeavy()
		{
			if (!Preference.Instance.VibrationOn)
			{
				return;
			}

			if (vibrationCustomizable)
			{
				Vibrate(VibrationLevel.Heavy);
			}
			else
			{
				Handheld.Vibrate();
			}
		}

		public void VibrateMedium()
		{
			if (Preference.Instance.VibrationOn && vibrationCustomizable)
			{
				Vibrate(VibrationLevel.Medium);
			}
		}


		private void Vibrate(VibrationLevel level)
		{
#if UNITY_EDITOR
#elif UNITY_ANDROID
			int miliseconds = 0, amplitude = 0;
			switch (level)
			{
				case VibrationLevel.Flash:
					miliseconds = 6;
					amplitude = 20;
					break;

				case VibrationLevel.Light:
					miliseconds = 20;
					amplitude = 40;
					break;

				case VibrationLevel.Medium:
					miliseconds = 40;
					amplitude = 100;
					break;

				case VibrationLevel.Heavy:
					miliseconds = 80;
					amplitude = 255;
					break;
			}
			plugin.Call("Vibrate", miliseconds, amplitude);
#elif UNITY_IOS
			vibrate((int)level);
#endif
		}


		private void OnTriggerEnter(Collider other)
		{
			Handheld.Vibrate();
		}

		public static float LandDistance(Vector3 v1, Vector3 v2)
		{
			v1.y = 0;
			v2.y = 0;
			return Vector3.Distance(v1, v2);
		}

		public static bool NonUIHold()
		{
#if UNITY_EDITOR
			if (!Input.GetMouseButton(0) || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			{
				return false;
			}
#else
			if (Input.touches.Length <= 0 ||
				Input.touches[0].phase == TouchPhase.Canceled ||
				Input.touches[0].phase == TouchPhase.Ended ||
				UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
			{
				return false;
			}
#endif
			return true;
		}

		public static bool NonUITapped()
		{
#if UNITY_EDITOR
			if (!Input.GetMouseButtonDown(0) || UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
			{
				return false;
			}
#else
			if (Input.touches.Length <= 0 ||
				Input.touches[0].phase != TouchPhase.Began ||
				UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
			{
				return false;
			}
#endif
			return true;
		}

		public static bool TouchReleased()
		{
#if UNITY_EDITOR
			return Input.GetMouseButtonUp(0);
#else
			return
				Input.touches.Length > 0 &&
				(Input.touches[0].phase == TouchPhase.Ended ||
				Input.touches[0].phase == TouchPhase.Canceled);
#endif
		}

		public static Vector2 TouchPosition()
		{
#if UNITY_EDITOR
			return Input.mousePosition;
#else
			return Input.touches[0].position;
#endif
		}

		public static bool IsAppInstalled(string bundleId)
		{
#if UNITY_EDITOR
			return false;
#elif UNITY_ANDROID
			if (bundleId == null || bundleId == "")
			{
				return false;
			}
			bool installed = false;
			AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject curActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject packageManager = curActivity.Call<AndroidJavaObject>("getPackageManager");
			AndroidJavaObject launchIntent = null;
			try
			{
				launchIntent = packageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", bundleId);
				installed = launchIntent != null;
			}
			catch (System.Exception e)
			{
				installed = false;
			}
			return installed;
#else
			return false;
#endif
		}

		public static float RandomCurveValue(AnimationCurve curve)
		{
			return curve.Evaluate(UnityEngine.Random.Range(
				curve.keys[0].time,
				curve.keys[curve.keys.Length - 1].time));
		}

		public static void SetLayerRecursively(GameObject go, int layerNumber)
		{
			foreach (var trans in go.GetComponentsInChildren<Transform>(true))
			{
				trans.gameObject.layer = layerNumber;
			}
		}
	}

#if UNITY_EDITOR
	public static class EditorUtils
	{
		internal static T GetCopyOf<T>(this Component comp, T other) where T : Component
		{
			var type = comp.GetType();
			if (type != other.GetType()) return null; // type mis-match
			const BindingFlags flags =
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
				BindingFlags.Default | BindingFlags.DeclaredOnly;
			var pinfos = type.GetProperties(flags);
			foreach (var pinfo in pinfos)
			{
				if (!pinfo.CanWrite) continue;
				try
				{
					pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
				}
				catch
				{
					// ignored
				} // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
			}

			var finfos = type.GetFields(flags);
			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
			}

			return comp as T;
		}

		public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
		{
			return go.AddComponent<T>().GetCopyOf(toAdd);
		}

		private const float GizmoDiskThickness = 0.01f;

		public static void DrawGizmoDisk(this Transform t, float radius)
		{
			var oldMatrix = Gizmos.matrix;
			Gizmos.matrix = Matrix4x4.TRS(t.position, t.rotation, new Vector3(1, GizmoDiskThickness, 1));
			Gizmos.DrawWireSphere(Vector3.zero, radius);
			Gizmos.matrix = oldMatrix;
		}

		internal static T GetRandomElement<T>(this IEnumerable<T> list)
		{
			var l = list.GetRandomElements(1);
			return l.IsNullOrEmpty() ? default(T) : l[0];
		}

		internal static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
		{
			var enumerable = list as T[] ?? list.ToArray();
			elementsCount = Mathf.Clamp(elementsCount, 0, enumerable.Count());
			return enumerable.OrderBy(arg => System.Guid.NewGuid()).Take(elementsCount).ToList();
		}

		private static readonly System.Random Rng = new System.Random();

		internal static void Shuffle<T>(this IList<T> list)
		{
			var n = list.Count;
			while (n > 1)
			{
				n--;
				var k = Rng.Next(n + 1);
				var value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}

		internal static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			return enumerable switch
			{
				null => true,
				/* If this is a list, use the Count property for efficiency.
			 * The Count property is O(1) while IEnumerable.Count() is O(N). */
				ICollection<T> collection => collection.Count < 1,
				_ => !enumerable.Any()
			};
		}

		/// <summary>
		/// Add or edit key frame
		/// </summary>
		/// <param name="curve"></param>
		/// <param name="key"></param>
		/// <returns>
		/// Return 0 on add new key, 1 when edit existed key
		/// </returns>
		internal static int AddOrEditKeyFrame(this AnimationCurve curve, Keyframe key)
		{
			if (curve.AddKey(key) != -1) return 0;
			for (var i = 0; i < curve.length; i++)
			{
				if (Math.Abs(curve.keys[i].time - key.time) > 0.001f) continue;
				//curve.keys[i] = key;
				var keys = curve.keys;
				keys[i] = key;
				curve.keys = keys;
				return 1;
			}

			return 0;
		}

		internal static bool RandomPercentage(float percent)
		{
			var rdm = Random.Range(0f, 99.99f);
			return rdm < percent;
		}

		internal static double ToRadians(this double val)
		{
			return (Mathf.PI / 180) * val;
		}

		internal static float ToRadians(this float val)
		{
			return (Mathf.PI / 180) * val;
		}

#if UNITY_EDITOR
		[MenuItem("Tools/Clear Data")]
		internal static void ClearGameData()
		{
			File.Delete(Application.persistentDataPath + "/save.dat");

			PlayerPrefs.DeleteAll();
		}
#endif
	}
#endif
}