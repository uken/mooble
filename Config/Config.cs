#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mooble.Config
{
  public class Config
  {
    private static readonly string[] POTENTIAL_CONFIG_FILES = new[] {
      "moobleconfig.json",
      "Assets/Plugins/Mooble/moobleconfig.json"
    };

    public string[] PrefabLocations;
    public string[] SceneLocations;
    public string[] IgnoredSceneRootObjectNames;
    public StaticAnalysisRuleConfig[] Rules;

    public static Config LoadFromFile(string fileName = null)
    {
      List<string> fileNamesToTry = new List<string>();
      fileNamesToTry.Add(fileName);
      fileNamesToTry.AddRange(POTENTIAL_CONFIG_FILES);

      foreach (string path in fileNamesToTry)
      {
        if (System.IO.File.Exists(path))
        {
          Log.Debug("Mooble: Using config file found at " + path);
          string text = System.IO.File.ReadAllText(path);
          return JsonUtility.FromJson<Config>(text);
        }
      }

      throw new ArgumentException("Could not not find Mooble config at " + fileName + " or any of the default locations: " + string.Join(", ", POTENTIAL_CONFIG_FILES));
    }

    [Serializable]
    public class StaticAnalysisRuleConfig
    {
      public string Assembly;
      public string Name;
      public string ViolationLevel;
      public string ViolationScope;
      public string[] Exclusions;
    }
  }
}
#endif
