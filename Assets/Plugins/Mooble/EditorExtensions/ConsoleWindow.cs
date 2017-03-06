#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using Mooble.StaticAnalysis;
using UnityEditor;
using UnityEngine;

namespace Mooble.EditorExtensions {
  public class ConsoleWindow : EditorWindow {
    private const string ALL = "All";
    private const float SPACE_INCREMENT = 6.0f;

    private readonly Color selectedColor = Color.grey;
    private readonly Color deselectedColor = Color.white;

    private static ConsoleWindow instance;

    private Texture2D errorIcon;
    private Texture2D warningIcon;

    private Console console;

    private GUIStyle logEntryOdd;
    private GUIStyle logEntryEven;

    private bool showErrors = true;
    private bool showWarnings = true;

    private Dictionary<Rule, List<IViolation>> rulesAndTheirViolations;
    private List<ConsoleViolation> filteredList;
    private List<string> ruleNames;

    private float logListMaxWidth;
    private float logListLineHeight;
    private Vector2 logListScrollPosition;
    private int selectedLog = -1;
    private int popupIndex;

    public static ConsoleWindow Instance {
      get {
        if (instance == null) {
          Initialize();
        }

        return instance;
      }
    }

    [MenuItem("Mooble/Show Console")]
    public static void ShowWindow() {
      Initialize();
    }

    public void SetViolations(Dictionary<Rule, List<IViolation>> violations) {
      this.rulesAndTheirViolations = new Dictionary<Rule, List<IViolation>>();
      this.rulesAndTheirViolations = StaticAnalysis.StaticAnalysis.MergeRuleViolationDictionary(this.rulesAndTheirViolations, violations);
      this.Repaint();
    }

    private static void Initialize() {
      instance = EditorWindow.GetWindow(typeof(ConsoleWindow), false, "Mooble") as ConsoleWindow;
      instance.Show();
      EditorApplication.hierarchyWindowChanged += instance.Clear;
    }

    private void OnEnable() {
      if (!this.console) {
        this.console = Console.Create();
      }

      this.errorIcon = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
      this.warningIcon = EditorGUIUtility.FindTexture("d_console.warnicon.sml");
      this.Repaint();
    }

    private void OnGUI() {
      GUIStyle entryBackEven = GUI.skin.GetStyle("CN EntryBackEven");
      GUIStyle entryBackOdd = GUI.skin.GetStyle("CN EntryBackOdd");

      this.logEntryEven = new GUIStyle();
      this.logEntryEven.normal = entryBackEven.normal;
      this.logEntryEven.margin = new RectOffset(0, 0, 0, 0);
      this.logEntryEven.border = new RectOffset(0, 0, 0, 0);
      this.logEntryEven.fixedHeight = 0;

      this.logEntryOdd = new GUIStyle(this.logEntryEven);
      this.logEntryOdd.normal = entryBackOdd.normal;

      this.logListLineHeight = 0;
      this.logListMaxWidth = 0;

      filteredList = this.PopulateErrorsAndWarnings();
      this.SetupRuleDictAndNameList();
      this.DrawToolbar();
      this.DrawScrollRect();
      this.HandleInput();
    }

    private void SetupRuleDictAndNameList() {
      if (this.rulesAndTheirViolations == null) {
        this.rulesAndTheirViolations = new Dictionary<Rule, List<IViolation>>();
      }

      if (this.ruleNames == null) {
        this.ruleNames = new List<string> { ALL };
      }

      foreach (KeyValuePair<Rule, List<IViolation>> kvp in this.rulesAndTheirViolations) {
        if (kvp.Value.Count > 0) {
          this.ruleNames.Add(kvp.Key.Name);
        }
      }
    }

    private void HandleInput() {
      if (Event.current != null && Event.current.isKey && Event.current.type.ToString() == "KeyUp") {
        bool shouldRepaint = false;

        switch (Event.current.keyCode) {
          case KeyCode.UpArrow:
            if (this.selectedLog == 0) {
              return;
            }

            this.selectedLog--;
            shouldRepaint = true;
            break;
          case KeyCode.DownArrow:
            if (this.selectedLog == this.filteredList.Count - 1) {
              return;
            }

            this.selectedLog++;
            shouldRepaint = true;
            break;
        }

        if (shouldRepaint) {
          this.Repaint();
          this.UpdateSelection();
        }
      }
    }

    private List<ConsoleViolation> PopulateErrorsAndWarnings() {
      this.SetupRuleDictAndNameList();

      List<ConsoleViolation> violations = new List<ConsoleViolation>();

      List<ConsoleViolation> errors = this.PopulateErrors();
      List<ConsoleViolation> warnings = this.PopulateWarnings();

      if (errors != null) {
        violations.AddRange(errors);
      }

      if (warnings != null) {
        violations.AddRange(warnings);
      }

      return violations;
    }

    private List<ConsoleViolation> PopulateErrors() {
      if (!this.showErrors) {
        return null;
      }

      List<ConsoleViolation> errors = new List<ConsoleViolation>();

      foreach (KeyValuePair<Rule, List<IViolation>> kvp in this.rulesAndTheirViolations) {
        if (kvp.Key.Level == ViolationLevel.Error) {
          if (kvp.Value.Count == 0) {
            continue;
          }

          foreach (IViolation v in kvp.Value) {
            if (this.ruleNames[this.popupIndex] == kvp.Key.Name || this.ruleNames[this.popupIndex] == ALL) {
              errors.Add(new ConsoleViolation(kvp.Key.Level, v));
            }
          }
        }
      }

      this.console.ErrorCount = errors.Count;

      return errors;
    }

