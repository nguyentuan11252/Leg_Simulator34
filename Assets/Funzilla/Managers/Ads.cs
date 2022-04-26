
using System;
using System.Collections;
using System.Runtime.InteropServices;
using Facebook.Unity;
using UnityEngine;

namespace Funzilla
{
	internal enum RewardedVideoState
	{
		Closed, NotReady, Failed, Watched
	}

	internal enum InterstitialState
	{
		NotReady, LoadFailed, Available
	}

	internal class Ads : Singleton<Ads>
	{
#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")] private static extern bool isIos14();
		[DllImport("__Internal")] private static extern bool advertiserTrackingPrompted();
		[DllImport("__Internal")] private static extern void promptAdvertiserTracking();
		[DllImport("__Internal")] private static extern bool advertiserTrackingEnabled();
#endif
		private const float InterstitialLoadDelayTime = 3.0f;

		private const int MAXInterstitialAttempts = 3;
		private int _nInterstitialAttempts;
		private bool _interstitialShown;
		private bool _showingInterstitial;
		private bool _requestingInterstitial;

		private float _lastInterstitialShowTime;
		private float _lastRewardedVideoShowTime;
		private int _nPlaysUntilLastAd;

		private InterstitialState _interstitialState = InterstitialState.NotReady;

		private bool InterstitialAllowed { get; set; } = true;
		private bool BannerAllowed { get; set; } = false;

		private enum State { NotInitialized, Initializing, Initialized }
		private State _state = State.NotInitialized;

		internal void Init()
		{
			if (_state != State.NotInitialized)
			{
				return;
			}
			_state = State.Initializing;
			_nPlaysUntilLastAd = Profile.Instance.PlayCount;
			_lastInterstitialShowTime = Time.realtimeSinceStartup;

			IronSourceEvents.onRewardedVideoAdShowFailedEvent += OnRewardedVideoAdShowFailed;
			IronSourceEvents.onRewardedVideoAdOpenedEvent += OnRewardedVideoAdOpened;
			IronSourceEvents.onRewardedVideoAdClosedEvent += OnRewardedVideoAdClosed;
			IronSourceEvents.onRewardedVideoAdStartedEvent += OnRewardedVideoAdStarted;
			IronSourceEvents.onRewardedVideoAdEndedEvent += OnRewardedVideoAdEnded;
			IronSourceEvents.onRewardedVideoAdRewardedEvent += OnRewardedVideoAdRewarded;
			IronSourceEvents.onRewardedVideoAdClickedEvent += OnRewardedVideoAdClicked;
			IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += OnRewardedVideoAvailabilityChanged;

			IronSourceEvents.onInterstitialAdReadyEvent += OnInterstitialAdReady;
			IronSourceEvents.onInterstitialAdLoadFailedEvent += OnInterstitialAdLoadFailed;
			IronSourceEvents.onInterstitialAdOpenedEvent += OnInterstitialAdOpened;
			IronSourceEvents.onInterstitialAdClosedEvent += OnInterstitialAdClosed;
			IronSourceEvents.onInterstitialAdShowSucceededEvent += OnInterstitialAdShowSucceeded;
			IronSourceEvents.onInterstitialAdShowFailedEvent += OnInterstitialAdShowFailed;
			IronSourceEvents.onInterstitialAdClickedEvent += OnInterstitialAdClicked;
			#region Banner
			IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
			IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
			IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
			#endregion

#if !UNITY_EDITOR && UNITY_IOS
			if (isIos14() && Config.Instance.iOS14TrackingPromptEnabled)
			{
				promptAdvertiserTracking();
			}
			else
			{
				InitSDK();
			}
#else
			InitSDK();
#endif
		}

#if !UNITY_EDITOR && UNITY_IOS
		private void Update()
		{
			if (!advertiserTrackingPrompted())
			{
				return;
			}
			InitSDK();
		}
#endif

