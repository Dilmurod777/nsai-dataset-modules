using Custom;
using UnityEngine.SceneManagement;

public class SceneManager : Singleton<SceneManager>
{
	private const string StartMenuSceneName = "0_StartMenu";
	private const string MainSceneName = "1_Main";

	public static string StartMenuSceneNameGlobal = StartMenuSceneName;
	public static string MainSceneNameGlobal = MainSceneName;

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
				AssetManager.Instance.ShowFigure();
				UIManager.Instance.UpdateUI();
				AssetManager.Instance.UpdateAssets();
				break;
			case StartMenuSceneName:
				AssetManager.Instance.HideFigure();
				KnowledgeManager.Instance.ReadKnowledgeAndSetUp();
				KnowledgeManager.Instance.ReadQueriesAndSetUp();
				UIManager.Instance.SetUpMenus();
				break;
		}
	}
}