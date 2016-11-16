using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mooble.StaticAnalysis {
  public class StaticAnalysis {
    private List<Rule<GameObject>> gameObjectRules;
    private List<Rule<Component>> componentRules;

    public StaticAnalysis() {
      this.gameObjectRules = new List<Rule<GameObject>>();
      this.componentRules = new List<Rule<Component>>();
    }

    public void RegisterRule(Rule<Component> rule) {
      this.componentRules.Add(rule);
    }

    public void RegisterRule(Rule<GameObject> rule) {
      this.gameObjectRules.Add(rule);
    }

    public List<IViolation> Analyze(GameObject root) {
      var components = root.GetComponents<Component>();
      var violations = new List<IViolation>();

      for (var j = 0; j < this.gameObjectRules.Count; j++) {
        var rule = this.gameObjectRules[j];
        violations.AddRange(rule.Handle(root));
      }

      for (var i = 0; i < components.Length; i++) {
        var component = components[i];

        for (var j = 0; j < this.componentRules.Count; j++) {
          var rule = this.componentRules[j];
          violations.AddRange(rule.Handle(component));
        }
      }

      foreach (Transform child in root.transform) {
        violations.AddRange(this.Analyze(child.gameObject));
      }

      return violations;
    }
  }
}
