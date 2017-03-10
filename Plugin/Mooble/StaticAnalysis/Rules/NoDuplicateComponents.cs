#if UNITY_EDITOR
using System.Collections.Generic;

using Mooble.StaticAnalysis.Violation;
using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class NoDuplicateComponents : Rule<GameObject> {
    public NoDuplicateComponents(
      ViolationLevel level = ViolationLevel.Warning,
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

      var s = new Dictionary<string, int>();

      foreach (var component in allComponents) {
        if (component == null) {
          continue;
        }

        var componentType = component.GetType().ToString();

        if (s.ContainsKey(componentType)) {
          s[componentType]++;
        } else {
          s[componentType] = 1;
        }
      }

      foreach (var kvp in s) {
        if (kvp.Value > 1) {
          violations.Add(new NoDuplicateComponentsViolation(this.Level, thing, kvp.Key, kvp.Value));
        }
      }

      return violations;
    }
  }
}
#endif
