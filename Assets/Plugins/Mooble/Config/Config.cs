#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Mooble.Config {
  public class Config {
    public string[] PrefabLocations;
    public StaticAnalysisRuleConfig[] Rules;

    public static Config LoadFromFile(string fileName = "moobleconfig.json") {
      string text = System.IO.File.ReadAllText(fileName);
      return JsonUtility.FromJson<Config>(text);
    }

    [Serializable]
    public class StaticAnalysisRuleConfig {
      public string Assembly;
      public string Name;
      public string ViolationLevel;
      public string[] Exclusions;
    }
  }
}
#endif
