
using System;
using UnityEngine;

namespace Funzilla
{
	[Serializable]
	public class StringList : MonoBehaviour
	{
		public int Count { get { return strings.Length; } }
		public string this[int index]
		{
			get
			{
				return strings[index];
			}
		}

		public string[] strings;

		public bool Contains(string name)
		{
			foreach (var s in strings)
			{
				if (s.Equals(name))
				{
					return true;
				}
			}
			return false;
		}
	}
}