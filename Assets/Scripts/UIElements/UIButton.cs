using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class UIButton : MonoBehaviour
    {
        private Button _buttonComponent;

        private void Awake()
        {
            _buttonComponent = GetComponent<Button>();

            if (_buttonComponent == null) return;

            switch (tag)
            {
                case Tags.HomeButton:
                    _buttonComponent.onClick.RemoveAllListeners();
                    _buttonComponent.onClick.AddListener(() => { SceneManager.Instance.LoadScene(SceneManager.MenuSceneNameGlobal); });
                    break;
                case Tags.PlayButton:
                    _buttonComponent.onClick.RemoveAllListeners();
                    _buttonComponent.onClick.AddListener(() => { KnowledgeManager.Instance.ExecuteSubtask(); });
                    break;
                case Tags.NextButton:
                    _buttonComponent.onClick.RemoveAllListeners();
                    _buttonComponent.onClick.AddListener(() =>
                    {
                        KnowledgeManager.GoToNextSubtask();
                        UIManager.Instance.UpdateUI();
                    });
                    break;
                case Tags.PreviousButton:
                    _buttonComponent.onClick.RemoveAllListeners();
                    _buttonComponent.onClick.AddListener(() =>
                    {
                        KnowledgeManager.GoToPreviousSubtask();
                        UIManager.Instance.UpdateUI();
                    });
                    break;
                case Tags.ResetButton:
                    _buttonComponent.onClick.RemoveAllListeners();
                    _buttonComponent.onClick.AddListener(() =>
                    {
                        ContextManager.Instance.CompletedActions.Clear();
                        AssetManager.Instance.ResetFigures();
                        KnowledgeManager.Instance.ResetTasks();
                        SceneManager.Instance.LoadScene(SceneManager.MenuSceneNameGlobal);
                    });
                    break;
                case Tags.QueryPlayButton:
                    _buttonComponent.onClick.RemoveAllListeners();
                    _buttonComponent.onClick.AddListener(() => { StartCoroutine(QueryExecutor.Instance.ExecuteQuery()); });
                    break;
            }
        }
    }
}