using System.Collections.Generic;

using Mooble.StaticAnalysis.Violation;
using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class DuplicateComponent : Rule<GameObject> {
    public const string NAME = "DuplicateComponent";

    public DuplicateComponent() : base(NAME, ViolationLevel.Warning) {
    }

    public override List<IViolation> Handle(object thing) {
      return this.Handle(thing as GameObject);
    }

    public override List<IViolation> Handle(GameObject thing) {
      var violations = new List<IViolation>();
      Component[] allComponents = thing.GetComponents<Component>();

      var s = new Dictionary<string, int>();

      foreach (var component in allComponents) {
        var componentType = component.GetType().ToString();

        if (s.ContainsKey(componentType)) {
          s[componentType]++;
        } else {
          s[componentType] = 1;
        }
      }

      foreach (var kvp in s) {
        if (kvp.Value > 1) {
          violations.Add(new DuplicateComponentViolation(this, thing.name, kvp.Key, kvp.Value));
        }
      }

      return violations;
    }
  }
}
