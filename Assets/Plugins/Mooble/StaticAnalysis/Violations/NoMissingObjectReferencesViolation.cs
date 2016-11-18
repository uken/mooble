using UnityEngine;

namespace Mooble.StaticAnalysis.Violation {
  public class NoMissingObjectReferencesViolation : IViolation {
    private Rules.NoMissingObjectReferences rule;
    private Component component;
    private string name;

    public NoMissingObjectReferencesViolation(Rules.NoMissingObjectReferences r, Component c, string n) {
      this.rule = r;
      this.component = c;
      this.name = n;
    }

    public string Format() {
      return string.Format(
        "Field '{2}' from Component '{1}' of '{0}' is undefined.",
        this.component.gameObject.name,
        this.component.GetType(),
        this.name);
    }

    public UnityEngine.Object GetObject() {
      return this.component;
    }
  }
}
