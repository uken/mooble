using System;
using System.Collections.Generic;
using System.Reflection;

using Mooble.StaticAnalysis.Violation;
using UnityEngine;

namespace Mooble.StaticAnalysis.Rules {
  public class NoMissingObjectReferences : Rule<Component> {
    public const string NAME = "MissingObjectReference";

    public NoMissingObjectReferences() : base(NAME, ViolationLevel.Warning) {
    }

    public override List<IViolation> Handle(object thing) {
      return this.Handle(thing as Component);
    }

    public override List<IViolation> Handle(Component thing) {
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
          violations.Add(new NoMissingObjectReferencesViolation(this, componentType, field.Name));
        }
      }

      foreach (var property in properties) {
        var val = property.GetValue(thing, null);

        if (property.IsDefined(typeof(HideInInspector), false) || property.GetSetMethod() == null) {
          continue;
        }

        if (property.PropertyType.IsClass && val == null) {
          violations.Add(new NoMissingObjectReferencesViolation(this, componentType, property.Name));
        }
      }

      return violations;
    }
  }
}
