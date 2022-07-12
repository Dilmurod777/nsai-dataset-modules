using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Custom;
using UnityEngine;

public class AssetManager : Singleton<AssetManager>
{
	public enum FigureType
	{
		Current,
		IFM,
		RFM,
		Scattered
	}

	public Material inProgressMaterial;
	public Material tempMaterial;
	
	private Vector3 offset = new Vector3(-60.7f, -20.4f, 36.9f);

	private string GetPlainFigureName(string subtaskId)
	{
		var figureNameToPrefabMatch = new Dictionary<string, string>
		{
			{"32", "MainLandingGear"}
		};

		var id = subtaskId.Split('-')[0];
		var prefabName = figureNameToPrefabMatch[id];

		var needsToReplace = new List<string> {"-IFM", "-RFM", "-Scattered"};

		foreach (var word in needsToReplace) prefabName = prefabName.Replace(word, "");

		return prefabName;
	}


	public void UpdateAssets()
	{
		var subtask = ContextManager.Instance.CurrentSubtask;

		if (subtask != null)
		{
			if (subtask.Figure == null) return;
			
			var figurePrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.SubtaskId) + "-Initial");
			var ifmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.SubtaskId) + "-IFM");
			var rfmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.SubtaskId) + "-RFM");
			var scatteredPrefab = Resources.Load<GameObject>("ModelPrefabs/" + GetPlainFigureName(subtask.SubtaskId) + "-Scattered");

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

				foreach (var meshRenderer in instantiatedIfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (rfmPrefab != null)
			{
				var instantiatedRfm = Instantiate(rfmPrefab);
				instantiatedRfm.transform.position = offset + new Vector3(0, 0, 100f);
				instantiatedRfm.tag = "ReferenceObject";
				instantiatedRfm.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedRfm.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
			}

			if (scatteredPrefab != null)
			{
				var instantiatedScattered = Instantiate(scatteredPrefab);
				instantiatedScattered.transform.position = offset + new Vector3(0, 0, 100f);
				instantiatedScattered.tag = "ReferenceObject";
				instantiatedScattered.AddComponent<CustomDontDestroyOnLoad>();

				foreach (var meshRenderer in instantiatedScattered.GetComponentsInChildren<MeshRenderer>()) meshRenderer.enabled = false;
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
			FigureType.Current => GameObject.Find(GetPlainFigureName(subtaskId) + "-Initial(Clone)"),
			FigureType.IFM => GameObject.Find(GetPlainFigureName(subtaskId) + "-IFM(Clone)"),
			FigureType.RFM => GameObject.Find(GetPlainFigureName(subtaskId) + "-RFM(Clone)"),
			FigureType.Scattered => GameObject.Find(GetPlainFigureName(subtaskId) + "-Scattered(Clone)"),
			_ => GameObject.Find(subtaskId + "(Clone)")
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