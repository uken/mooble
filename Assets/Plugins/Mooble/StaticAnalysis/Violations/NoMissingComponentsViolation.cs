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
        "{0} {1}: GameObject {2} has an undefined component (missing script reference).",
        this.rule.Level,
        this.rule.Name,
        this.gameObject.name);
    }

    public UnityEngine.Object GetObject() {
      return this.gameObject;
    }
  }
}
