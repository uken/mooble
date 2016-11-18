using System.Collections.Generic;
using System.Text;

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
      var sa = new StaticAnalysisBuilder(config).Get();

      var prefabDirectories = config.PrefabLocations;
      var assets = AssetDatabase.FindAssets("t:prefab", prefabDirectories);
      bool foundError = false;
      var stringBuilder = new StringBuilder();

      for (var i = 0; i < assets.Length; i++) {
        var asset = assets[i];
        var path = AssetDatabase.GUIDToAssetPath(asset);
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        stringBuilder.Append("\nAnalyzing prefab: " + path);

        var violations = sa.Analyze(obj);
        var foundErrorThisTime = AppendViolations(stringBuilder, violations);
        foundError = foundError || foundErrorThisTime;
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

      foreach (var scene in scenes) {
        stringBuilder.Append("\nAnalyzing scene: " + scene.name);

        foreach (var root in scene.GetRootGameObjects()) {
          stringBuilder.Append("\n\tAnalyzing root object in scene: " + root.name);
          Dictionary<Rule, List<IViolation>> violations = sa.Analyze(root);

          var foundErrorThisTime = AppendViolations(stringBuilder, violations);
          foundError = foundError || foundErrorThisTime;
        }
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
          scene = EditorSceneManager.OpenScene(arg);
        } catch (System.ArgumentException) {
          continue;
        }

        if (scene.IsValid()) {
          Log.Debug("Found valid scene: " + scene.name);
          scenes.Add(scene);
        }
      }

      if (scenes.Count == 0) {
        throw new System.Exception("No scenes provided; skipping static analysis.");
      }

      return scenes;
    }

    private static bool AppendViolations(StringBuilder stringBuilder, Dictionary<Rule, List<IViolation>> violations) {
      bool foundError = false;

      foreach (var kvp in violations) {
        if (kvp.Value.Count == 0) {
          continue;
        }

        stringBuilder.Append("\n\t\tViolations for rule: ");
        stringBuilder.Append(kvp.Key.Name);

        foreach (var violation in kvp.Value) {
          if (violation.Level == ViolationLevel.Error) {
            foundError = true;
          }

          stringBuilder.Append("\n");
          stringBuilder.Append("\t\t\t");
          stringBuilder.Append(FullPath(violation.GetObject()));
          stringBuilder.Append(": ");
          stringBuilder.Append(violation.Format());
        }
      }

      return foundError;
    }

    private static string FullPath(UnityEngine.Object o) {
      if (o is Component) {
        return FullPath(o as Component);
      } else {
        return FullPath(o as GameObject);
      }
    }

    private static string FullPath(Component c) {
      return FullPath(c.gameObject);
    }

    private static string FullPath(GameObject gameObject) {
      var path = new StringBuilder();

      var transform = gameObject.transform;
      path.Append(transform.name);

      while (transform.parent != null) {
        transform = transform.parent;
        path.Insert(0, "/");
        path.Insert(0, transform.name);
      }

      path.Insert(0, "/");

      return path.ToString();
    }
  }
}
