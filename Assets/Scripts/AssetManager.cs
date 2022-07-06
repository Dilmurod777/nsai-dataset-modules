using System;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
	public enum FigureType
	{
		Current,
		IFM,
		RFM
	}

	public void UpdateAssets()
	{
		DestroyAllFigures();

		var subtask = RootManager.Instance.contextManager.CurrentSubtask;

		if (subtask != null)
		{
			var figurePrefab = Resources.Load<GameObject>("ModelPrefabs/" + subtask.Figure);
			var ifmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + subtask.Figure + "-IFM");
			var rfmPrefab = Resources.Load<GameObject>("ModelPrefabs/" + subtask.Figure + "-RFM");

			if (figurePrefab)
			{
				var instantiatedFigure = Instantiate(figurePrefab);
				instantiatedFigure.transform.position = new Vector3(4.5f, 0, 20);
			}
			
			if (ifmPrefab)
			{
				var instantiatedIfm = Instantiate(ifmPrefab);
				instantiatedIfm.transform.position = new Vector3(4.5f, 0, 120);

				foreach (var child in instantiatedIfm.GetComponentsInChildren<Transform>())
				{
					var meshRenderer = child.GetComponent<MeshRenderer>();
					if (meshRenderer != null)
					{
						meshRenderer.enabled = false;
					}
				}
			}
			
			if (rfmPrefab)
			{
				var instantiatedRfm = Instantiate(rfmPrefab);
				instantiatedRfm.transform.position = new Vector3(4.5f, 0, 120);

				foreach (var child in instantiatedRfm.GetComponentsInChildren<Transform>())
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
			FigureType.Current => GameObject.Find(figureName + "(Clone)"),
			FigureType.IFM => GameObject.Find(figureName + "-IFM(Clone)"),
			FigureType.RFM => GameObject.Find(figureName + "-RFM(Clone)"),
			_ => GameObject.Find(figureName)
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
	}
}