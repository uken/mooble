using System.Collections.Generic;

using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class InactiveComponent : Rule<Behaviour> {
    public const string NAME = "InactiveComponent";

    public InactiveComponent() : base(NAME, ViolationLevel.Warning) {
    }

    public override List<IViolation> Handle(object thing) {
      return this.Handle(thing as Behaviour);
    }

    public override List<IViolation> Handle(Behaviour thing) {
      var violations = new List<IViolation>();

      if (thing == null || thing.enabled) {
        return violations;
      }

      violations.Add(new InactiveComponentViolation(this, thing));

      return violations;
    }
  }
}
