using System;
using UnityEngine;

namespace Custom
{
	public class CustomDontDestroyOnLoad: MonoBehaviour
	{
		private string uniqueID;

		private void Awake()
		{
			uniqueID = name + transform.position;
		}

		private void Start()
		{
			var dontDestroyOnLoadObjects = FindObjectsOfType<CustomDontDestroyOnLoad>();

			for (int i = 0; i < dontDestroyOnLoadObjects.Length; i++)
			{
				if (dontDestroyOnLoadObjects[i] != this && dontDestroyOnLoadObjects[i].uniqueID == uniqueID)
				{
					Destroy(gameObject);
				}
			}
			
			DontDestroyOnLoad(gameObject);
		}
	}
}