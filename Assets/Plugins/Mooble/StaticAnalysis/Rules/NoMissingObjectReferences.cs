using System;
using System.Collections.Generic;
using System.Reflection;

using Mooble.StaticAnalysis.Violation;
using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class NoMissingObjectReferences : Rule<MonoBehaviour> {
    public const string NAME = "MissingObjectReference";

    public NoMissingObjectReferences() : base(NAME, ViolationLevel.Warning) {
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
      FieldInfo[] fields = componentType.GetFields(
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      PropertyInfo[] properties = componentType.GetProperties(
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
          violations.Add(new NoMissingObjectReferencesViolation(this, thing, field.Name));
        }
      }

      foreach (var property in properties) {
        object val;

        try {
          val = property.GetValue(thing, null);
        } catch {
          continue;
        }

        if (property.IsDefined(typeof(HideInInspector), false) || property.GetSetMethod() == null) {
          continue;
        }

        if (property.PropertyType.IsClass && val == null) {
          violations.Add(new NoMissingObjectReferencesViolation(this, thing, property.Name));
        }
      }

      return violations;
    }
  }
}
