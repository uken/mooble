#if UNITY_EDITOR
using UnityEngine;

namespace Mooble.StaticAnalysis {
  public class NoInactiveBehavioursViolation : IViolation {
    private Component component;

    public NoInactiveBehavioursViolation(ViolationLevel level, Component c) {
      this.Level = level;
      this.component = c;
    }

    public ViolationLevel Level { get; set; }

    public string Format() {
      return string.Format(
        "{0}.{1} is inactive.",
        this.component.name,
        this.component.GetType());
    }

    public UnityEngine.Object GetObject() {
      return this.component;
    }
  }
}
#endif
