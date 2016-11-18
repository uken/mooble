#if UNITY_EDITOR
using UnityEngine;

namespace Mooble.StaticAnalysis.Violation {
  public class NoDuplicateComponentsViolation : IViolation {
    private GameObject gameObject;
    private string componentName;
    private int count;

    public NoDuplicateComponentsViolation(ViolationLevel level, GameObject o, string c, int n) {
      this.Level = level;
      this.gameObject = o;
      this.componentName = c;
      this.count = n;
    }

    public ViolationLevel Level { get; set; }

    public string FormatCLI() {
      return string.Format(
        "{2}: There are multiple ({0}) '{1}' Components.",
        this.count,
        this.componentName,
        Utility.FormatObjectPath(this.gameObject));
    }

    public string FormatEditor() {
      return string.Format(
        "There are multiple ({0}) {1} Components on {2}.",
        this.count,
        Utility.FormatSecondaryObject(this.componentName),
        Utility.FormatPrimaryObject(this.gameObject.name));
    }

    public UnityEngine.Object GetObject() {
      return this.gameObject;
    }
  }
}
#endif
