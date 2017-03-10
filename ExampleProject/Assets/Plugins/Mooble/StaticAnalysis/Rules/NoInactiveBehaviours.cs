#if UNITY_EDITOR
using System;
using System.Collections.Generic;

using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class NoInactiveBehaviours : Rule<Behaviour> {
    private List<Type> excludedTypes;

    public NoInactiveBehaviours(
      ViolationLevel level = ViolationLevel.Warning,
      ViolationScope scope = ViolationScope.Both,
      List<Type> excludedTypes = null) : base(level, scope) {
      this.excludedTypes = excludedTypes ?? new List<Type>();
    }

    public override List<IViolation> Handle(object thing) {
      return this.Handle(thing as Behaviour);
    }

    public override List<IViolation> Handle(Behaviour thing) {
      var violations = new List<IViolation>();

      if (thing == null || thing.enabled || this.excludedTypes.Contains(thing.GetType())) {
        return violations;
      }

      violations.Add(new Violation.NoInactiveBehavioursViolation(this.Level, thing));

      return violations;
    }
  }
}
#endif
