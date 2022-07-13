using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Custom;
using Instances;
using UnityEngine;

public class AssetManager : Singleton<AssetManager>
{
	public enum FigureType
	{
		Current,
		IFM,
		RFM,
		Reference
	}

	public Material inProgressMaterial;
	public Material tempMaterial;

	private Vector3 offset = new Vector3(-60.7f, -20.4f, 36.9f);

	private string GetPlainFigureName(string subtaskId)
	{
		var subtaskIdsToFigurePrefab = new Dictionary<string, string>
		{
			{"32-11-61", "MainLandingGear"}
		};

		string prefabName = "";

		foreach (var key in subtaskIdsToFigurePrefab.Keys)
		{
			if (subtaskId.StartsWith(key))
			{
				prefabName = subtaskIdsToFigurePrefab[key];
			}
		}

		var needsToReplace = new List<string> {"-IFM", "-RFM", "-Reference"};

		foreach (var word in needsToReplace) prefabName = prefabName.Replace(word, "");

		return prefabName;
	}


	public void UpdateAssets()
	{
		var task = ContextManager.Instance.CurrentTask;
		var subtask = ContextManager.Instance.CurrentSubtask;

		if (task != null && subtask != null)
		{
			if (subtask.Figure == null) return;

			var plainFigureName = GetPlainFigureName(subtask.SubtaskId);
			GameObject figurePrefab = null;
			if (task.Title.ToLower().Contains("installation"))
			{
				figurePrefab = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "-Scattered");
			}
			else if (task.Title.ToLower().Contains("removal"))
			{
				figurePrefab = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "-IFM");
			}

			var ifmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "-IFM");
			var rfmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "-RFM");
			var referencePrefab = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "-Reference");

			if (referencePrefab != null)
			{
				var instantiatedReference = Instantiate(referencePrefab);
				instantiatedReference.transform.position = offset + new Vector3(0, 0, 100f);
				instantiatedReference.tag = "ReferenceObject";
				instantiatedReference.name = "CurrentFigureReference";
				instantiatedReference.transform.rotation = Quaternion.identity;
				instantiatedReference.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedReference.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (figurePrefab != null)
			{
				var instantiatedFigure = Instantiate(figurePrefab);
				instantiatedFigure.transform.position = offset;
				instantiatedFigure.tag = "Figure";
				instantiatedFigure.name = "CurrentFigure";
				instantiatedFigure.transform.rotation = Quaternion.identity;
				instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedFigure.GetComponentsInChildren<Transform>())
				{
					if (child.name == instantiatedFigure.name) continue;

					if (child.GetComponent<ObjectMeta>() == null)
					{
						var referenceChild = FindObjectInFigure(FigureType.Reference, child.name);

						if (referenceChild != null)
						{
							var referenceObjectMeta = referenceChild.GetComponent<ObjectMeta>();
							if (referenceObjectMeta != null)
							{
								var childObjectMeta = child.gameObject.AddComponent<ObjectMeta>();
								childObjectMeta.attachType = referenceObjectMeta.attachType;
								childObjectMeta.detachType = referenceObjectMeta.detachType;
								childObjectMeta.attachRotationAxis = referenceObjectMeta.attachRotationAxis;
								childObjectMeta.dettachRotationAxis = referenceObjectMeta.dettachRotationAxis;
							}
							else
							{
								Debug.LogError("Reference " + referenceChild.name + " object has no ObjectMeta script");
							}
						}
					}

					if (child.GetComponent<BoxCollider>() == null)
					{
						child.gameObject.AddComponent<BoxCollider>();
					}

					if (ContextManager.Instance.latestGameObjectPositions.ContainsKey(child.name))
					{
						ContextManager.Instance.latestGameObjectPositions[child.name] = child.position;
					}
					else
					{
						ContextManager.Instance.latestGameObjectPositions.Add(child.name, child.position);
					}
				}
			}

			if (ifmPrefab != null)
			{
				var instantiatedIfm = Instantiate(ifmPrefab);
				instantiatedIfm.transform.position = offset + new Vector3(0, 0, 100f);
				instantiatedIfm.tag = "ReferenceObject";
				instantiatedIfm.name = "CurrentFigureIFM";
				instantiatedIfm.transform.rotation = Quaternion.identity;
				instantiatedIfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedIfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (rfmPrefab != null)
			{
				var instantiatedRfm = Instantiate(rfmPrefab);
				instantiatedRfm.transform.position = offset + new Vector3(0, 0, 100f);
				instantiatedRfm.tag = "ReferenceObject";
				instantiatedRfm.name = "CurrentFigureRFM";
				instantiatedRfm.transform.rotation = Quaternion.identity;
				instantiatedRfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedRfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}
		}
	}

	public void ResetCurrentFigure()
	{
		var figure = GameObject.FindWithTag("Figure");

		if (figure != null)
		{
			Destroy(figure);
		}

		var subtask = ContextManager.Instance.CurrentSubtask;

		if (subtask != null)
		{
			var figurePrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.SubtaskId) + "-Initial");

			if (figurePrefab != null)
			{
				var instantiatedFigure = Instantiate(figurePrefab);
				instantiatedFigure.transform.position = offset;
				instantiatedFigure.tag = "Figure";
				instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();
			}
		}
	}

	public GameObject FindObjectInFigure(FigureType type, string objName)
	{
		var subtaskId = ContextManager.Instance.CurrentSubtask.SubtaskId;
		var figure = type switch
		{
			FigureType.Current => GameObject.Find("CurrentFigure"),
			FigureType.IFM => GameObject.Find("CurrentFigureIFM"),
			FigureType.RFM => GameObject.Find("CurrentFigureRFM"),
			FigureType.Reference => GameObject.Find("CurrentFigureReference"),
			_ => GameObject.Find("CurrentFigure")
		};

		foreach (var child in figure.GetComponentsInChildren<Transform>())
			if (child.name.Contains(objName))
				return child.gameObject;

		return null;
	}

	public void ShowFigure()
	{
		var figureInScene = GameObject.FindWithTag("Figure");
		foreach (var meshRenderer in figureInScene.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = true;
	}

	public void HideFigure()
	{
		var figureInScene = GameObject.FindWithTag("Figure");
		if (figureInScene != null)
			foreach (var meshRenderer in figureInScene.GetComponentsInChildren<MeshRenderer>())
				meshRenderer.enabled = false;
	}
}