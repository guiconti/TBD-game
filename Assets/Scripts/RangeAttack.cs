using UnityEngine;

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

  private void OnCollisionEnter2D(Collision2D other) {
    if (other.collider.CompareTag("Enemy")) {
      HitEvent.Invoke(other.gameObject);
      Destroy();
    } else if (other.collider.CompareTag("Wall") || other.collider.CompareTag("Ground")) {
      Destroy();
    }
  }

  public void ReverseSpeed() {
    projectileSpeed *= -1;
  }
}