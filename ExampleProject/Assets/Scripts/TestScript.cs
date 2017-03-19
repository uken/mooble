using UnityEngine;

public class TestScript : MonoBehaviour {
  [SerializeField] private double serialized;
  [SerializeField] private SomeClass someClass = null;

  public int publicDoom;
  private int unserializedBeans;
  private string unserializedCows;

  public class SomeClass {
  }

  void Start() {

  }

}