		void InitSDK()
		{
			enabled = false;
			var userId = IronSource.Agent.getAdvertiserId();
			IronSource.Agent.setUserId(userId);
			Firebase.Analytics.FirebaseAnalytics.SetUserId(userId);

#if !UNITY_EDITOR && UNITY_IOS
			FB.Mobile.SetAdvertiserTrackingEnabled(advertiserTrackingEnabled());
#endif

			IronSource.Agent.init(Config.Instance.IronSourceId);
			StartCoroutine(LoadInterstitialWithDelay(InterstitialLoadDelayTime));
			_state = State.Initialized;
#if DEBUG_ENABLED
			IronSource.Agent.validateIntegration();
#endif
		}

		void LoadInterstitial(bool showImediately = false)
		{
			_interstitialState = InterstitialState.NotReady;
			_showingInterstitial = showImediately;
			_nInterstitialAttempts = 0;
			_requestingInterstitial = true;
			IronSource.Agent.loadInterstitial();
		}

		private IEnumerator LoadInterstitialWithDelay(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			LoadInterstitial();
		}

		private Action<RewardedVideoState> _rewardedVideoCallback;
		private RewardedVideoState _rewardedVideoState;
		private string _rewardedVideoTriggerPlace;

#if UNITY_EDITOR
		internal bool RewardedVideoReady => true;
#else
		internal bool RewardedVideoReady => _state == State.Initialized && IronSource.Agent.isRewardedVideoAvailable();
#endif

		private bool InterstitialValid =>
			_state == State.Initialized &&
			!_showingInterstitial &&
			!_requestingInterstitial;

		private void ShowReadyInterstitial(Action onFinished)
		{
			if (!InterstitialAllowed)
			{
				onFinished();
				return;
			}

			_showingInterstitial = false;
			try
			{
				_onIntersitialRequestProcessed = onFinished;
				IronSource.Agent.showInterstitial("FS" + Config.Instance.InterstitialCappingTime);
			}
			catch
			{
				onFinished?.Invoke();
			}
		}

		private bool CanShowInterstitial
		{
			get
			{
				if (Profile.Instance.Vip)
				{
#if DEBUG_ENABLED
					Debug.LogError("Cannot show interstitial to VIP");
#endif
					return false;
				}

				// Check availability
				if (!InterstitialValid)
				{ // Not ready yet
#if DEBUG_ENABLED
					Debug.LogError("Interstitial is not either initialized or loaded");
#endif
					return false;
				}

				// Check capping
				var config = Config.Instance;
				if (Time.realtimeSinceStartup - _lastRewardedVideoShowTime < config.InterstitialRewardedVideoCappingTime)
				{
#if DEBUG_ENABLED
					var t = Time.realtimeSinceStartup - lastRewardedVideoShowTime;
					Debug.LogError("Rewarded video opened " + t + " seconds ago. Need to wait " +
						(config.InterstitialRewardedVideoCappingTime - t) + " seconds to show interstitial");
#endif
					return false;
				}
				if (Profile.Instance.PlayCount - _nPlaysUntilLastAd < config.GamesForInterstitial)
				{
					if (!_interstitialShown)
					{
						if (Time.realtimeSinceStartup - _lastInterstitialShowTime < config.FirstInterstitialCappingTime)
						{
#if DEBUG_ENABLED
							var t = Time.realtimeSinceStartup - lastRewardedVideoShowTime;
							Debug.LogError("Need wait " +
								(config.FirstInterstitialCappingTime - t) + " seconds to show interstitial");
#endif
							return false;
						}
					}
					else
					{
						if (Time.realtimeSinceStartup - _lastInterstitialShowTime < config.InterstitialCappingTime)
						{
#if DEBUG_ENABLED
							var t = Time.realtimeSinceStartup - lastRewardedVideoShowTime;
							Debug.LogError("Interstitial opened " + t + " seconds ago. Need to wait " +
								(config.InterstitialRewardedVideoCappingTime - t) + " seconds to show interstitial");
#endif
							return false;
						}
					}
				}
				return true;
			}
		}

