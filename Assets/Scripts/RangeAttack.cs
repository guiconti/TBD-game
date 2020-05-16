using UnityEngine;
using UnityEngine.Events;

public class RangeAttack : Attack {
  public int projectileSpeed;

  public void Awake() {
    DirectionReversed.AddListener(ReverseSpeed);
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Enemy")) {
      HitEvent.Invoke(other.gameObject);
      Destroy();
    } else if (other.CompareTag("Wall")) {
      Destroy();
    }
  }

  public void ReverseSpeed() {
    projectileSpeed *= -1;
  }
}