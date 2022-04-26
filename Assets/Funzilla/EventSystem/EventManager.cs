
using System;
using System.Collections.Generic;
using System.Linq;

namespace Funzilla
{
	class EventManager : Singleton<EventManager>
	{
		class Event
		{
			public Event(EventType type, object data)
			{
				this.type = type;
				this.data = data;
			}
			public EventType type;
			public object data;
		}

		List<Action<object>> actions = new List<Action<object>>(Enum.GetNames(typeof(EventType)).Length);
		Queue<Event> events = new Queue<Event>();

		private void Awake()
		{
			for (int i = 0; i < Enum.GetNames(typeof(EventType)).Length; i++)
			{
				actions.Add(null);
			}
		}

		public void Subscribe(EventType type, Action<object> action)
		{
			if (actions[(int)type] == null)
			{
				actions[(int)type] = action;
			}
			else if (!actions[(int)type].GetInvocationList().Contains(action))
			{
				actions[(int)type] += action;
			}
		}

		public void Unsubscribe(EventType type, Action<object> action)
		{
			if (actions[(int)type] != null)
			{
				actions[(int)type] -= action;
			}
		}

		public void Annouce(EventType type, object data = null)
		{
			events.Enqueue(new Event(type,data));
		}

		void Dispatch()
		{
			if (events == null || events.Count <= 0)
			{
				return;
			}
			var e = events.Dequeue();
			actions[(int)e.type]?.Invoke(e.data);
		}

		private void Update()
		{
			Dispatch();
		}
	}
}