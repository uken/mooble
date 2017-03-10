#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mooble.StaticAnalysis {
  public class StaticAnalysisBuilder {
    private Mooble.Config.Config config;

    public StaticAnalysisBuilder(Mooble.Config.Config config) {
      this.config = config;
    }

    public StaticAnalysis Get() {
      var sa = new StaticAnalysis();

      for (var i = 0; i < this.config.Rules.Length; i++) {
        var ruleConfig = this.config.Rules[i];
        var klassName = ruleConfig.Name;
        Assembly assembly = typeof(StaticAnalysis).Assembly;

        if (string.IsNullOrEmpty(ruleConfig.Assembly)) {
          var ns = "Mooble.StaticAnalysis.Rules.";

          if (!klassName.StartsWith(ns, StringComparison.CurrentCulture)) {
            klassName = ns + klassName;
          }
        } else {
          assembly = Assembly.Load(ruleConfig.Assembly);
        }

        Type ruleClass = assembly.GetType(klassName);

        if (ruleClass == null) {
          Log.Debug("Could not find rule class: " + klassName + ". Skipping.");
          continue;
        }

        if (!ruleClass.BaseType.IsGenericType || ruleClass.BaseType.BaseType != typeof(Rule)) {
          Log.Debug("Rule " + klassName + " did not extend Rule class two levels up; parent was not generic type. Skipping.");
          continue;
        }

        var level = (ViolationLevel)Enum.Parse(typeof(ViolationLevel), ruleConfig.ViolationLevel);
        var scope = (ViolationScope)Enum.Parse(typeof(ViolationScope), ruleConfig.ViolationScope);
        List<Type> excludedTypes = this.GetExcludedTypes(ruleConfig.Exclusions);

        var rule = this.ConstructRule(ruleClass, level, scope, excludedTypes);
        var ruleObjectType = ruleClass.BaseType.GetGenericArguments()[0];

        sa.RegisterRule(ruleObjectType, rule);
      }

      return sa;
    }

    private Rule ConstructRule(Type ruleClass, ViolationLevel level, ViolationScope scope, List<Type> excludedTypes) {
      if (excludedTypes == null || excludedTypes.Count == 0) {
        ConstructorInfo ctor = ruleClass.GetConstructor(new[] {
          typeof(ViolationLevel),
          typeof(ViolationScope)
        });
        return (Rule)ctor.Invoke(new object[] { level, scope });
      } else {
        ConstructorInfo ctor = ruleClass.GetConstructor(new[] {
          typeof(ViolationLevel),
          typeof(ViolationScope),
          typeof(List<Type>)
        });

        if (ctor == null) {
          Log.Debug("Could not find constructor for " + ruleClass +
              " that takes list of excluded types. Using default constructor.");
          ctor = ruleClass.GetConstructor(new[] { typeof(ViolationLevel), typeof(ViolationScope) });
          return (Rule)ctor.Invoke(new object[] { level, scope });
        }

        return (Rule)ctor.Invoke(new object[] { level, scope, excludedTypes });
      }
    }

    private List<Type> GetExcludedTypes(string[] excludedTypeNames) {
      var excludedTypes = new List<Type>();

      for (var i = 0; i < excludedTypeNames.Length; i++) {
        Type type = Type.GetType(excludedTypeNames[i]);

        if (type == null) {
          Log.Debug("Not including exclusion of type " + excludedTypeNames[i] + "; could not find type.");
        } else {
          excludedTypes.Add(type);
        }
      }

      return excludedTypes;
    }
  }
}
#endif
