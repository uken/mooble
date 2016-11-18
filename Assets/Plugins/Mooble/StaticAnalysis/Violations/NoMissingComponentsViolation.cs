using UnityEngine;

namespace Mooble.StaticAnalysis.Violation {
  public class NoMissingComponentsViolation : IViolation {
    private Rules.NoMissingComponents rule;
    private GameObject gameObject;

    public NoMissingComponentsViolation(Rules.NoMissingComponents r, GameObject o) {
      this.rule = r;
      this.gameObject = o;
    }

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
