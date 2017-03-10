using UnityEngine;
using System.Collections;

public class Something : MonoBehaviour {
  [SerializeField] private Animator animator;

  public float Beans {
    get {
      return this.animator.GetCurrentAnimatorStateInfo(0).length;
    }
  }

}
