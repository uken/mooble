using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mooble.StaticAnalysis {
  public abstract class Rule {
    public abstract List<IViolation> Handle(object thing);
  }

  [SuppressMessage(
  "Microsoft.StyleCop.CSharp.MaintainabilityRules",
  "SA1402:FileMayOnlyContainASingleClass",
  Justification = "The rule class exists only because of C#'s silly generic system.")]
  public abstract class Rule<T> : Rule {
    public readonly string Name;
    public readonly ViolationLevel Level;

    protected Rule(string name, ViolationLevel level) {
      this.Name = name;
      this.Level = level;
    }

    public abstract List<IViolation> Handle(T thing);
  }
}
