#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mooble.StaticAnalysis {
  public static class CLI {
    /**
     * Sample Usage: /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath `pwd` -executeMethod Mooble.StaticAnalysis.CLI.RunPrefabAnalysis Assets/Prefabs/Bob.prefab Assets/Prefabs/Beans.prefab
     */
    public static void RunPrefabAnalysis() {
      var config = Mooble.Config.Config.LoadFromFile();
      var args = System.Environment.GetCommandLineArgs();
      var prefabPaths = new List<string>();

      for (int i = 0; i < args.Length; i++) {
        if (args[i].EndsWith(".prefab")) {
          prefabPaths.Add(args[i]);
        }
      }

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
     * Sample Usage: /Applications/Unity/Unity.app/Contents/MacOS/Unity -quit -batchmode -projectPath `pwd` -executeMethod Mooble.StaticAnalysis.CLI.RunSceneAnalysis Assets/Scene1.unity
     */
    public static void RunSceneAnalysis() {
      var scenes = LoadScenesFromArgs();

      var config = Mooble.Config.Config.LoadFromFile();
      var sa = new StaticAnalysisBuilder(config).Get();

      var stringBuilder = new StringBuilder();
      bool foundError = false;
      HashSet<string> ignoredSceneObjectNames = new HashSet<string>(config.IgnoredSceneRootObjectNames);

      foreach (var scene in scenes) {
        if (ignoredSceneObjectNames.Contains(root.name)) {
          stringBuilder.Append("\n  Ignoring root object in scene: " + root.name);
        } else {
          stringBuilder.Append("\n  Analyzing root object in scene: " + root.name);
          Dictionary<Rule, List<IViolation>> violations = sa.Analyze(ViolationScope.Scene, root);

          var foundErrorThisTime = AppendViolations(stringBuilder, violations);
          foundError = foundError || foundErrorThisTime;
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

    private static List<Scene> LoadScenesFromArgs() {
      var commandLineArgs = System.Environment.GetCommandLineArgs();

      var scenes = new List<Scene>();

      foreach (var arg in commandLineArgs) {
        Scene scene;

        try {
          scene = EditorSceneManager.OpenScene(arg, OpenSceneMode.Additive);
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
