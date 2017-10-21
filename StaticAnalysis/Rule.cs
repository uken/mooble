#if UNITY_EDITOR
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Mooble.StaticAnalysis {
  public abstract class Rule {
    public readonly ViolationLevel Level;
    public readonly ViolationScope Scope;

    protected Rule(ViolationLevel level, ViolationScope scope) {
      this.Level = level;
      this.Scope = scope;
    }

    public string Name {
      get {
        return this.GetType().Name;
      }
    }

    public abstract List<IViolation> Handle(object thing);

    public bool ValidForScope(ViolationScope scope) {
      return this.Scope == ViolationScope.Both || this.Scope == scope;
    }
  }

  [SuppressMessage(
  "Microsoft.StyleCop.CSharp.MaintainabilityRules",
  "SA1402:FileMayOnlyContainASingleClass",
  Justification = "The rule class exists only because of C#'s silly generic system.")]
  public abstract class Rule<T> : Rule {
    protected Rule(ViolationLevel level, ViolationScope scope) : base(level, scope) {
    }

    public abstract List<IViolation> Handle(T thing);
  }
}
#endif
