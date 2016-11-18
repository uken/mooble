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

    public string Format() {
      return string.Format(
        "There are {0} {1} scripts on {2}",
        this.count,
        this.componentName,
        this.gameObject.name);
    }

    public UnityEngine.Object GetObject() {
      return this.gameObject;
    }
  }
}
