using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnergyBall : RangeAttack {
  Rigidbody2D rb;
  // Start is called before the first frame update
  void Start () {
    rb = this.GetComponent<Rigidbody2D>();
    rb.velocity = new Vector2(projectileSpeed, 0f);
  }

  private void CalculateVelocity() {
    rb.velocity = new Vector2(projectileSpeed, 0f);
  }  
}