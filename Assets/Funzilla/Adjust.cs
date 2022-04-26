using com.adjust.sdk;
using Firebase.Analytics;
using UnityEngine;

namespace Funzilla
{
	internal class Adjust : Singleton<Adjust>
	{
		internal const string LevelStart = "lc46gk";
		internal const string RwClicked = "jkr7ea";
		internal const string RwShown = "l63imw";
		internal const string RwWatched = "rh2ue9";
		internal const string FsShown = "rh2ue9";

		private const string AppToken = "y0nr9phsp340";
		[SerializeField] private AdjustLogLevel logLevel = AdjustLogLevel.Verbose;
		[SerializeField] private AdjustEnvironment environment = AdjustEnvironment.Production;

		private bool Initialized { get; set; }

		internal void Init()
		{
			var adjustConfig = new AdjustConfig(AppToken, environment, true);
			adjustConfig.setLogLevel(logLevel);
			adjustConfig.setSendInBackground(false);
			adjustConfig.setEventBufferingEnabled(false);
			adjustConfig.setLaunchDeferredDeeplink(true);
			adjustConfig.setAttributionChangedDelegate(OnAttributionChanged);
			com.adjust.sdk.Adjust.start(adjustConfig);
			Initialized = true;

#if UNITY_ANDROID
			com.adjust.sdk.Adjust.getGoogleAdId(SetUserId);
#elif UNITY_IOS
			SetUserId(com.adjust.sdk.Adjust.getIdfa());
#endif
		}

		private static void SetUserId(string userId)
		{
			if (!string.IsNullOrEmpty(userId))
			{
				FirebaseAnalytics.SetUserId(userId);
			}
		}

		internal static void TrackEvent(string id, string name, string param)
		{
			if (!Instance.Initialized)
			{
				return;
			}
			var adjustEvent = new AdjustEvent(id);
			adjustEvent.addCallbackParameter(name, param);
			com.adjust.sdk.Adjust.trackEvent(adjustEvent);
		}

		internal static void TrackEvent(string id)
		{
			if (!Instance.Initialized)
			{
				return;
			}
			var adjustEvent = new AdjustEvent(id);
			com.adjust.sdk.Adjust.trackEvent(adjustEvent);
		}

		private static void OnAttributionChanged(AdjustAttribution attribution)
		{
		}
	}

}