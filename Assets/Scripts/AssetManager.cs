using System.Collections.Generic;
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
		Reference,
		Scattered
	}

	public Material inProgressMaterial;
	public Material tempMaterial;

	private readonly Vector3 _offset = new Vector3(-60.7f, -20.4f, 36.9f);

	private string GetCurrentFigurePlainName()
	{
		var subtask = ContextManager.Instance.CurrentSubtask;
		var subtaskId = subtask.SubtaskId;

		var subtaskIdsToFigurePrefab = new Dictionary<string, string>
		{
			{"32-11-61", "MainLandingGear"},
			{"32-45-11", "WheelAndTire"}
		};

		var prefabName = "";

		foreach (var key in subtaskIdsToFigurePrefab.Keys)
			if (subtaskId.StartsWith(key))
				prefabName = subtaskIdsToFigurePrefab[key];

		var needsToReplace = new List<string> {"-IFM", "-RFM", "-Reference", "-Scattered"};

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

			var plainFigureName = GetCurrentFigurePlainName();

			var figurePrefabInstallation = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "/Scattered");
			var figurePrefabRemoval = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "/IFM");

			var ifmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "/IFM");
			var rfmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "/RFM");
			var referencePrefab = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "/Reference");
			var scatteredPrefab = Resources.Load<GameObject>("ModelPrefabs/" + plainFigureName + "/Scattered");

			if (referencePrefab != null)
			{
				var instantiatedReference = Instantiate(referencePrefab);
				instantiatedReference.transform.position = _offset + new Vector3(0, 0, 100f);
				instantiatedReference.tag = "ReferenceObject";
				instantiatedReference.name = plainFigureName + "-Reference";
				instantiatedReference.transform.rotation = Quaternion.identity;
				instantiatedReference.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedReference.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (figurePrefabInstallation != null)
			{
				var instantiatedFigure = Instantiate(figurePrefabInstallation);
				instantiatedFigure.tag = "Figure";
				instantiatedFigure.name = plainFigureName + "-Installation";
				instantiatedFigure.transform.rotation = Quaternion.identity;
				instantiatedFigure.transform.position = Vector3.zero;
				instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedFigure.GetComponentsInChildren<Transform>())
				{
					if (child.name == instantiatedFigure.name) continue;

					var meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
					if (meshRenderer != null) meshRenderer.enabled = false;

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
						}

						var boxCollider = child.GetComponent<BoxCollider>();
						if (boxCollider == null)
						{
							var referenceBoxCollider = referenceChild.GetComponent<BoxCollider>();

							if (referenceBoxCollider != null)
							{
								boxCollider = child.gameObject.AddComponent<BoxCollider>();
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
				instantiatedFigure.name = plainFigureName + "-Removal";
				instantiatedFigure.transform.rotation = Quaternion.identity;
				instantiatedFigure.transform.position = Vector3.zero;
				instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedFigure.GetComponentsInChildren<Transform>())
				{
					if (child.name == instantiatedFigure.name) continue;

					var meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
					if (meshRenderer != null) meshRenderer.enabled = false;


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
						}

						var boxCollider = child.GetComponent<BoxCollider>();
						if (boxCollider == null)
						{
							var referenceBoxCollider = referenceChild.GetComponent<BoxCollider>();

							if (referenceBoxCollider != null)
							{
								boxCollider = child.gameObject.AddComponent<BoxCollider>();
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
				instantiatedIfm.name = plainFigureName + "-IFM";
				instantiatedIfm.transform.rotation = Quaternion.identity;
				instantiatedIfm.transform.position = Vector3.zero;
				instantiatedIfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedIfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (rfmPrefab != null)
			{
				var instantiatedRfm = Instantiate(rfmPrefab);
				instantiatedRfm.tag = "ReferenceObject";
				instantiatedRfm.name = plainFigureName + "-RFM";
				instantiatedRfm.transform.rotation = Quaternion.identity;
				instantiatedRfm.transform.position = Vector3.zero;
				instantiatedRfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedRfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (scatteredPrefab != null)
			{
				var instantiatedScattered = Instantiate(scatteredPrefab);
				instantiatedScattered.tag = "ReferenceObject";
				instantiatedScattered.name = plainFigureName + "-Scattered";
				instantiatedScattered.transform.rotation = Quaternion.identity;
				instantiatedScattered.transform.position = Vector3.zero;
				instantiatedScattered.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedScattered.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}
		}
	}

	public void ResetFigures()
	{
		var figures = GameObject.FindGameObjectsWithTag("Figure");

		foreach (var figure in figures)
		foreach (var child in figure.GetComponentsInChildren<Transform>())
		{
			if (child.name == figure.name) continue;

			var objectMeta = child.GetComponent<ObjectMeta>();
			if (objectMeta && objectMeta.isCoreInFigure) continue;

			var plainFigureName = GetCurrentFigurePlainName();

			var referenceChild = Instance.FindObjectInFigure(FigureType.Reference, child.name);
			var transformReference = Instance.FindObjectInFigure(figure.name == plainFigureName + "-Installation" ? FigureType.Scattered : FigureType.IFM, child.name)
				.transform;

			child.position = transformReference.position;
			child.rotation = transformReference.rotation;
			child.localScale = transformReference.localScale;

			if (referenceChild != null)
			{
				var meshRenderer = referenceChild.GetComponent<MeshRenderer>();
				if (meshRenderer != null)
				{
					var materials = meshRenderer.materials;
					child.GetComponent<MeshRenderer>().materials = materials;
				}

				if (objectMeta)
				{
					var referenceObjectMeta = referenceChild.GetComponent<ObjectMeta>();

					objectMeta.attachType = referenceObjectMeta.attachType;
					objectMeta.detachType = referenceObjectMeta.detachType;
					objectMeta.attachRotationAxis = referenceObjectMeta.attachRotationAxis;
					objectMeta.dettachRotationAxis = referenceObjectMeta.dettachRotationAxis;
					objectMeta.status = referenceObjectMeta.status;
					objectMeta.isCoreInFigure = referenceObjectMeta.isCoreInFigure;
				}
			}
		}
	}

	public GameObject FindObjectInFigure(FigureType type, string objName)
	{
		var task = ContextManager.Instance.CurrentTask;
		var taskType = ContextManager.GetTaskType(task);

		var plainFigureName = GetCurrentFigurePlainName();

		var figure = type switch
		{
			FigureType.Current => taskType == ContextManager.TaskType.Installation
				? GameObject.Find(plainFigureName + "-Installation")
				: GameObject.Find(plainFigureName + "-Removal"),
			FigureType.IFM => GameObject.Find(plainFigureName + "-IFM"),
			FigureType.RFM => GameObject.Find(plainFigureName + "-RFM"),
			FigureType.Reference => GameObject.Find(plainFigureName + "-Reference"),
			FigureType.Scattered => GameObject.Find(plainFigureName + "-Scattered"),
			_ => GameObject.Find(plainFigureName + "-Installation")
		};

		foreach (var child in figure.GetComponentsInChildren<Transform>())
		{
			if (child.name.Contains(objName))
				return child.gameObject;
		}

		return null;
	}

	public void ShowCurrentFigure()
	{
		var currentTask = ContextManager.Instance.CurrentTask;
		var taskType = ContextManager.GetTaskType(currentTask);
		var plainFigureName = GetCurrentFigurePlainName();

		var figuresInScene = GameObject.FindGameObjectsWithTag("Figure");
		foreach (var figureInScene in figuresInScene)
		{
			if (taskType == ContextManager.TaskType.Installation)
				if (figureInScene.name == plainFigureName + "-Installation")
				{
					for (var i = 0; i < figureInScene.transform.childCount; i++)
					{
						var objectMeta = figureInScene.transform.GetChild(i).gameObject.GetComponent<ObjectMeta>();

						if (objectMeta.isCoreInFigure) CameraManager.Instance.FocusOnFigure(figureInScene.transform.GetChild(i).gameObject);

						var meshRenderer = figureInScene.transform.GetChild(i).GetComponent<MeshRenderer>();
						if (meshRenderer != null) meshRenderer.enabled = objectMeta.status == ObjectMeta.Status.Attached;
					}

					break;
				}

			if (taskType == ContextManager.TaskType.Removal)
				if (figureInScene.name == plainFigureName + "-Removal")
				{
					foreach (var child in figureInScene.GetComponentsInChildren<Transform>())
					{
						var objectMeta = child.gameObject.GetComponent<ObjectMeta>();

						if (objectMeta && objectMeta.isCoreInFigure) CameraManager.Instance.FocusOnFigure(child.gameObject);

						var meshRenderer = child.GetComponent<MeshRenderer>();
						if (meshRenderer) meshRenderer.enabled = true;
					}

					break;
				}
		}
	}

	public void HideAllFigures()
	{
		var figuresInScene = GameObject.FindGameObjectsWithTag("Figure");
		foreach (var figureInScene in figuresInScene)
		foreach (var meshRenderer in figureInScene.GetComponentsInChildren<MeshRenderer>())
			meshRenderer.enabled = false;
	}
}