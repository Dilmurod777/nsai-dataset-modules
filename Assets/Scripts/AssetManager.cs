using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Custom;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
	public enum FigureType
	{
		Current,
		IFM,
		RFM,
		Scattered
	}

	private Vector3 offset = new Vector3(-60.7f, -20.4f, 36.9f);

	private string GetPlainFigureName(string figureName)
	{
		var figureNameToPrefabMatch = new Dictionary<string, string>
		{
			{"32", "MainLandingGear"}
		};

		var id = figureName.Split('-')[1];
		var prefabName = figureNameToPrefabMatch[id];

		var needsToReplace = new List<string> {"-IFM", "-RFM", "-Scattered"};

		foreach (var word in needsToReplace)
		{
			prefabName = prefabName.Replace(word, "");
		}

		return prefabName;
	}


	public void UpdateAssets()
	{
		// DestroyAllFigures();

		var subtask = RootManager.Instance.contextManager.CurrentSubtask;

		if (subtask != null)
		{
			var figurePrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.Figure) + "-Initial");
			var ifmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.Figure) + "-IFM");
			var rfmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.Figure) + "-RFM");
			var scatteredPrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.Figure) + "-Scattered");

			if (figurePrefab != null)
			{
				var instantiatedFigure = Instantiate(figurePrefab);
				instantiatedFigure.transform.position = offset;
				instantiatedFigure.tag = "Figure";
				instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();
			}

			if (ifmPrefab != null)
			{
				var instantiatedIfm = Instantiate(ifmPrefab);
				instantiatedIfm.transform.position = offset + new Vector3(0, 0, 100f);
				instantiatedIfm.tag = "ReferenceObject";
				instantiatedIfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedIfm.GetComponentsInChildren<Transform>())
				{
					var meshRenderer = child.GetComponent<MeshRenderer>();
					if (meshRenderer != null)
					{
						meshRenderer.enabled = false;
					}
				}
			}

			if (rfmPrefab != null)
			{
				var instantiatedRfm = Instantiate(rfmPrefab);
				instantiatedRfm.transform.position = offset + new Vector3(0, 0, 100f);
				instantiatedRfm.tag = "ReferenceObject";
				instantiatedRfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedRfm.GetComponentsInChildren<Transform>())
				{
					var meshRenderer = child.GetComponent<MeshRenderer>();
					if (meshRenderer != null)
					{
						meshRenderer.enabled = false;
					}
				}
			}

			if (scatteredPrefab != null)
			{
				var instantiatedScattered = Instantiate(scatteredPrefab);
				instantiatedScattered.transform.position = offset + new Vector3(0, 0, 100f);
				instantiatedScattered.tag = "ReferenceObject";
				instantiatedScattered.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedScattered.GetComponentsInChildren<Transform>())
				{
					var meshRenderer = child.GetComponent<MeshRenderer>();
					if (meshRenderer != null)
					{
						meshRenderer.enabled = false;
					}
				}
			}
		}
	}

	public GameObject FindObjectInFigure(FigureType type, string objName)
	{
		var figureName = RootManager.Instance.contextManager.CurrentSubtask.Figure;
		var figure = type switch
		{
			FigureType.Current => GameObject.Find(GetPlainFigureName(figureName) + "-Initial(Clone)"),
			FigureType.IFM => GameObject.Find(GetPlainFigureName(figureName) + "-IFM(Clone)"),
			FigureType.RFM => GameObject.Find(GetPlainFigureName(figureName) + "-RFM(Clone)"),
			FigureType.Scattered => GameObject.Find(GetPlainFigureName(figureName) + "-Scattered(Clone)"),
			_ => GameObject.Find(figureName + "(Clone)")
		};

		foreach (var child in figure.GetComponentsInChildren<Transform>())
		{
			if (child.name.Contains(objName))
			{
				return child.gameObject;
			}
		}

		return null;
	}

	public void DestroyAllFigures()
	{
		var figuresInScene = GameObject.FindGameObjectsWithTag("Figure");
		foreach (var figureInScene in figuresInScene)
		{
			Destroy(figureInScene);
		}

		var referenceObjectsInScene = GameObject.FindGameObjectsWithTag("ReferenceObject");
		foreach (var obj in referenceObjectsInScene)
		{
			Destroy(obj);
		}
	}
}