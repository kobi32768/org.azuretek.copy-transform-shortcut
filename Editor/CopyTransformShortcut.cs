using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace org.azuretek.copytransformshortcut
{
    [InitializeOnLoad]
    public class CopyTransformShortcut
    {
        private static Transform _transform;
        private static bool _enabled = true;

        [MenuItem("Azuretek/Copy Transform", false, 0)]
        static void ToggleActive()
        {
            _enabled = !_enabled;
            Menu.SetChecked("Azuretek/Copy Transform", _enabled);
        }

        static CopyTransformShortcut()
        {
            // initialize
            Menu.SetChecked("Azuretek/Copy Transform", _enabled);

            EditorApplication.CallbackFunction function = () =>
            {
                if (!_enabled) return;

                if (Event.current.type == EventType.KeyDown)
                {
                    Transform selectedTransform = Selection.activeTransform;

                    // Alt+Shift+C で選択しているオブジェクトの Transform をコピー
                    if (IsCopyPressed() && selectedTransform != null)
                    {
                        _transform = selectedTransform;
                        // Debug.Log($"Transform をコピーしました: {_transform.name}");
                    }

                    // Alt+Shift+V でコピーした Transform を選択しているオブジェクトにペースト
                    if (IsPastePressed() && selectedTransform != null)
                    {
                        Undo.RecordObject(selectedTransform, $"Paste Transform ({selectedTransform.name})");
                        if (_transform == null)
                        {
                            Debug.Log("Transform がコピーされていません");
                            return;
                        }

                        selectedTransform.position = _transform!.position;
                        selectedTransform.rotation = _transform!.rotation;
                        selectedTransform.localScale = _transform!.localScale;
                        Debug.Log($"Transform をペーストしました: {_transform.name} -> {selectedTransform.name}");
                    }
                }
            };

            FieldInfo info = typeof(EditorApplication).GetField("globalEventHandler",
                BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
            EditorApplication.CallbackFunction functions = (EditorApplication.CallbackFunction)info.GetValue(null);
            functions += function;
            info.SetValue(null, functions);
        }

        static bool IsCopyPressed()
        {
            return Event.current.alt && Event.current.shift && Event.current.keyCode == KeyCode.C;
        }

        static bool IsPastePressed()
        {
            return Event.current.alt && Event.current.shift && Event.current.keyCode == KeyCode.V;
        }
    }
}