		internal bool ShowInterstitial(Action onFinished = null)
		{
			if (!CanShowInterstitial)
			{
				onFinished?.Invoke();
				return false;
			}

			Analytics.Instance.LogEvent("ad_fs_requested");
			if (IronSource.Agent.isInterstitialReady())
			{
				_showingInterstitial = false;
				ShowReadyInterstitial(onFinished);
				return true;
			}
			else
			{
				switch (_interstitialState)
				{
					case InterstitialState.NotReady:
						Analytics.Instance.LogEvent("ad_fs_not_ready");
						break;
					case InterstitialState.LoadFailed:
						Analytics.Instance.LogEvent("ad_fs_load_failed");
						break;
					case InterstitialState.Available:
						Analytics.Instance.LogEvent("ad_fs_ready");
						break;
				}
				LoadInterstitial();
				onFinished?.Invoke();
				return false;
			}
		}

		private static void ShowMessage(string msg)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidJavaObject activity =
			new AndroidJavaClass("com.unity3d.player.UnityPlayer").
			GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaObject toastClass = new AndroidJavaClass("android.widget.Toast");
			toastClass.CallStatic<AndroidJavaObject>("makeText", activity, msg, toastClass.GetStatic<int>("LENGTH_SHORT")).Call("show");
#endif
		}

		private static void ShowRewardedVideoFailMessage()
		{
			ShowMessage(Application.internetReachability == NetworkReachability.NotReachable
				? "No internet connection. Try again"
				: "No video available at the moment. Try again later");
		}

		internal void ShowRewardedVideo(string place, Action<RewardedVideoState> callback)
		{
			_rewardedVideoTriggerPlace = place;
#if UNITY_EDITOR
			callback(RewardedVideoState.Watched);
#else
			if (_rewardedVideoCallback != null)
			{ // Previous rewarded video request is not finished yet
				callback(RewardedVideoState.Closed);
				return;
			}
			if (!RewardedVideoReady)
			{
				Analytics.Instance.LogEvent("ad_rw_not_available");
				callback(RewardedVideoState.NotReady);
				ShowRewardedVideoFailMessage();
				return;
			}
			Analytics.Instance.LogEvent("ad_rw_clicked", "place", place);
			Adjust.TrackEvent(Adjust.RwClicked);
			_rewardedVideoState = RewardedVideoState.Closed;
			_rewardedVideoCallback = callback;
			SceneManager.Instance.ShowLoading();
			if (IronSource.Agent.isRewardedVideoAvailable())
			{
				IronSource.Agent.showRewardedVideo();
			}
#endif
		}

		private void OnApplicationPause(bool isPaused)
		{
			IronSource.Agent.onApplicationPause(isPaused);
		}

		private void OnRewardedVideoFailed()
		{
			ShowRewardedVideoFailMessage();
			SceneManager.Instance.HideLoading();
			_rewardedVideoCallback?.Invoke(RewardedVideoState.Failed);
			_rewardedVideoCallback = null;
			ShowMessage("Video failed to show. Please retry");
		}

		private void OnRewardedVideoAdShowFailed(IronSourceError error)
		{
			Analytics.Instance.LogEvent("ad_rw_show_failed");
			OnRewardedVideoFailed();
			ShowMessage("Video failed to show. Please retry");
		}

		private void OnRewardedVideoAdOpened()
		{
			Analytics.Instance.LogEvent("ad_rw_shown", "place", _rewardedVideoTriggerPlace);
			Adjust.TrackEvent(Adjust.RwShown);
			SceneManager.Instance.HideLoading();
		}

		private void OnRewardedVideoAdClosed()
		{
			if (_rewardedVideoState == RewardedVideoState.Watched)
			{
				_lastRewardedVideoShowTime = Time.realtimeSinceStartup;
				Analytics.Instance.LogEvent("ad_rw_watched", "place", _rewardedVideoTriggerPlace);
				Adjust.TrackEvent(Adjust.RwWatched);
			}
			SceneManager.Instance.HideLoading();
			_rewardedVideoCallback?.Invoke(_rewardedVideoState);
			_rewardedVideoCallback = null;
		}

