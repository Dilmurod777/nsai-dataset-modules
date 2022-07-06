using UnityEngine;

public class AssetManager : MonoBehaviour
{
	public void UpdateAssets()
	{
		DestroyAllFigures();

		var subtask = RootManager.Instance.contextManager.CurrentSubtask;

		if (subtask != null)
		{
			var figurePrefab = Resources.Load<GameObject>("ModelPrefabs/" + subtask.Figure);

			if (figurePrefab)
			{
				var instantiatedFigure = Instantiate(figurePrefab);
				instantiatedFigure.transform.position = new Vector3(8, 0, 20);
			}
		}
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