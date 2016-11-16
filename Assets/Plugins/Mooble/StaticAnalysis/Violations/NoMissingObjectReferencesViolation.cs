using System;

namespace Mooble.StaticAnalysis.Violation {
  public class NoMissingObjectReferencesViolation : IViolation {
    private Rules.NoMissingObjectReferences rule;
    private Type type;
    private string name;

    public NoMissingObjectReferencesViolation(Rules.NoMissingObjectReferences r, Type t, string n) {
      this.rule = r;
      this.type = t;
      this.name = n;
    }

    public string Format() {
      return this.rule.Level + " " + this.rule.Name + ": " + this.type + "." + this.name + " is undefined.";
    }
  }
}
