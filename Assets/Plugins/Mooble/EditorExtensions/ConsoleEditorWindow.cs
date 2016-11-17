using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using Mooble.StaticAnalysis;

namespace Mooble.EditorExtensions {
  [System.Serializable]
  public class ConsoleEditorLogger : ScriptableObject {
    public int NoErrors { get; set; }

    public int NoWarnings { get; set; }

    public Dictionary<string, IViolation> EnabledRules { get; private set; }

    public List<string> RuleNames { get; private set; }

    public static ConsoleEditorLogger Create() {
      var logger = ScriptableObject.CreateInstance<ConsoleEditorLogger>();
      logger.NoErrors = 0;
      logger.NoWarnings = 0;
      return logger;
    }
  }

  public class ConsoleEditorWindow : EditorWindow {
    public static ConsoleEditorWindow Instance {
      get {
        if (instance == null) {
          Init();
        }

        return instance;
      }
    }

    private static ConsoleEditorWindow instance;

    private Vector2 drawPosition;
    private Texture2D errorIcon;
    private Texture2D warningIcon;

    private ConsoleEditorLogger logger;

    private GUIStyle logEntryOdd;
    private GUIStyle logEntryEven;

    private Color lineColour;

    private bool showErrors = true;
    private bool showWarnings = true;

    private Dictionary<Rule, List<IViolation>> rulesAndTheirViolations;

    private Vector2 logListScrollPosition;
    private float logListMaxWidth;
    private float logListLineHeight;
    private int selectedLog = -1;

    [MenuItem("Mooble/Show Console")]
    public static void ShowWindow() {
      Init();
    }

    private static void Init() {
      instance = EditorWindow.GetWindow(typeof(ConsoleEditorWindow), false, "Mooble") as ConsoleEditorWindow;
      instance.Show();
    }

    public void SetViolations(Dictionary<Rule, List<IViolation>> violations) {
      this.rulesAndTheirViolations = new Dictionary<Rule, List<IViolation>>();
      this.rulesAndTheirViolations = StaticAnalysis.StaticAnalysis.MergeRuleViolationDictionary(this.rulesAndTheirViolations, violations);
    }

    private void OnEnable() {
      if (!this.logger) {
        this.logger = ConsoleEditorLogger.Create();
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
      this.DrawToolbar();
      this.DrawScrollRect(this.drawPosition.y);
    }

    private void DrawToolbar() {
      if (this.rulesAndTheirViolations == null) {
        this.rulesAndTheirViolations = new Dictionary<Rule, List<IViolation>>();
      }

      var toolbarStyle = EditorStyles.toolbarButton;

      this.drawPosition.x = position.width * 0.01f;

      Vector2 sizeOfElement;
      if (this.DrawButton("Clear", EditorStyles.toolbarButton, out sizeOfElement)) {
        // TODO: write code to clear logger;
      }

      this.drawPosition.x = position.width * 0.25f;

      this.DrawLabel("Rule: ", EditorStyles.toolbarButton, out sizeOfElement);
      this.drawPosition.x += sizeOfElement.x;

      var rules = this.rulesAndTheirViolations.Keys;
      var ruleNames = new List<string>();
      var longestRuleName = string.Empty;
      foreach (var rule in rules) {
        ruleNames.Add(rule.Name);
        Debug.Log(rule.Name);
        longestRuleName = longestRuleName.Length > rule.Name.Length ? longestRuleName : rule.Name;
      }

      EditorGUI.Popup(
        new Rect(
          this.drawPosition,
          toolbarStyle.CalcSize(new GUIContent(longestRuleName))),
        0,
        ruleNames.ToArray(),
        EditorStyles.toolbarPopup);

      var errorToggleContent = new GUIContent(this.logger.NoErrors.ToString(), this.errorIcon);
      var warningToggleContent = new GUIContent(this.logger.NoWarnings.ToString(), this.warningIcon);

      float totalErrorButtonWidth =
        toolbarStyle.CalcSize(errorToggleContent).x +
        toolbarStyle.CalcSize(warningToggleContent).x;

      float errorIconX = position.width - totalErrorButtonWidth;

      if (errorIconX > this.drawPosition.x) {
        this.drawPosition.x = errorIconX;
      }

      this.DrawToggleButton(this.showErrors, errorToggleContent, toolbarStyle, out sizeOfElement);
      this.drawPosition.x += sizeOfElement.x;

      this.DrawToggleButton(this.showWarnings, warningToggleContent, toolbarStyle, out sizeOfElement);
      this.drawPosition.x += sizeOfElement.x;

      this.drawPosition.y += sizeOfElement.y;

      this.drawPosition.x = 0;
    }

    private void DrawScrollRect(float height) {
      if (this.rulesAndTheirViolations == null) {
        return;
      }

      var oldColor = GUI.backgroundColor;
      var logLineStyle = this.logEntryEven;
      var logLineSize = logLineStyle.CalcSize(new GUIContent("A"));

      foreach (var kvp in this.rulesAndTheirViolations) {
        for (var i = 0; i < kvp.Value.Count; i++) {
          var size = logLineStyle.CalcSize(new GUIContent(kvp.Value[i].Format(), this.warningIcon));
          this.logListMaxWidth = Mathf.Max(this.logListMaxWidth, size.x);
          this.logListLineHeight = Mathf.Max(this.logListLineHeight, size.y);
        }
      }

      this.logListLineHeight *= 1.1f;

      var scrollRect = new Rect(this.drawPosition, new Vector2(position.width, position.height));
      var contentRect = new Rect(0, 0, Mathf.Max(this.logListMaxWidth, scrollRect.width), this.rulesAndTheirViolations.Count * this.logListLineHeight);

      this.logListScrollPosition = GUI.BeginScrollView(scrollRect, this.logListScrollPosition, contentRect);

      var toDisplay = new List<IViolation>();

      if (this.showErrors) {
        foreach (var kvp in this.rulesAndTheirViolations) {
          if (kvp.Key.Level == ViolationLevel.Error) {
            toDisplay.AddRange(kvp.Value);
          }
        }
      }

      if (this.showWarnings) {
        foreach (var kvp in this.rulesAndTheirViolations) {
          if (kvp.Key.Level == ViolationLevel.Warning) {
            toDisplay.AddRange(kvp.Value);
          }
        }
      }

      for (var i = 0; i < toDisplay.Count; i++) {
        logLineStyle = i % 2 == 0 ? this.logEntryEven : this.logEntryOdd;
        GUI.backgroundColor = i == this.selectedLog ? new Color(0.5f, 0.5f, 1) : Color.white;

        var rect = new Rect(0, this.logListLineHeight * i, contentRect.width, this.logListLineHeight);
        if (GUI.Button(rect, new GUIContent(toDisplay[i].Format(), this.warningIcon), logLineStyle)) {
          if (i != this.selectedLog) {
            this.selectedLog = i;
          }

          var go = toDisplay[i].GetObject();
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
  }
}
