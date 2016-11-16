using System;
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
        "{0} {1}: Field '{4}' from Component '{3}' of '{2}' is undefined.",
        this.rule.Level,
        this.rule.Name,
        this.component.gameObject.name,
        this.component.GetType(),
        this.name);
    }
  }
}
