#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using Mooble.StaticAnalysis;
using UnityEditor;
using UnityEngine;

namespace Mooble.EditorExtensions {
  public class ConsoleWindow : EditorWindow {
    private const string ALL = "All";
    private static ConsoleWindow instance;

    private Vector2 drawPosition;
    private Texture2D errorIcon;
    private Texture2D warningIcon;

    private Console console;

    private GUIStyle logEntryOdd;
    private GUIStyle logEntryEven;

    private Color lineColour;

    private bool showErrors = true;
    private bool showWarnings = true;

    private Dictionary<Rule, List<IViolation>> rulesAndTheirViolations;
    private List<string> ruleNames;

    private float logListMaxWidth;
    private float logListLineHeight;
    private Vector2 logListScrollPosition;
    private int selectedLog = -1;
    private int popupIndex;
    private string longestRuleName;

    public static ConsoleWindow Instance {
      get {
        if (instance == null) {
          Init();
        }

        return instance;
      }
    }

    [MenuItem("Mooble/Show Console")]
    public static void ShowWindow() {
      Init();
    }

    public void SetViolations(Dictionary<Rule, List<IViolation>> violations) {
      this.rulesAndTheirViolations = new Dictionary<Rule, List<IViolation>>();
      this.rulesAndTheirViolations = StaticAnalysis.StaticAnalysis.MergeRuleViolationDictionary(this.rulesAndTheirViolations, violations);
      EditorUtility.SetDirty(this);
    }

    private static void Init() {
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
      GUIStyle unityLogLineEven = null;
      GUIStyle unityLogLineOdd = null;
      GUIStyle unitySmallLogLine = null;

      foreach (var style in GUI.skin.customStyles) {
        switch (style.name) {
          case "CN EntryBackEven": unityLogLineEven = style; break;
          case "CN EntryBackOdd": unityLogLineOdd = style; break;
          case "CN StatusInfo": unitySmallLogLine = style; break;
        }
      }

      this.logEntryEven = new GUIStyle(unitySmallLogLine);
      this.logEntryEven.normal = unityLogLineEven.normal;
      this.logEntryEven.margin = new RectOffset(0, 0, 0, 0);
      this.logEntryEven.border = new RectOffset(0, 0, 0, 0);
      this.logEntryEven.fixedHeight = 0;

      this.logEntryOdd = new GUIStyle(this.logEntryEven);
      this.logEntryOdd.normal = unityLogLineOdd.normal;

      this.logListLineHeight = 0;
      this.logListMaxWidth = 0;

      this.drawPosition = Vector2.zero;

      List<ConsoleViolation> toDisplay = this.PopulateErrorsAndWarnings();
      this.DrawToolbar();
      this.DrawScrollRect(this.drawPosition.y, toDisplay);
    }

    private void SetupRuleDictAndNameList() {
      if (this.rulesAndTheirViolations == null) {
        this.rulesAndTheirViolations = new Dictionary<Rule, List<IViolation>>();
      }

      if (this.ruleNames == null) {
        this.ruleNames = new List<string> { ALL };
        this.longestRuleName = ALL;

        foreach (var kvp in this.rulesAndTheirViolations) {
          if (kvp.Value.Count > 0) {
            this.ruleNames.Add(kvp.Key.Name);
            this.longestRuleName = this.longestRuleName.Length > kvp.Key.Name.Length ? this.longestRuleName : kvp.Key.Name;
          }
        }
      }
    }

    private List<ConsoleViolation> PopulateErrorsAndWarnings() {
      this.SetupRuleDictAndNameList();

      var toDisplay = new List<ConsoleViolation>();

      if (this.showErrors) {
        foreach (var kvp in this.rulesAndTheirViolations) {
          if (kvp.Key.Level == ViolationLevel.Error) {
            if (kvp.Value.Count == 0) {
              continue;
            }

            foreach (var v in kvp.Value) {
              if (this.ruleNames[this.popupIndex] == kvp.Key.Name || this.ruleNames[this.popupIndex] == ALL) {
                toDisplay.Add(new ConsoleViolation(kvp.Key.Level, v));
              }
            }
          }
        }

        this.console.ErrorCount = toDisplay.Count;
      }

      if (this.showWarnings) {
        foreach (var kvp in this.rulesAndTheirViolations) {
          if (kvp.Key.Level == ViolationLevel.Warning) {
            if (kvp.Value.Count == 0) {
              continue;
            }

            foreach (var v in kvp.Value) {
              if (this.ruleNames[this.popupIndex] == kvp.Key.Name || this.ruleNames[this.popupIndex] == ALL) {
                toDisplay.Add(new ConsoleViolation(kvp.Key.Level, v));
              }
            }
          }
        }

        this.console.WarningCount = toDisplay.Count - this.console.ErrorCount;
      }

      return toDisplay;
    }

