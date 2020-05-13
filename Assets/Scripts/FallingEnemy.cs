using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof (Rigidbody2D))]
public class FallingEnemy : MonoBehaviour {
  Rigidbody2D rb;
  Animator animator;
  [SerializeField] private Transform _groundCheck;
  private bool _isFalling = false;

  private bool _isGrounded = false;
  private bool _wasGrounded = false;
  const float groundedRadius = .2f;
  [SerializeField] private LayerMask _groundLayer;

  private UnityEvent OnLandEvent = new UnityEvent();

  // Start is called before the first frame update
  void Start () {
    rb = this.GetComponent<Rigidbody2D> ();
    rb.isKinematic = true;
    animator = this.GetComponent<Animator> ();
    OnLandEvent.AddListener (Landed);
  }

  // Update is called once per frame
  void Update () {
    if (Input.GetKeyDown ("f")) {
      rb.isKinematic = false;
      _isFalling = true;
      animator.SetBool ("Falling", true);
    }
  }

  void FixedUpdate () {
    if (_isFalling) {
      Collider2D[] colliders = Physics2D.OverlapCircleAll (_groundCheck.position, groundedRadius, _groundLayer);
      for (int i = 0; i < colliders.Length; i++) {
        if (colliders[i].gameObject != gameObject) {
          OnLandEvent.Invoke ();
        }
      }
    }
  }

  void Landed () {
    _isFalling = false;
    animator.SetBool ("Falling", false);
    // rb.isKinematic = true;
  }
}