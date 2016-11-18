#if UNITY_EDITOR
using UnityEngine;

namespace Mooble.StaticAnalysis.Violation {
  public class NoMissingObjectReferencesViolation : IViolation {
    private Component component;
    private string name;

    public NoMissingObjectReferencesViolation(ViolationLevel level, Component c, string n) {
      this.component = c;
      this.Level = level;
      this.name = n;
    }

    public ViolationLevel Level { get; set; }

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
#endif
