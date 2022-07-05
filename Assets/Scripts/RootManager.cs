using System;
using UnityEngine;

public class RootManager : MonoBehaviour
{
    public SceneManager sceneManager;
    public KnowledgeManager knowledgeManager;
    public UIManager uiManager;
    public ContextManager contextManager;
    public static Robot Robot;

    public static RootManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Robot = new Robot();

        knowledgeManager.ReadKnowledgeAndSetUp();
        knowledgeManager.ReadQueriesAndSetUp();
        uiManager.SetUpMenus(knowledgeManager.Tasks, knowledgeManager.Queries);
    }
}