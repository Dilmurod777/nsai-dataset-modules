using System.Collections.Generic;
using Custom;
using Instances;
using UnityEngine;

public class AssetManager : Singleton<AssetManager>
{
	public Material inProgressMaterial;
	public Material tempMaterial;

	private readonly Vector3 _offset = new Vector3(-60.7f, -20.4f, 36.9f);

	public void UpdateAssets()
	{
		var task = ContextManager.Instance.CurrentTask;
		var subtask = ContextManager.Instance.CurrentSubtask;

		if (task != null && subtask != null)
		{
			if (subtask.Figure == null) return;

			var plainFigureName = Helpers.GetCurrentFigurePlainName();

			var figurePrefabInstallation = Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Scattered);
			var figurePrefabRemoval = Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Ifm);

			var ifmPrefab = Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Ifm);
			var rfmPrefab = Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Rfm);
			var referencePrefab = Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Reference);
			var scatteredPrefab = Resources.Load<GameObject>(Constants.ModelPrefabFolder + "/" + plainFigureName + "/" + Constants.FigureType.Scattered);

			if (referencePrefab != null)
			{
				var instantiatedReference = Instantiate(referencePrefab);
				instantiatedReference.transform.position = _offset + new Vector3(0, 0, 100f);
				instantiatedReference.tag = "ReferenceObject";
				instantiatedReference.name = plainFigureName + Constants.FigureType.Reference;
				instantiatedReference.transform.rotation = Quaternion.identity;
				instantiatedReference.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedReference.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (figurePrefabInstallation != null)
			{
				var instantiatedFigure = Instantiate(figurePrefabInstallation);
				instantiatedFigure.tag = "Figure";
				instantiatedFigure.name = plainFigureName + Constants.TaskType.Installation;
				instantiatedFigure.transform.rotation = Quaternion.identity;
				instantiatedFigure.transform.position = Vector3.zero;
				instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedFigure.GetComponentsInChildren<Transform>())
				{
					if (child.name == instantiatedFigure.name) continue;

					child.tag = "Object";
					var meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
					if (meshRenderer != null) meshRenderer.enabled = false;

					var referenceChild = Helpers.FindObjectInFigure(Constants.FigureType.Reference, child.name);

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
				instantiatedFigure.name = plainFigureName + Constants.TaskType.Removal;
				instantiatedFigure.transform.rotation = Quaternion.identity;
				instantiatedFigure.transform.position = Vector3.zero;
				instantiatedFigure.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var child in instantiatedFigure.GetComponentsInChildren<Transform>())
				{
					if (child.name == instantiatedFigure.name) continue;

					child.tag = "Object";
					var meshRenderer = child.gameObject.GetComponent<MeshRenderer>();
					if (meshRenderer != null) meshRenderer.enabled = false;


					var referenceChild = Helpers.FindObjectInFigure(Constants.FigureType.Reference, child.name);

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
				instantiatedIfm.name = plainFigureName + Constants.FigureType.Ifm;
				instantiatedIfm.transform.rotation = Quaternion.identity;
				instantiatedIfm.transform.position = Vector3.zero;
				instantiatedIfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedIfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (rfmPrefab != null)
			{
				var instantiatedRfm = Instantiate(rfmPrefab);
				instantiatedRfm.tag = "ReferenceObject";
				instantiatedRfm.name = plainFigureName + Constants.FigureType.Rfm;
				instantiatedRfm.transform.rotation = Quaternion.identity;
				instantiatedRfm.transform.position = Vector3.zero;
				instantiatedRfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedRfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (scatteredPrefab != null)
			{
				var instantiatedScattered = Instantiate(scatteredPrefab);
				instantiatedScattered.tag = "ReferenceObject";
				instantiatedScattered.name = plainFigureName + Constants.FigureType.Scattered;
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
		{
			ResetFigure(figure);
		}
	}

	public void ResetFigure(GameObject figure)
	{
		var plainFigureName = Helpers.GetCurrentFigurePlainName();
		var transformReferenceFigure = GameObject.Find(plainFigureName + Constants.FigureType.Reference)?.transform;

		if (transformReferenceFigure == null) return;

		figure.transform.position = Vector3.zero;
		figure.transform.rotation = transformReferenceFigure.rotation;
		figure.transform.localScale = transformReferenceFigure.localScale;

		foreach (var child in figure.GetComponentsInChildren<Transform>())
		{
			if (child.name == figure.name) continue;

			var objectMeta = child.GetComponent<ObjectMeta>();
			if (objectMeta && objectMeta.isCoreInFigure) continue;

			var outline = child.GetComponent<Outline>();
			if (outline != null) Destroy(outline);


			var referenceChild = Helpers.FindObjectInFigure(Constants.FigureType.Reference, child.name);
			var transformReference = Helpers
				.FindObjectInFigure(figure.name == plainFigureName + Constants.TaskType.Installation ? Constants.FigureType.Scattered : Constants.FigureType.Ifm, child.name)
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

	public void ShowCurrentFigure()
	{
		var currentTask = ContextManager.Instance.CurrentTask;
		var taskType = ContextManager.GetTaskType(currentTask);
		var plainFigureName = Helpers.GetCurrentFigurePlainName();

		var figuresInScene = GameObject.FindGameObjectsWithTag("Figure");
		foreach (var figureInScene in figuresInScene)
		{
			if (taskType == Constants.TaskType.Installation)
				if (figureInScene.name == plainFigureName + Constants.TaskType.Installation)
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

			if (taskType == Constants.TaskType.Removal)
				if (figureInScene.name == plainFigureName + Constants.TaskType.Removal)
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