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
      return this.rule.Level + " " + this.rule.Name + ": " + this.component.name + "." + this.component.GetType() + " is inactive.";
    }
  }
}
