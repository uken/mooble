#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace Mooble.StaticAnalysis.Violation {
  public class NoImageWithMissingSpriteViolation : IViolation {
    private Image image;

    public NoImageWithMissingSpriteViolation(ViolationLevel level, Image image) {
      this.Level = level;
      this.image = image;
    }

    public ViolationLevel Level { get; set; }

    public string FormatCLI() {
      return string.Format(
        "{2} {1}: Image {0} has a missing sprite reference.",
        this.image.name,
        Utility.FormatObjectPath(this.image.gameObject),
        this.Level.ToString().ToUpper());
    }

    public string FormatEditor() {
      return string.Format(
        "Image {0} has a missing sprite reference.",
        Utility.FormatPrimaryObject(this.image.name));
    }

    public UnityEngine.Object GetObject() {
      return this.image.gameObject;
    }
  }
}
#endif
