using System;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using Mooble.StaticAnalysis.Violation;

namespace Mooble.StaticAnalysis.Rules {
  public class MissingObjectReference : Rule<Component> {
    public const string NAME = "MissingObjectReference";

    public MissingObjectReference() : base(NAME, ViolationLevel.Warning) {
    }

    public override List<IViolation> Handle(Component thing) {
      var violations = new List<IViolation>();

      Type componentType = thing.GetType();
      FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      PropertyInfo[] properties = componentType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

      foreach (var field in fields) {
        var val = field.GetValue(thing);

        if (field.IsNotSerialized) {
          continue;
        }

        if (field.FieldType.IsClass && val == null) {
          violations.Add(new MissingObjectReferenceViolation(this, componentType, field.Name));
        }
      }

      foreach (var property in properties) {
        var val = property.GetValue(thing, null);

        if (property.PropertyType.IsClass && val == null) {
          violations.Add(new MissingObjectReferenceViolation(this, componentType, property.Name));
        }
      }

      return violations;
    }
  }
}