		private static void OnRewardedVideoAdStarted()
		{

		}

		private void OnRewardedVideoAdEnded()
		{
			_rewardedVideoState = RewardedVideoState.Watched;
		}

		private void OnRewardedVideoAdRewarded(IronSourcePlacement placement)
		{
			_rewardedVideoState = RewardedVideoState.Watched;
		}

		private static void OnRewardedVideoAdClicked(IronSourcePlacement placement)
		{

		}

		private static void OnRewardedVideoAvailabilityChanged(bool available)
		{

		}

		private Action _onIntersitialRequestProcessed;

		private void OnInterstitialAdReady()
		{
			_interstitialState = InterstitialState.Available;
			_requestingInterstitial = false;
			if (_showingInterstitial)
			{
				ShowReadyInterstitial(_onIntersitialRequestProcessed);
			}
		}

		private void OnInterstitialAdLoadFailed(IronSourceError error)
		{
			_interstitialState = InterstitialState.LoadFailed;
			RetryInterstitial();
		}

		private static void OnInterstitialAdOpened()
		{

		}

		private void OnInterstitialAdClosed()
		{
			LoadInterstitial();
		}

		private void OnInterstitialAdShowSucceeded()
		{
			_nPlaysUntilLastAd = Profile.Instance.PlayCount;
			Analytics.Instance.LogEvent("ad_fs_shown");
			Adjust.TrackEvent(Adjust.FsShown);
			_lastInterstitialShowTime = Time.realtimeSinceStartup;
			if (!_interstitialShown)
			{
				_interstitialShown = true;
			}
			_onIntersitialRequestProcessed?.Invoke();
		}

		private void OnInterstitialAdShowFailed(IronSourceError error)
		{
			Analytics.Instance.LogEvent("ad_fs_show_failed");
			RetryInterstitial();
			_onIntersitialRequestProcessed?.Invoke();
		}

		private void RetryInterstitial()
		{
			_nInterstitialAttempts++;
			if (_nInterstitialAttempts < MAXInterstitialAttempts)
			{
				IronSource.Agent.loadInterstitial();
			}
			else
			{
				_requestingInterstitial = false;
				_showingInterstitial = false;
			}
		}

		private static void OnInterstitialAdClicked()
		{
		}

		#region Banner

		private bool _isBannerReady;

		private bool CanShowBanner =>
			!Profile.Instance.Vip &&
			Config.Instance.BannerEnabled &&
			_state == State.Initialized &&
			_isBannerReady;

		private void LoadBanner()
		{
			if (!Config.Instance.BannerEnabled)
			{
				return;
			}
			if (_isBannerReady)
			{
				return;
			}
			IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
		}

		private void ShowBanner()
		{
			if(!BannerAllowed)
			{
				return;
			}
			if (!CanShowBanner)
			{
				return;
			}
			if (_isBannerReady)
			{
				try
				{
					// TODO: Show Banner shield
					IronSource.Agent.displayBanner();
				}
				catch
				{
					// ignored
				}

				Analytics.Instance.LogEvent("ad_bn_shown");
			}
			else
			{
				LoadBanner();
			}
		}

		internal void HideBanner()
		{
			try
			{
				// TODO: Hide Banner shield
				IronSource.Agent.hideBanner();
			}
			catch
			{
				// ignored
			}
		}

		private static void BannerAdClickedEvent()
		{
			Analytics.Instance.LogEvent("ad_bn_clicked");
		}

		private void BannerAdLoadFailedEvent(IronSourceError obj)
		{
			Analytics.Instance.LogEvent("ad_bn_load_failed");
			_isBannerReady = false;
		}

		private void BannerAdLoadedEvent()
		{
			Analytics.Instance.LogEvent("ad_bn_loaded");
			_isBannerReady = true;
			ShowBanner();
		}

		#endregion
	}
}