#if UNITY_EDITOR
using UnityEngine;

namespace Mooble.StaticAnalysis.Violation {
  public class NoInactiveBehavioursViolation : IViolation {
    private Component component;

    public NoInactiveBehavioursViolation(ViolationLevel level, Component c) {
      this.Level = level;
      this.component = c;
    }

    public ViolationLevel Level { get; set; }

    public string FormatCLI() {
      return string.Format(
        "{0}: Behaviour {1} is inactive.",
        Utility.FormatObjectPath(this.component),
        this.component.GetType());
    }

    public string FormatEditor() {
      return string.Format(
        "Behaviour {1} is inactive on {0}.",
        Utility.FormatPrimaryObject(this.component.name),
        Utility.FormatSecondaryObject(this.component.GetType().ToString()));
    }

    public UnityEngine.Object GetObject() {
      return this.component;
    }
  }
}
#endif
