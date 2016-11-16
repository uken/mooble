using UnityEngine;

namespace Mooble.StaticAnalysis {
  public class InactiveComponentViolation : IViolation {
    private Rules.InactiveComponent rule;
    private Component component;

    public InactiveComponentViolation(Rules.InactiveComponent r, Component c) {
      this.rule = r;
      this.component = c;
    }

    public string Format() {
      return this.rule.Level + " " + this.rule.Name + ": " + this.component.name + "." + this.component.GetType() + " is inactive.";
    }
  }
}
