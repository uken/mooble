using UnityEngine;

namespace Mooble.StaticAnalysis {
  public class NoInactiveBehavioursViolation : IViolation {
    private Rules.NoInactiveBehaviours rule;
    private Component component;

    public NoInactiveBehavioursViolation(Rules.NoInactiveBehaviours r, Component c) {
      this.rule = r;
      this.component = c;
    }

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
