using System.Collections.Generic;
using Firebase.RemoteConfig;

namespace Funzilla
{
	internal class Config : Singleton<Config>
	{
#if UNITY_ANDROID
		private const string IronSrcID = "106faafd1";
#else
		public const string IronSrcID = "106faafd1";
#endif

		internal bool Initialized { get; private set; } = false;
		internal float InterstitialCappingTime { get; private set; } = 45f;
		internal float FirstInterstitialCappingTime { get; private set; } = 30f;
		internal float InterstitialRewardedVideoCappingTime { get; private set; } = 45f;
		internal int GamesForInterstitial { get; private set; } = 10;

		internal string IronSourceId { get; private set; } = IronSrcID;

		internal bool CheatEnabled { get; private set; } = true;
		internal bool BannerEnabled { get; private set; } = true;

#if !UNITY_EDITOR && UNITY_IOS
		public bool iOS14TrackingPromptEnabled => true;
#endif
		private enum State
		{
			None,
			Initializing,
			Initialized,
			Fetched
		}

		State _state = State.None;

		public void Init()
		{
			if (_state != State.None)
			{
				return;
			}

			_state = State.Initializing;

			var defaults = new Dictionary<string, object>
			{
				{"ironsource_id", IronSrcID},
				{"interstitial_capping_time", InterstitialCappingTime},
				{"interstitial_reward_capping_time", InterstitialRewardedVideoCappingTime},
				{"first_interstitial_capping_time", FirstInterstitialCappingTime},
				{"games_for_interstitial", GamesForInterstitial},
				{"cheat_enabled", CheatEnabled}
			};

			FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWith((t1) =>
			{
				if (!t1.IsCompleted)
				{
					return;
				}

				_state = State.Initialized;
				enabled = true;
				FirebaseRemoteConfig.DefaultInstance.FetchAndActivateAsync().ContinueWith(t2 =>
				{
					if (t2.IsCompleted)
					{
						_state = State.Fetched;
						enabled = true;
					}
				});
			});
		}

		private void Update()
		{
			switch (_state)
			{
				case State.None:
				case State.Initializing:
					break;
				case State.Initialized:
				case State.Fetched:
					LoadConfigs();
					break;
			}
		}

		private void LoadConfigs()
		{
			enabled = false;
			var config = FirebaseRemoteConfig.DefaultInstance;
			IronSourceId = config.GetValue("ironsource_id").StringValue;
			InterstitialCappingTime = (float) config.GetValue("interstitial_capping_time").DoubleValue;
			InterstitialRewardedVideoCappingTime =
				(float) config.GetValue("interstitial_reward_capping_time").DoubleValue;
			FirstInterstitialCappingTime = (float) config.GetValue("first_interstitial_capping_time").DoubleValue;
			GamesForInterstitial = (int) config.GetValue("games_for_interstitial").LongValue;
			CheatEnabled = config.GetValue("cheat_enabled").BooleanValue;
			EventManager.Instance.Annouce(EventType.ConfigsLoaded);
			Ads.Instance.Init();
		}
	}
}