    private List<ConsoleViolation> PopulateWarnings() {
      if (!this.showWarnings) {
        return null;
      }

      List<ConsoleViolation> warnings = new List<ConsoleViolation>();

      foreach (KeyValuePair<Rule, List<IViolation>> kvp in this.rulesAndTheirViolations) {
        if (kvp.Key.Level == ViolationLevel.Warning) {
          if (kvp.Value.Count == 0) {
            continue;
          }

          foreach (IViolation v in kvp.Value) {
            if (this.ruleNames[this.popupIndex] == kvp.Key.Name || this.ruleNames[this.popupIndex] == ALL) {
              warnings.Add(new ConsoleViolation(kvp.Key.Level, v));
            }
          }
        }
      }

      this.console.WarningCount = warnings.Count;

      return warnings;
    }

    private void Clear() {
      this.console.ErrorCount = 0;
      this.console.WarningCount = 0;
      this.popupIndex = 0;
      this.filteredList.Clear();
      this.rulesAndTheirViolations = new Dictionary<Rule, List<IViolation>>();
      this.ruleNames = new List<string> { ALL };
    }

    private void DrawToolbar() {
      GUILayout.BeginHorizontal(EditorStyles.toolbar);

      this.DrawButton("Clear", this.Clear);

      this.InsertSpace();

      this.DrawButton("Analyze Scene", Mooble.EditorExtension.StaticAnalysisMenu.PrintSceneAnalysis);

      this.InsertSpace();

      this.DrawButton("Analyze All Prefabs", Mooble.EditorExtension.StaticAnalysisMenu.PrintPrefabAnalysis);

      GUILayout.FlexibleSpace();

      GUILayout.Label("Rule: ", EditorStyles.toolbarButton);
      this.popupIndex = EditorGUILayout.Popup(this.popupIndex, this.ruleNames.ToArray(), EditorStyles.toolbarPopup);

      this.InsertSpace();
      this.InsertSpace();

      this.showWarnings = this.DrawToggle(this.showWarnings, new GUIContent(this.console.WarningCount.ToString(), this.warningIcon));
      this.showErrors = this.DrawToggle(this.showErrors, new GUIContent(this.console.ErrorCount.ToString(), this.errorIcon));

      GUILayout.EndHorizontal();
    }

    private void DrawScrollRect() {
      if (this.filteredList.Count == 0) {
        return;
      }

      Color oldColor = GUI.backgroundColor;
      GUIStyle logLineStyle = this.logEntryEven;

      for (int k = 0; k < this.filteredList.Count; k++) {
        Vector2 size = logLineStyle.CalcSize(this.GUIContentForIViolation(this.filteredList[k]));
        this.logListMaxWidth = Mathf.Max(this.logListMaxWidth, size.x);
        this.logListLineHeight = Mathf.Max(this.logListLineHeight, size.y);
      }

      this.logListLineHeight *= 1.1f;

      Rect scrollRect = new Rect(Vector2.zero, new Vector2(this.position.width, this.position.height));
      Rect contentRect = new Rect(Vector2.zero, new Vector2(Mathf.Max(this.logListMaxWidth, scrollRect.width), this.filteredList.Count * this.logListLineHeight));
      this.logListScrollPosition = GUI.BeginScrollView(scrollRect, this.logListScrollPosition, contentRect);

      for (int i = 0; i < this.filteredList.Count; i++) {
        logLineStyle = i % 2 == 0 ? this.logEntryEven : this.logEntryOdd;
        GUI.backgroundColor = i == this.selectedLog ? this.selectedColor : this.deselectedColor;

        if (GUILayout.Button(this.GUIContentForIViolation(this.filteredList[i]), logLineStyle)) {
          if (i != this.selectedLog) {
            this.selectedLog = i;
          }

          this.UpdateSelection();
        }
      }

      GUI.EndScrollView();
      GUI.backgroundColor = oldColor;
    }

    private GUIContent GUIContentForIViolation(ConsoleViolation violation) {
      return new GUIContent(violation.Violation.FormatEditor(), violation.Level == ViolationLevel.Warning ? this.warningIcon : this.errorIcon);
    }

    private void UpdateSelection() {
      UnityEngine.Object obj = this.filteredList[this.selectedLog].Violation.GetObject();
      if (obj != null) {
        Selection.activeObject = obj;
      }
    }

    private void DrawButton(string text, Action action) {
      if (GUILayout.Button(text, EditorStyles.toolbarButton)) {
        if (action == null) {
          return;
        }

        action();
      }
    }

    private bool DrawToggle(bool selected, GUIContent content) {
      return GUILayout.Toggle(selected, content, EditorStyles.toolbarButton);
    }

    private void InsertSpace() {
      GUILayout.Space(SPACE_INCREMENT);
    }

    private struct ConsoleViolation {
      public readonly ViolationLevel Level;
      public readonly IViolation Violation;

      public ConsoleViolation(ViolationLevel level, IViolation violation) {
        this.Level = level;
        this.Violation = violation;
      }
    }
  }
}
#endif
