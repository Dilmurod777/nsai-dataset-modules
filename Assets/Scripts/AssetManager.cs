using System;
using System.Collections.Generic;
using System.Linq;
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
			GameObject figurePrefabInstallation = null;
			GameObject figurePrefabRemoval = null;

			figurePrefabInstallation = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "-Scattered");
			figurePrefabRemoval = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "-IFM");

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

			if (figurePrefabInstallation != null)
			{
				var instantiatedFigure = Instantiate(figurePrefabInstallation);
				instantiatedFigure.tag = "Figure";
				instantiatedFigure.name = "CurrentFigureInstallation";
				instantiatedFigure.transform.rotation = Quaternion.identity;
				instantiatedFigure.transform.position = Vector3.zero;
				instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedFigure.GetComponentsInChildren<Transform>())
				{
					if (child.name == instantiatedFigure.name) continue;

					var meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
					if (meshRenderer != null)
					{
						meshRenderer.enabled = false;
					}

					var referenceChild = FindObjectInFigure(FigureType.Reference, child.name);

					if (referenceChild != null)
					{
						var childObjectMeta = child.GetComponent<ObjectMeta>();

						if (childObjectMeta == null)
						{
							var referenceObjectMeta = referenceChild.GetComponent<ObjectMeta>();
							if (referenceObjectMeta != null)
							{
								childObjectMeta = child.gameObject.AddComponent<ObjectMeta>();
								childObjectMeta.attachType = referenceObjectMeta.attachType;
								childObjectMeta.detachType = referenceObjectMeta.detachType;
								childObjectMeta.attachRotationAxis = referenceObjectMeta.attachRotationAxis;
								childObjectMeta.dettachRotationAxis = referenceObjectMeta.dettachRotationAxis;
								childObjectMeta.status = referenceObjectMeta.status;
								childObjectMeta.isCoreInFigure = referenceObjectMeta.isCoreInFigure;
							}
							else
							{
								Debug.LogError("Reference " + referenceChild.name + " object has no ObjectMeta script");
							}
						}

						var boxCollider = child.GetComponent<BoxCollider>();
						if (boxCollider == null)
						{
							boxCollider = child.gameObject.AddComponent<BoxCollider>();

							var referenceBoxCollider = referenceChild.GetComponent<BoxCollider>();

							if (referenceBoxCollider != null)
							{
								boxCollider.isTrigger = referenceBoxCollider.isTrigger;
								boxCollider.center = referenceBoxCollider.center;
								boxCollider.size = referenceBoxCollider.size;
							}
						}
					}
				}
			}

			if (figurePrefabRemoval != null)
			{
				var instantiatedFigure = Instantiate(figurePrefabRemoval);
				instantiatedFigure.tag = "Figure";
				instantiatedFigure.name = "CurrentFigureRemoval";
				instantiatedFigure.transform.rotation = Quaternion.identity;
				instantiatedFigure.transform.position = Vector3.zero;
				instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedFigure.GetComponentsInChildren<Transform>())
				{
					if (child.name == instantiatedFigure.name) continue;

					var meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
					if (meshRenderer != null)
					{
						meshRenderer.enabled = false;
					}


					var referenceChild = FindObjectInFigure(FigureType.Reference, child.name);

					if (referenceChild != null)
					{
						var childObjectMeta = child.GetComponent<ObjectMeta>();
						if (childObjectMeta == null)
						{
							var referenceObjectMeta = referenceChild.GetComponent<ObjectMeta>();
							if (referenceObjectMeta != null)
							{
								childObjectMeta = child.gameObject.AddComponent<ObjectMeta>();
								childObjectMeta.attachType = referenceObjectMeta.attachType;
								childObjectMeta.detachType = referenceObjectMeta.detachType;
								childObjectMeta.attachRotationAxis = referenceObjectMeta.attachRotationAxis;
								childObjectMeta.dettachRotationAxis = referenceObjectMeta.dettachRotationAxis;
								childObjectMeta.status = referenceObjectMeta.status;
								childObjectMeta.isCoreInFigure = referenceObjectMeta.isCoreInFigure;
							}
							else
							{
								Debug.LogError("Reference " + referenceChild.name + " object has no ObjectMeta script");
							}
						}

						var boxCollider = child.GetComponent<BoxCollider>();
						if (boxCollider == null)
						{
							boxCollider = child.gameObject.AddComponent<BoxCollider>();

							var referenceBoxCollider = referenceChild.GetComponent<BoxCollider>();

							if (referenceBoxCollider != null)
							{
								boxCollider.isTrigger = referenceBoxCollider.isTrigger;
								boxCollider.center = referenceBoxCollider.center;
								boxCollider.size = referenceBoxCollider.size;
							}
						}
					}
				}
			}

			if (ifmPrefab != null)
			{
				var instantiatedIfm = Instantiate(ifmPrefab);
				instantiatedIfm.tag = "ReferenceObject";
				instantiatedIfm.name = "CurrentFigureIFM";
				instantiatedIfm.transform.rotation = Quaternion.identity;
				instantiatedIfm.transform.position = Vector3.zero;
				instantiatedIfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedIfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (rfmPrefab != null)
			{
				var instantiatedRfm = Instantiate(rfmPrefab);
				instantiatedRfm.tag = "ReferenceObject";
				instantiatedRfm.name = "CurrentFigureRFM";
				instantiatedRfm.transform.rotation = Quaternion.identity;
				instantiatedRfm.transform.position = Vector3.zero;
				instantiatedRfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedRfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}
		}
	}

	public void ResetCurrentFigure()
	{
		var figure = GameObject.FindWithTag("Figure");
		var referenceFigure = GameObject.FindGameObjectsWithTag("ReferenceObject").First(obj => obj.name == "CurrentFigureReference");
		
		var task = ContextManager.Instance.CurrentTask;
		var subtask = ContextManager.Instance.CurrentSubtask;
		var taskType = ContextManager.GetTaskType(task);

		if (subtask != null)
		{
			GameObject figurePrefab;

			if (taskType == ContextManager.TaskType.Installation)
			{
				figurePrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.SubtaskId) + "-Scattered");
			}
			else
			{
				figurePrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.SubtaskId) + "-IFM");
			}

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
		var task = ContextManager.Instance.CurrentTask;
		var taskType = ContextManager.GetTaskType(task);

		var figure = type switch
		{
			FigureType.Current => taskType == ContextManager.TaskType.Installation
				? GameObject.Find("CurrentFigureInstallation")
				: GameObject.Find("CurrentFigureRemoval"),
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

	public void ShowCurrentFigure()
	{
		var currentTask = ContextManager.Instance.CurrentTask;
		var taskType = ContextManager.GetTaskType(currentTask);

		var figuresInScene = GameObject.FindGameObjectsWithTag("Figure");
		foreach (var figureInScene in figuresInScene)
		{
			if (taskType == ContextManager.TaskType.Installation)
			{
				if (figureInScene.name == "CurrentFigureInstallation")
				{
					for (var i = 0; i < figureInScene.transform.childCount; i++)
					{
						var objectMeta = figureInScene.transform.GetChild(i).gameObject.GetComponent<ObjectMeta>();

						if (objectMeta.isCoreInFigure)
						{
							CameraManager.Instance.FocusOnFigure(figureInScene.transform.GetChild(i).gameObject);
						}

						var meshRenderer = figureInScene.transform.GetChild(i).GetComponent<MeshRenderer>();
						if (meshRenderer != null)
						{
							meshRenderer.enabled = objectMeta.status == ObjectMeta.Status.Attached;
						}
					}

					break;
				}
			}

			if (taskType == ContextManager.TaskType.Removal)
			{
				if (figureInScene.name == "CurrentFigureRemoval")
				{
					for (var i = 0; i < figureInScene.transform.childCount; i++)
					{
						var objectMeta = figureInScene.transform.GetChild(i).gameObject.GetComponent<ObjectMeta>();

						if (objectMeta.isCoreInFigure)
						{
							CameraManager.Instance.FocusOnFigure(figureInScene.transform.GetChild(i).gameObject);
						}

						var meshRenderer = figureInScene.transform.GetChild(i).GetComponent<MeshRenderer>();
						if (meshRenderer)
						{
							meshRenderer.enabled = true;
						}
					}

					break;
				}
			}
		}
	}

	public void HideAllFigures()
	{
		var figuresInScene = GameObject.FindGameObjectsWithTag("Figure");
		foreach (var figureInScene in figuresInScene)
		{
			foreach (var meshRenderer in figureInScene.GetComponentsInChildren<MeshRenderer>())
				meshRenderer.enabled = false;
		}
	}
}