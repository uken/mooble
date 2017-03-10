#if UNITY_EDITOR
using System.Text;
using UnityEngine;

namespace Mooble.StaticAnalysis {
  public static class Utility {
    public static string FormatObjectPath(Component component) {
      return FormatObjectPath(component.gameObject);
    }

    public static string FormatObjectPath(GameObject obj) {
      var path = new StringBuilder();

      var transform = obj.transform;
      path.Append(transform.name);

      while (transform.parent != null) {
        transform = transform.parent;
        path.Insert(0, "/");
        path.Insert(0, transform.name);
      }

      path.Insert(0, "/");

      return path.ToString();
    }

    public static string FormatPrimaryObject(string obj) {
      return "<color=#ff9100><b>" + obj + "</b></color>";
    }

    public static string FormatSecondaryObject(string obj) {
      return "<color=#ffc400><b>" + obj + "</b></color>";
    }

    public static string FormatTertiaryObject(string obj) {
      return "<color=#ffea00><b>" + obj + "</b></color>";
    }
  }
}
#endif
