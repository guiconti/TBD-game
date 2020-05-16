using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour {
  public class GameObjectEvent : UnityEvent<GameObject> { }
  public GameObjectEvent HitEvent;
  public UnityEvent DirectionReversed;

  private void Awake() {
    if (HitEvent == null) {
      HitEvent = new GameObjectEvent ();
    }
  }

  public void Destroy() {
    GameObject.Destroy(this.gameObject);
  }

  public void ReverseDirection() {
    Vector3 theScale = transform.localScale;
    theScale.x *= -1;
    transform.localScale = theScale;
    DirectionReversed.Invoke();
  }
}