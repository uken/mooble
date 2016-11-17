using System.Collections.Generic;

using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class NoInactiveBehaviours : Rule<Behaviour> {
    public NoInactiveBehaviours(ViolationLevel level = ViolationLevel.Warning) : base(level) {
    }

    public override List<IViolation> Handle(object thing) {
      return this.Handle(thing as Behaviour);
    }

    public override List<IViolation> Handle(Behaviour thing) {
      var violations = new List<IViolation>();

      if (thing == null || thing.enabled) {
        return violations;
      }

      violations.Add(new NoInactiveBehavioursViolation(this, thing));

      return violations;
    }
  }
}
