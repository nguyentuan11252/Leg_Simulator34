
using System.Collections.Generic;
using Firebase.Analytics;
using System;
using UnityEngine;

#if !UNITY_EDITOR
using GameAnalyticsSDK;
#endif

namespace Funzilla
{
	internal class FirebaseEvent
	{
		internal FirebaseEvent(string name, Parameter[] parameters)
		{
			Name = name;
			Parameters = parameters;
		}
		internal readonly string Name;
		internal readonly Parameter[] Parameters;
	}


	internal class Analytics : Singleton<Analytics>
	{
		private readonly Queue<FirebaseEvent> _firebaseEvents = new Queue<FirebaseEvent>(4);

		private void Awake()
		{
#if !UNITY_EDITOR
			GameAnalytics.Initialize();
#endif
		}

		internal void LogEvent(string eventName)
		{

#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.LogError("LogEvent: " + eventName);
#endif
			if (GameManager.FirebaseOk)
			{
				try
				{
					FirebaseAnalytics.LogEvent(eventName);
				}
				catch (Exception e)
				{
					Debug.LogError("Firebase analytics exception: " + e.ToString());
				}
			}
			else
			{
				_firebaseEvents.Enqueue(new FirebaseEvent(eventName, null));
			}
#if !UNITY_EDITOR
			GameAnalytics.NewDesignEvent(eventName);
#endif
		}

		internal void LogEvent(string eventName, string paramName, string paramValue)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.Log($"LogEvent: {eventName}, {paramName}={paramValue}");
#endif
			var firebaseParameters = new Parameter[] {
				new Parameter(paramName, paramValue),
			};
			if (GameManager.FirebaseOk)
			{
				try
				{
					FirebaseAnalytics.LogEvent(eventName, firebaseParameters);
				}
				catch (Exception e)
				{
					Debug.LogError("Firebase analytics exception: " + e.ToString());
				}
			}
			else
			{
				_firebaseEvents.Enqueue(new FirebaseEvent(eventName, firebaseParameters));
			}
#if !UNITY_EDITOR
			GameAnalytics.NewDesignEvent(eventName);
#endif
		}

		internal void LogEvent(string eventName, string param1Name, string param1Value, string param2Name, string param2Value)
		{
#if UNITY_EDITOR || DEBUG_ENABLED
			Debug.Log($"LogEvent: {eventName}, {param1Name}={param1Value}, {param2Name}={param2Value}");
#endif
			var firebaseParameters = new Parameter[] {
				new Parameter(param1Name, param1Value),
				new Parameter(param2Name, param2Value),
			};
			if (GameManager.FirebaseOk)
			{
				try
				{
					FirebaseAnalytics.LogEvent(eventName, firebaseParameters);
				}
				catch (Exception e)
				{
					Debug.LogError("Firebase analytics exception: " + e.ToString());
				}
			}
			else
			{
				_firebaseEvents.Enqueue(new FirebaseEvent(eventName, firebaseParameters));
			}
#if !UNITY_EDITOR
			GameAnalytics.NewDesignEvent(eventName);
#endif
		}

		private void Update()
		{
			if (!GameManager.FirebaseOk) return;
			while (_firebaseEvents.Count > 0)
			{
				var evt = _firebaseEvents.Dequeue();
				try
				{
					if (evt.Parameters == null)
					{
						FirebaseAnalytics.LogEvent(evt.Name);
					}
					else
					{
						FirebaseAnalytics.LogEvent(evt.Name, evt.Parameters);
					}
				}
				catch (Exception e)
				{
					Debug.LogError("Firebase analytics exception: " + e.ToString());
				}
			}

		}
	}
}