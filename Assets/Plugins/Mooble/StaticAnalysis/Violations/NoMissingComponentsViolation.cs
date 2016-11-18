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

    public string Format() {
      return string.Format(
        "GameObject {0} has an undefined component (missing script reference).",
        this.gameObject.name);
    }

    public UnityEngine.Object GetObject() {
      return this.gameObject;
    }
  }
}
#endif
