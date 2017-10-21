#if UNITY_EDITOR
using UnityEngine;

namespace Mooble.StaticAnalysis.Violation {
  public class NoMissingComponentsViolation : IViolation {
    private GameObject gameObject;

    public NoMissingComponentsViolation(ViolationLevel level, GameObject o) {
      this.Level = level;
      this.gameObject = o;
    }

    public ViolationLevel Level { get; set; }

    public string FormatCLI() {
      return string.Format(
        "{1} {0}: GameObject has an undefined Component (missing script reference).",
        Utility.FormatObjectPath(this.gameObject),
        this.Level.ToString().ToUpper());
    }

    public string FormatEditor() {
      return string.Format(
        "GameObject {0} has an undefined Component (missing script reference).",
        Utility.FormatPrimaryObject(Utility.FormatObjectPath(this.gameObject)));
    }

    public UnityEngine.Object GetObject() {
      return this.gameObject;
    }
  }
}
#endif
