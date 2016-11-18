namespace Mooble.StaticAnalysis {
  public interface IViolation {
    ViolationLevel Level { get; set; }

    string Format();

    UnityEngine.Object GetObject();
  }
}
