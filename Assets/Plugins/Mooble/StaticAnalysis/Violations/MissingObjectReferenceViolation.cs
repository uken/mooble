using System;

namespace Mooble.StaticAnalysis.Violation {
  public class MissingObjectReferenceViolation : IViolation {
    private Rules.MissingObjectReference rule;
    private Type type;
    private string name;

    public MissingObjectReferenceViolation(Rules.MissingObjectReference r, Type t, string n) {
      this.rule = r;
      this.type = t;
      this.name = n;
    }

    public string Format() {
      return this.rule.Level + " " + this.rule.Name + ": " + this.type + "." + this.name + " is undefined.";
    }
  }
}
