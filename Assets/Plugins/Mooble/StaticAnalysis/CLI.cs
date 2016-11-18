using System.Collections.Generic;
using System.Text;

using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mooble.StaticAnalysis {
  public static class CLI {
    public static void RunSceneAnalysis() {
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

      var config = Mooble.Config.Config.LoadFromFile();
      var sa = new StaticAnalysisBuilder(config).Get();

      var stringBuilder = new StringBuilder();
      bool foundError = false;

      foreach (var scene in scenes) {
        stringBuilder.Append("\nAnalyzing scene: " + scene.name);

        foreach (var root in scene.GetRootGameObjects()) {
          Dictionary<Rule, List<IViolation>> violations = sa.Analyze(root);
          stringBuilder.Append("\n\tAnalyzing root object in scene: " + root.name);

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
        }
      }

      Log.Debug(stringBuilder.ToString());

      if (foundError) {
        throw new System.Exception("Error violation found!");
      }
    }

    public static string FullPath(UnityEngine.Object o) {
      if (o is Component) {
        return FullPath(o as Component);
      } else {
        return FullPath(o as GameObject);
      }
    }

    public static string FullPath(Component c) {
      return FullPath(c.gameObject);
    }

    public static string FullPath(GameObject gameObject) {
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
