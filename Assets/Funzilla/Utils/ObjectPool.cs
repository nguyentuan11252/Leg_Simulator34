
using System.Collections.Generic;
using UnityEngine;

namespace Funzilla
{
	public class ObjectPool<T> where T : Object
	{
		Stack<T> inactive;
		T prefab;

		public ObjectPool(T prefab, int capacity)
		{
			Init(prefab, capacity);
		}

		void Init(T prefab, int capacity)
		{
			this.prefab = prefab;
			inactive = new Stack<T>(capacity);
		}

		public T Spawn()
		{
			if (inactive.Count > 0)
			{
				return inactive.Pop();
			}
			return Object.Instantiate<T>(prefab);
		}

		public void Despawn(T obj)
		{
			if (obj == null)
			{
				return;
			}
			inactive.Push(obj);
		}
	}
}