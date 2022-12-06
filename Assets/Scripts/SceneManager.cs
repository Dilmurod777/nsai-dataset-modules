using Custom;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : Singleton<SceneManager>
{
    private const string MenuSceneName = "0_Menu";
    private const string MainSceneName = "1_Main";

    public static string MenuSceneNameGlobal = MenuSceneName;
    public static string MainSceneNameGlobal = MainSceneName;

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
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
                UIManager.ResetBasicOperationsList();
                UIManager.Instance.UpdateUI();
                AssetManager.Instance.UpdateAssets();
                AssetManager.Instance.HideAllFigures();
                AssetManager.Instance.ShowCurrentFigure();

                var query = ContextManager.Instance.CurrentQuery;
                if (query == null) return;

                var queryText = GameObject.FindWithTag(Tags.QueryText);
                if (queryText != null)
                {
                    queryText.GetComponent<InputField>().text = query.Title;
                }

                QueryExecutor.IsTextInputUpdated = false;
                break;
        }
    }
}