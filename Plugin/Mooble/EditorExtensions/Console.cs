#if UNITY_EDITOR
using System.Collections.Generic;

using UnityEngine;

namespace Mooble.EditorExtensions {
  [System.Serializable]
  public class Console : ScriptableObject {
    public int ErrorCount { get; set; }

    public int WarningCount { get; set; }

    public List<string> RuleNames { get; private set; }

    public static Console Create() {
      var logger = ScriptableObject.CreateInstance<Console>();
      logger.ErrorCount = 0;
      logger.WarningCount = 0;
      return logger;
    }
  }
}
#endif
