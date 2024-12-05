#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;

using Com.Uken.Extensions;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mooble.StaticAnalysis {
  public static class CLI {
    /**
     * Sample Usage: /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath `pwd` -executeMethod Mooble.StaticAnalysis.CLI.RunPrefabAnalysis
     */
    public static void RunPrefabAnalysis() {
      var config = Mooble.Config.Config.LoadFromFile();

      var prefabPaths = AssetDatabase
        .FindAssets("t:prefab", config.PrefabLocations)
        .Map(guid => AssetDatabase.GUIDToAssetPath(guid));

      var sa = new StaticAnalysisBuilder(config).Get();

      bool foundError = false;
      var stringBuilder = new StringBuilder();

      for (var i = 0; i < prefabPaths.Count; i++) {
        var path = prefabPaths[i];
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        stringBuilder.Append("\nAnalyzing prefab: " + path);

        var violations = sa.Analyze(ViolationScope.Prefab, obj);
        var foundErrorThisTime = AppendViolations(stringBuilder, violations);
        foundError = foundError || foundErrorThisTime;
      }

      if (prefabPaths.Count == 0) {
        stringBuilder.Append("No prefabs were analyzed.");
      }

      Log.Debug(stringBuilder.ToString());

      if (foundError) {
        throw new System.Exception("Error level violation found!");
      }
    }

    /**
     * Sample Usage: /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath `pwd` -executeMethod Mooble.StaticAnalysis.CLI.RunSceneAnalysis
     */
    public static void RunSceneAnalysis() {
      var config = Mooble.Config.Config.LoadFromFile();

      var scenes = LoadScenesFromConfig(config);

      var sa = new StaticAnalysisBuilder(config).Get();

      var stringBuilder = new StringBuilder();
      bool foundError = false;
      HashSet<string> ignoredSceneObjectNames = new HashSet<string>(config.IgnoredSceneRootObjectNames);

      foreach (var scene in scenes) {
        foreach (var root in scene.GetRootGameObjects()) {
          if (ignoredSceneObjectNames.Contains(root.name)) {
            stringBuilder.Append("\n  Ignoring root object in scene: " + root.name);
          } else {
            stringBuilder.Append("\n  Analyzing root object in scene: " + root.name);
            Dictionary<Rule, List<IViolation>> violations = sa.Analyze(ViolationScope.Scene, root);

            var foundErrorThisTime = AppendViolations(stringBuilder, violations);
            foundError = foundError || foundErrorThisTime;
          }
        }
      }

      if (scenes.Count == 0) {
        stringBuilder.Append("No scenes were analyzed.");
      }

      Log.Debug(stringBuilder.ToString());

      if (foundError) {
        throw new System.Exception("Error level violation found!");
      }
    }

    private static List<Scene> LoadScenesFromConfig(Mooble.Config.Config config) {
      var scenes = new List<Scene>();

      var scenePaths = AssetDatabase
        .FindAssets("t:scene", config.SceneLocations)
        .Map(guid => AssetDatabase.GUIDToAssetPath(guid));

      foreach (string scenePath in scenePaths) {
        Scene scene;

        try {
          scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        } catch (System.ArgumentException) {
          continue;
        }

        if (scene.IsValid()) {
          Log.Debug("Found valid scene: " + scene.name);
          scenes.Add(scene);
        }
      }

      return scenes;
    }

    private static bool AppendViolations(StringBuilder stringBuilder, Dictionary<Rule, List<IViolation>> violations) {
      bool foundError = false;

      foreach (var kvp in violations) {
        if (kvp.Value.Count == 0) {
          continue;
        }

        stringBuilder.Append("\n    Violations for rule: ");
        stringBuilder.Append(kvp.Key.Name);

        foreach (var violation in kvp.Value) {
          if (violation.Level == ViolationLevel.Error) {
            foundError = true;
          }

          stringBuilder.Append("\n");
          stringBuilder.Append("      ");
          stringBuilder.Append(violation.FormatCLI());
        }
      }

      return foundError;
    }
  }
}
#endif
