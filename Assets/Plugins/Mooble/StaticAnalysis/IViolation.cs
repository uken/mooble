namespace Mooble.StaticAnalysis {
  public interface IViolation {
    string Format();

    UnityEngine.Object GetObject();
  }
}
