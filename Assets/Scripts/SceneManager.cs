using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
	public GameObject managers;

	public const string StartMenuSceneName = "0_StartMenu";
	public const string MainSceneName = "1_Main";

	private void Awake()
	{
		DontDestroyOnLoad(managers);
	}

	public void LoadScene(string sceneName)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
	}

	private void OnEnable()
	{
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
	}

	static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		switch (scene.name)
		{
			case MainSceneName:
				RootManager.Instance.uiManager.UpdateUI();
				RootManager.Instance.assetManager.UpdateAssets();
				break;
			case StartMenuSceneName:
				RootManager.Instance.knowledgeManager.ReadKnowledgeAndSetUp();
				RootManager.Instance.knowledgeManager.ReadQueriesAndSetUp();
				RootManager.Instance.uiManager.SetUpMenus();
				break;
		}
	}
}