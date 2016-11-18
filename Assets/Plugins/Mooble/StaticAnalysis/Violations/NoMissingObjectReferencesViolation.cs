#if UNITY_EDITOR
using UnityEngine;

namespace Mooble.StaticAnalysis.Violation {
  public class NoMissingObjectReferencesViolation : IViolation {
    private Component component;
    private string fieldName;

    public NoMissingObjectReferencesViolation(ViolationLevel level, Component c, string n) {
      this.component = c;
      this.Level = level;
      this.fieldName = n;
    }

    public ViolationLevel Level { get; set; }

    public string FormatCLI() {
      return string.Format(
        "{0}: Field {2} from Component {1} is undefined.",
        Utility.FormatObjectPath(this.component.gameObject),
        this.component.GetType(),
        this.fieldName);
    }

    public string FormatEditor() {
      return string.Format(
        "Field {2} from Component {1} on {0} is undefined.",
        Utility.FormatPrimaryObject(this.component.gameObject.name),
        Utility.FormatSecondaryObject(this.component.GetType().ToString()),
        Utility.FormatTertiaryObject(this.fieldName));
    }

    public UnityEngine.Object GetObject() {
      return this.component;
    }
  }
}
#endif
