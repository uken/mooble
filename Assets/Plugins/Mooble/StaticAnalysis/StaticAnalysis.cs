using System;
using System.Collections.Generic;

using UnityEngine;

namespace Mooble.StaticAnalysis {
  public class StaticAnalysis {
    private List<Rule<GameObject>> gameObjectRules;
    private Dictionary<Type, List<Rule>> componentRules;

    public StaticAnalysis() {
      this.gameObjectRules = new List<Rule<GameObject>>();
      this.componentRules = new Dictionary<Type, List<Rule>>();
    }

    public List<Rule> EnabledRules {
      get {
        var allRules = new List<Rule>();

        allRules.AddRange(this.gameObjectRules);

        foreach (var kvp in this.componentRules) {
          allRules.AddRange(kvp.Value);
        }

        return allRules;
      }
    }

    public void RegisterComponentRule(Type type, Rule rule) {
      if (!this.componentRules.ContainsKey(type)) {
        this.componentRules[type] = new List<Rule>();
      }

      this.componentRules[type].Add(rule);
    }

    public void RegisterRule<T>(Rule<T> rule) {
      this.RegisterComponentRule(typeof(T), rule);
    }

    public void RegisterRule(Type type, Rule rule) {
      if (type == typeof(GameObject)) {
        this.RegisterRule((Rule<GameObject>)rule);
      } else {
        this.RegisterComponentRule(type, rule);
      }
    }

    public void RegisterRule(Rule<GameObject> rule) {
      this.gameObjectRules.Add(rule);
    }

    public List<IViolation> Analyze(GameObject root) {
      var violations = new List<IViolation>();

      for (var j = 0; j < this.gameObjectRules.Count; j++) {
        var rule = this.gameObjectRules[j];
        violations.AddRange(rule.Handle(root));
      }

      foreach (var componentRuleSet in this.componentRules) {
        var rules = componentRuleSet.Value;

        for (var j = 0; j < rules.Count; j++) {
          var rule = rules[j];
          var components = root.GetComponents(componentRuleSet.Key);

          for (var i = 0; i < components.Length; i++) {
            var component = components[i];
            violations.AddRange(rule.Handle(component));
          }
        }
      }

      foreach (Transform child in root.transform) {
        violations.AddRange(this.Analyze(child.gameObject));
      }

      return violations;
    }
  }
}
