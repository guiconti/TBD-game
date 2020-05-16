using UnityEngine;
using UnityEngine.Events;

public class Attack : MonoBehaviour {
  public class GameObjectEvent : UnityEvent<GameObject> { }
  public GameObjectEvent HitEvent;

  private void Awake() {
    if (HitEvent == null) {
      HitEvent = new GameObjectEvent ();
    }
  }

  private void OnTriggerEnter(Collider other) {
    if (other.CompareTag("Enemy")) {
      HitEvent.Invoke(other.gameObject);
    }
  }

  public void Destroy() {
    GameObject.Destroy(this.gameObject);
  }
}