using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class UIText : MonoBehaviour
    {
        private InputField _inputFieldComponent;

        private void Awake()
        {
            _inputFieldComponent = GetComponent<InputField>();

            if (_inputFieldComponent == null) return;

            switch (tag)
            {
                case Tags.QueryText:
                    _inputFieldComponent.onValueChanged.AddListener(QueryExecutor.Instance.UpdateQueryText);
                    break;
            }
        }
    }
}