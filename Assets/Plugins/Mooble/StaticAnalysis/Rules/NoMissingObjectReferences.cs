#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;

using Mooble.StaticAnalysis.Violation;
using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class NoMissingObjectReferences : Rule<MonoBehaviour> {
    public NoMissingObjectReferences(ViolationLevel level = ViolationLevel.Warning) : base(level) {
    }

    public override List<IViolation> Handle(object thing) {
      return this.Handle(thing as MonoBehaviour);
    }

    public override List<IViolation> Handle(MonoBehaviour thing) {
      var violations = new List<IViolation>();

      if (thing == null) {
        return violations;
      }

      Type componentType = thing.GetType();

      if (componentType.Namespace != null && componentType.Namespace.Contains("UnityEngine")) {
        return violations;
      }

      FieldInfo[] fields = componentType.GetFields(
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

      foreach (var field in fields) {
        var val = field.GetValue(thing);

        if (field.IsPrivate && !field.IsDefined(typeof(SerializeField), false)) {
          continue;
        }

        if (field.IsPublic && field.IsDefined(typeof(HideInInspector), false)) {
          continue;
        }

        if (field.FieldType.IsClass && val == null) {
          violations.Add(new NoMissingObjectReferencesViolation(this.Level, thing, field.Name));
        }
      }

      return violations;
    }
  }
}
#endif
