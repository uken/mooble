using System.Diagnostics.CodeAnalysis;

public static class Log {
  [SuppressMessage("Unity.StyleCop.CSharp.DebugLogRule", "UN1000:DebugLogsShouldBeRemoved", Justification = "This is the place where we log.")]
  public static void Warning(string message) {
    UnityEngine.Debug.LogWarning(message);
  }

  [SuppressMessage("Unity.StyleCop.CSharp.DebugLogRule", "UN1000:DebugLogsShouldBeRemoved", Justification = "This is the place where we log.")]
  public static void Debug(string message) {
    UnityEngine.Debug.Log(message);
  }
}
