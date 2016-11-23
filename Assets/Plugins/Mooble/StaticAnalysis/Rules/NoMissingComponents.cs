#if UNITY_EDITOR
using System.Collections.Generic;

using Mooble.StaticAnalysis.Violation;
using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class NoMissingComponents : Rule<GameObject> {
    public NoMissingComponents(
      ViolationLevel level = ViolationLevel.Error,
      ViolationScope scope = ViolationScope.Both) : base(level, scope) {
    }

    public override List<IViolation> Handle(object thing) {
      return this.Handle(thing as GameObject);
    }

    public override List<IViolation> Handle(GameObject thing) {
      var violations = new List<IViolation>();

      if (thing == null) {
        return violations;
      }

      Component[] allComponents = thing.GetComponents<Component>();

      foreach (var component in allComponents) {
        if (component == null) {
          violations.Add(new NoMissingComponentsViolation(this.Level, thing));
        }
      }

      return violations;
    }
  }
}
#endif