    private void Clear() {
      this.console.ErrorCount = 0;
      this.console.WarningCount = 0;
      this.popupIndex = 0;
      this.rulesAndTheirViolations = null;
      this.ruleNames = null;
    }

    private void DrawToolbar() {
      this.SetupRuleDictAndNameList();

      var toolbarStyle = EditorStyles.toolbarButton;

      this.drawPosition.x = position.width * 0.01f;

      Vector2 sizeOfElement;
      if (this.DrawButton("Clear", EditorStyles.toolbarButton, out sizeOfElement)) {
        this.Clear();
      }

      this.drawPosition.x = position.width * 0.25f;

      this.DrawLabel("Rule: ", EditorStyles.toolbarButton, out sizeOfElement);
      this.drawPosition.x += sizeOfElement.x;

      this.popupIndex = EditorGUI.Popup(
        new Rect(
          this.drawPosition,
          toolbarStyle.CalcSize(new GUIContent(this.longestRuleName))),
        this.popupIndex,
        this.ruleNames.ToArray(),
        EditorStyles.toolbarPopup);

      var errorToggleContent = new GUIContent(this.console.ErrorCount.ToString(), this.errorIcon);
      var warningToggleContent = new GUIContent(this.console.WarningCount.ToString(), this.warningIcon);

      float totalErrorButtonWidth =
        toolbarStyle.CalcSize(errorToggleContent).x +
        toolbarStyle.CalcSize(warningToggleContent).x;

      float errorIconX = position.width - totalErrorButtonWidth;

      if (errorIconX > this.drawPosition.x) {
        this.drawPosition.x = errorIconX;
      }

      this.showErrors = this.DrawToggleButton(this.showErrors, errorToggleContent, toolbarStyle, out sizeOfElement);
      this.drawPosition.x += sizeOfElement.x;

      this.showWarnings = this.DrawToggleButton(this.showWarnings, warningToggleContent, toolbarStyle, out sizeOfElement);
      this.drawPosition.x += sizeOfElement.x;

      this.drawPosition.y += sizeOfElement.y;

      this.drawPosition.x = 0;
    }

    private void DrawScrollRect(float height, IList<ConsoleViolation> filteredList) {
      if (filteredList.Count == 0) {
        return;
      }

      var oldColor = GUI.backgroundColor;
      var logLineStyle = this.logEntryEven;
      var logLineSize = logLineStyle.CalcSize(new GUIContent("A"));

      for (var k = 0; k < filteredList.Count; k++) {
        var size = logLineStyle.CalcSize(this.GUIContentForIViolation(filteredList[k]));
        this.logListMaxWidth = Mathf.Max(this.logListMaxWidth, size.x);
        this.logListLineHeight = Mathf.Max(this.logListLineHeight, size.y);
      }

      this.logListLineHeight *= 1.1f;

      var scrollRect = new Rect(this.drawPosition, new Vector2(position.width, position.height));
      var contentRect = new Rect(0, 0, Mathf.Max(this.logListMaxWidth, scrollRect.width), this.rulesAndTheirViolations.Count * this.logListLineHeight);
      this.logListScrollPosition = GUI.BeginScrollView(scrollRect, this.logListScrollPosition, contentRect);

      for (var i = 0; i < filteredList.Count; i++) {
        logLineStyle = i % 2 == 0 ? this.logEntryEven : this.logEntryOdd;
        GUI.backgroundColor = i == this.selectedLog ? new Color(0.5f, 0.5f, 1) : Color.white;

        var rect = new Rect(0, this.logListLineHeight * i, contentRect.width, this.logListLineHeight);
        if (GUI.Button(rect, this.GUIContentForIViolation(filteredList[i]), logLineStyle)) {
          if (i != this.selectedLog) {
            this.selectedLog = i;
          }

          var go = filteredList[i].Violation.GetObject();
          if (go != null) {
            Selection.activeObject = go;
          }
        }
      }

      GUI.EndScrollView();
      this.drawPosition.y += height;
      this.drawPosition.x = 0;
      GUI.backgroundColor = oldColor;
    }

    private GUIContent GUIContentForIViolation(ConsoleViolation violation) {
      return new GUIContent(violation.Violation.Format(), violation.Level == ViolationLevel.Warning ? this.warningIcon : this.errorIcon);
    }

    private bool DrawButton(string text, GUIStyle style, out Vector2 size) {
      var content = new GUIContent(text);
      size = style.CalcSize(content);
      return GUI.Button(new Rect(this.drawPosition, size), text, style);
    }

    private bool DrawToggleButton(bool selected, GUIContent content, GUIStyle style, out Vector2 size) {
      size = style.CalcSize(content);
      return GUI.Toggle(new Rect(this.drawPosition, size), selected, content, style);
    }

    private void DrawLabel(string text, GUIStyle style, out Vector2 size) {
      var content = new GUIContent(text);
      size = style.CalcSize(content);
      GUI.Label(new Rect(this.drawPosition, size), text, style);
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
