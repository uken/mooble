using UnityEditor;
using UnityEngine;

namespace Mooble.EditorExtension {
  public class ConsoleEditorWindow : EditorWindow {
    [MenuItem("Mooble/Show Console")]
    public static void ShowWindow() {
    }

    private static void Init() {
      var window = EditorWindow.GetWindow(typeof(ConsoleEditorWindow), false, "Mooble") as ConsoleEditorWindow;
      window.Show();
    }

    private void OnGUI() {
    }

    private void DrawToolbar() {
      var toolbarStyle = EditorStyles.toolbarButton;
    }

    private bool DrawSelectedButton(string text, GUIStyle style, out Vector2 size) {
      var content = new GUIContent(text);
      size = style.CalcSize(content);
      return GUI.Button(new Rect(Vector2.zero, size), text, style);
    }
  }
}
