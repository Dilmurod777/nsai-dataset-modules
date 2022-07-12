using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Custom
{
	public abstract class Singleton<T> : MonoBehaviour
	{
		private static readonly Dictionary<Type, object> Singletons = new Dictionary<Type, object>();

		public static T Instance => (T) Singletons[typeof(T)];

		private void Awake()
		{
			if (Singletons.ContainsKey(GetType()))
				Destroy(this);
			else
				Singletons.Add(GetType(), this);
		}
	}
}