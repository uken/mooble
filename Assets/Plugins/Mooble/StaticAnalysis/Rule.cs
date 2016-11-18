using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mooble.StaticAnalysis {
  public abstract class Rule {
    public readonly ViolationLevel Level;

    protected Rule(ViolationLevel level) {
      this.Level = level;
    }

    public string Name {
      get {
        return this.GetType().Name;
      }
    }

    public abstract List<IViolation> Handle(object thing);
  }

  [SuppressMessage(
  "Microsoft.StyleCop.CSharp.MaintainabilityRules",
  "SA1402:FileMayOnlyContainASingleClass",
  Justification = "The rule class exists only because of C#'s silly generic system.")]
  public abstract class Rule<T> : Rule {
    protected Rule(ViolationLevel level) : base(level) {
    }

    public abstract List<IViolation> Handle(T thing);
  }
}
