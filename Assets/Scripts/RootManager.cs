using UnityEngine;

public class RootManager: MonoBehaviour
{
    public SceneManager SceneManager;
    public KnowledgeManager knowledgeManager;
    public UIManager uiManager;
    public static Robot Robot;
    
    private void Start()
    {
        Robot = new Robot();

        knowledgeManager.ReadKnowledgeAndSetUp();
        knowledgeManager.ReadQueriesAndSetUp();
        uiManager.SetUpMenus(knowledgeManager.Tasks, knowledgeManager.Queries);
    }
}