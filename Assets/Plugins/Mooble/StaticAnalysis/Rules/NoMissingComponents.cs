using System.Collections.Generic;

using Mooble.StaticAnalysis.Violation;
using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class NoMissingComponents : Rule<GameObject> {
    public NoMissingComponents(ViolationLevel level = ViolationLevel.Error) : base(level) {
    }

    public override List<IViolation> Handle(object thing) {
      return this.Handle(thing as GameObject);
    }

    public override List<IViolation> Handle(GameObject thing) {
      var violations = new List<IViolation>();
      Component[] allComponents = thing.GetComponents<Component>();

      foreach (var component in allComponents) {
        if (component == null) {
          violations.Add(new NoMissingComponentsViolation(this, thing));
        }
      }

      return violations;
    }
  }
}
