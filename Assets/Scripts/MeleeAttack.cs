using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack {
  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Enemy")) {
      HitEvent.Invoke(other.gameObject);
    }
  }
}