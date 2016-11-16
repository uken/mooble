using System;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mooble.StaticAnalysis {
  public class StaticAnalysis {
    private List<Rule<GameObject>> gameObjectRules;
    private Dictionary<Type, List<Rule>> componentRules;

    public StaticAnalysis() {
      this.gameObjectRules = new List<Rule<GameObject>>();
      this.componentRules = new Dictionary<Type, List<Rule>>();
    }

    [MenuItem("Mooble/StaticAnalysis/AnalyzePrefabs")]
    public static void PrintPrefabAnalysis() {
      var sa = new StaticAnalysis();
      sa.RegisterRule(new Rules.NoInactiveBehaviours());
      sa.RegisterRule(new Rules.NoDuplicateComponents());
      sa.RegisterRule(new Rules.NoMissingObjectReferences());

      var assets = AssetDatabase.FindAssets("t:prefab");
      var violations = new List<IViolation>();

      for (var i = 0; i < assets.Length; i++) {
        var asset = assets[i];
        var path = AssetDatabase.GUIDToAssetPath(asset);
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        violations.AddRange(sa.Analyze(obj));
      }

      for (int i = 0; i < violations.Count; i++) {
        Log.Warning(violations[i].Format());
      }
    }

    [MenuItem("Mooble/StaticAnalysis/AnalyzeScene")]
    public static void PrintSceneAnalysis() {
      var sa = new StaticAnalysis();
      sa.RegisterRule(new Rules.NoInactiveBehaviours());
      sa.RegisterRule(new Rules.NoDuplicateComponents());
      sa.RegisterRule(new Rules.NoMissingObjectReferences());

      var scenes = SceneManager.GetAllScenes();
      var violations = new List<IViolation>();

      for (var i = 0; i < scenes.Length; i++) {
        var scene = scenes[i];

        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        for (var j = 0; j < rootGameObjects.Length; j++) {
          var obj = rootGameObjects[j];
          violations.AddRange(sa.Analyze(obj));
        }
      }

      for (int i = 0; i < violations.Count; i++) {
        Log.Debug(violations[i].Format());
      }
    }

    public void RegisterRule<T>(Rule<T> rule) {
      if (!this.componentRules.ContainsKey(typeof(T))) {
        this.componentRules[typeof(T)] = new List<Rule>();
      }

      this.componentRules[typeof(T)].Add(rule);
    }

    public void RegisterRule(Rule<GameObject> rule) {
      this.gameObjectRules.Add(rule);
    }

    public List<IViolation> Analyze(GameObject root) {
      var violations = new List<IViolation>();

      for (var j = 0; j < this.gameObjectRules.Count; j++) {
        var rule = this.gameObjectRules[j];
        violations.AddRange(rule.Handle(root));
      }

      foreach (var componentRuleSet in this.componentRules) {
        var rules = componentRuleSet.Value;

        for (var j = 0; j < rules.Count; j++) {
          var rule = rules[j];
          var components = root.GetComponents(componentRuleSet.Key);

          for (var i = 0; i < components.Length; i++) {
            var component = components[i];
            violations.AddRange(rule.Handle(component));
          }
        }
      }

      foreach (Transform child in root.transform) {
        violations.AddRange(this.Analyze(child.gameObject));
      }

      return violations;
    }
  }
}
