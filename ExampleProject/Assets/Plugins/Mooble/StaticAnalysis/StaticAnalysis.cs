#if UNITY_EDITOR
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

    public List<string> EnabledRuleNames {
      get {
        var allRules = new List<string>();

        for (var i = 0; i < this.gameObjectRules.Count; i++) {
          allRules.Add(this.gameObjectRules[i].Name);
        }

        foreach (var kvp in this.componentRules) {
          for (var i = 0; i < kvp.Value.Count; i++) {
            allRules.Add(kvp.Value[i].Name);
          }
        }

        return allRules;
      }
    }

    public static Dictionary<Rule, List<IViolation>> MergeRuleViolationDictionary(Dictionary<Rule, List<IViolation>> violations1, Dictionary<Rule, List<IViolation>> violations2) {
      foreach (var more in violations2) {
        if (violations1.ContainsKey(more.Key)) {
          violations1[more.Key].AddRange(more.Value);
        } else {
          violations1[more.Key] = more.Value;
        }
      }

      return violations1;
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

    public Dictionary<Rule, List<IViolation>> Analyze(ViolationScope scope, GameObject root) {
      var violations = new Dictionary<Rule, List<IViolation>>();

      if (root == null) {
        return violations;
      }

      for (var j = 0; j < this.gameObjectRules.Count; j++) {
        var rule = this.gameObjectRules[j];
        if (!rule.ValidForScope(scope)) {
          continue;
        }

        violations[rule] = rule.Handle(root);
      }

      foreach (var componentRuleSet in this.componentRules) {
        var rules = componentRuleSet.Value;

        for (var j = 0; j < rules.Count; j++) {
          var rule = rules[j];
          if (!rule.ValidForScope(scope)) {
            continue;
          }

          var components = root.GetComponents(componentRuleSet.Key);
          violations[rule] = new List<IViolation>();

          for (var i = 0; i < components.Length; i++) {
            var component = components[i];
            violations[rule].AddRange(rule.Handle(component));
          }
        }
      }

      foreach (Transform child in root.transform) {
        var moreViolations = this.Analyze(scope, child.gameObject);

        violations = MergeRuleViolationDictionary(violations, moreViolations);
      }

      return violations;
    }
  }
}
#endif
