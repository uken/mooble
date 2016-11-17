using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mooble.EditorExtension {
  using StaticAnalysis;
  using StaticAnalysis.Rules;

  public static class StaticAnalysisMenu {
    [MenuItem("Mooble/StaticAnalysis/AnalyzePrefabs")]
    public static void PrintPrefabAnalysis() {
      var sa = new StaticAnalysis();
      sa.RegisterRule(new NoInactiveBehaviours());
      sa.RegisterRule(new NoDuplicateComponents());
      sa.RegisterRule(new NoMissingObjectReferences());

      // TODO: Configure prefab location
      var prefabDirectories = new[] { "Assets/Prefabs" };
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
      var sa = new StaticAnalysis();
      sa.RegisterRule(new NoInactiveBehaviours());
      sa.RegisterRule(new NoDuplicateComponents());
      sa.RegisterRule(new NoMissingObjectReferences());

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
  }
}
