using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mooble.EditorExtension {
  using StaticAnalysis;
  using StaticAnalysis.Rules;

  public static class StaticAnalysisMenu {
    [MenuItem("Mooble/StaticAnalysis/AnalyzePrefabs")]
    public static void PrintPrefabAnalysis() {
      var config = Config.Config.LoadFromFile();
      var sa = LoadStaticAnalysisRules(config);

      // TODO: Configure prefab location
      var prefabDirectories = config.PrefabLocations;
      var assets = AssetDatabase.FindAssets("t:prefab", prefabDirectories);
      var violations = new List<IViolation>();

      for (var i = 0; i < assets.Length; i++) {
        var asset = assets[i];
        var path = AssetDatabase.GUIDToAssetPath(asset);
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        violations.AddRange(sa.Analyze(obj));
      }

      for (int i = 0; i < violations.Count; i++) {
        Log.Warning(violations[i].Format(), violations[i].GetObject());
      }
    }

    [MenuItem("Mooble/StaticAnalysis/AnalyzeScene")]
    public static void PrintSceneAnalysis() {
      var config = Config.Config.LoadFromFile();
      var sa = LoadStaticAnalysisRules(config);

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
        Log.Warning(violations[i].Format(), violations[i].GetObject());
      }
    }

    private static StaticAnalysis LoadStaticAnalysisRules(Config.Config config) {
      var sa = new StaticAnalysis();

      for (var i = 0; i < config.Rules.Length; i++) {
        var ruleConfig = config.Rules[i];
        var klassName = ruleConfig.Name;
        var ns = "Mooble.StaticAnalysis.Rules.";

        if (!klassName.StartsWith(ns)) {
          klassName = ns + klassName;
        }

        Type ruleClass = typeof(StaticAnalysis).Assembly.GetType(klassName);

        if (ruleClass == null) {
          Log.Debug("Could not find rule class: " + klassName + ". Skipping.");
          continue;
        }

        if (!ruleClass.BaseType.IsGenericType || ruleClass.BaseType.BaseType != typeof(Rule)) {
          Log.Debug("Rule " + klassName + " did not extend Rule class two levels up; parent was not generic type. Skipping.");
          continue;
        }

        var level = (ViolationLevel)Enum.Parse(typeof(ViolationLevel), ruleConfig.ViolationLevel);
        ConstructorInfo ctor = ruleClass.GetConstructor(new[] { typeof(ViolationLevel) });
        var rule = (Rule)ctor.Invoke(new object[] { level });
        var ruleObjectType = ruleClass.BaseType.GetGenericArguments()[0];

        sa.RegisterRule(ruleObjectType, rule);
      }

      return sa;
    }
  }
}
