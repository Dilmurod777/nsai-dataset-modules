using Custom;
using UnityEngine.SceneManagement;

public class SceneManager : Singleton<SceneManager>
{
	private const string MenuSceneName = "0_Menu";
	private const string MainSceneName = "1_Main";

	public static string MenuSceneNameGlobal = MenuSceneName;
	public static string MainSceneNameGlobal = MainSceneName;

	public void LoadScene(string sceneName)
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
	}

	private void OnEnable()
	{
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		switch (scene.name)
		{
			case MenuSceneName:
				AssetManager.Instance.HideAllFigures();
				KnowledgeManager.Instance.ReadKnowledgeAndSetUp();
				KnowledgeManager.Instance.ReadQueriesAndSetUp();
				UIManager.Instance.SetUpMenus();
				break;
			case MainSceneName:
				UIManager.Instance.UpdateUI();
				AssetManager.Instance.UpdateAssets();
				AssetManager.Instance.ShowCurrentFigure();
				break;
		}
	}
}