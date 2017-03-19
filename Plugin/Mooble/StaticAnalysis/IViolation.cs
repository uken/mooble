#if UNITY_EDITOR
namespace Mooble.StaticAnalysis {
  public interface IViolation {
    ViolationLevel Level { get; set; }

    string FormatCLI();

    string FormatEditor();

    UnityEngine.Object GetObject();
  }
}
#endif
