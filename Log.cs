#if UNITY_EDITOR
using System.Diagnostics.CodeAnalysis;

namespace Mooble {
  public static class Log {
    [SuppressMessage("Unity.StyleCop.CSharp.DebugLogRule", "UN1000:DebugLogsShouldBeRemoved", Justification = "This is the place where we log.")]
    public static void Warning(string message, UnityEngine.Object obj) {
      UnityEngine.Debug.LogWarning(message, obj);
    }

    [SuppressMessage("Unity.StyleCop.CSharp.DebugLogRule", "UN1000:DebugLogsShouldBeRemoved", Justification = "This is the place where we log.")]
    public static void Debug(string message) {
      UnityEngine.Debug.Log(message);
    }
  }
}
#endif
