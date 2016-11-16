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
        "{0} {1}: {2}.{3} is inactive.",
        this.rule.Level,
        this.rule.Name,
        this.component.name,
        this.component.GetType());
    }

    public Object GetObject() {
      return this.component;
    }
  }
}
