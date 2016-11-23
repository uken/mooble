#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mooble.EditorExtension {
  using StaticAnalysis;
  using StaticAnalysis.Rules;

  public static class StaticAnalysisMenu {
    [MenuItem("Mooble/Static Analysis/Analyze All Prefabs")]
    public static void PrintPrefabAnalysis() {
      var config = Config.Config.LoadFromFile();
      var sa = LoadStaticAnalysisRules(config);

      var prefabDirectories = config.PrefabLocations;
      var assets = AssetDatabase.FindAssets("t:prefab", prefabDirectories);
      var violations = new Dictionary<Rule, List<IViolation>>();

      for (var i = 0; i < assets.Length; i++) {
        var asset = assets[i];
        var path = AssetDatabase.GUIDToAssetPath(asset);
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        violations = StaticAnalysis.MergeRuleViolationDictionary(violations, sa.Analyze(ViolationScope.Prefab, obj));
      }

      EditorExtensions.ConsoleWindow.Instance.SetViolations(violations);
    }

    [MenuItem("Mooble/Static Analysis/Analyze This Scene")]
    public static void PrintSceneAnalysis() {
      var config = Config.Config.LoadFromFile();
      var sa = LoadStaticAnalysisRules(config);

      var scenes = SceneManager.GetAllScenes();
      var violations = new Dictionary<Rule, List<IViolation>>();

      for (var i = 0; i < scenes.Length; i++) {
        var scene = scenes[i];

        GameObject[] rootGameObjects = scene.GetRootGameObjects();
        for (var j = 0; j < rootGameObjects.Length; j++) {
          var obj = rootGameObjects[j];
          violations = StaticAnalysis.MergeRuleViolationDictionary(violations, sa.Analyze(ViolationScope.Scene, obj));
        }
      }

      EditorExtensions.ConsoleWindow.Instance.SetViolations(violations);
    }

    private static StaticAnalysis LoadStaticAnalysisRules(Config.Config config) {
      return new StaticAnalysisBuilder(config).Get();
    }
  }
}
#endif
