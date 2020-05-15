using UnityEngine;
using UnityEngine.Events;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Animator))]
public class CharacterController2D : MonoBehaviour {
  [SerializeField] private float _jumpForce = 400f; // Amount of force added when the player jumps.
  [SerializeField] private float _wallJumpForce = 100f; // Amount of force added when the player jumps form the wall.
  [Range (0, 1)][SerializeField] private float _crouchSpeed = .36f; // Amount of maxSpeed applied to crouching movement. 1 = 100%
  [Range (0, .3f)][SerializeField] private float _movementSmoothing = .05f; // How much to smooth out the movement
  [Range (0, 1)][SerializeField] private float _airborneSmoothing = .75f; //  How much to smooth out the airborne movement. 1 = 100%
  [Range (0, 1)][SerializeField] private float _wallSlidingVelocity = -1f; //  Slide velocity
  [SerializeField] private bool _airControl = true; // Whether or not a player can steer while jumping;
  [SerializeField] private LayerMask _groundLayer; // A mask determining what is ground to the character
  [SerializeField] private Transform _groundCheck; // A position marking where to check if the player is grounded.
  [SerializeField] private Transform _ceilingCheck; // A position marking where to check for ceilings
  [SerializeField] private Transform _leftSideCheck; // A position marking where to check for left side collisions
  [SerializeField] private Transform _rightSideCheck; // A position marking where to check for right side collisions
  [SerializeField] private Collider2D _crouchDisableCollider; // A collider that will be disabled when crouching

  const float groundedRadius = .05f; // Radius of the overlap circle to determine if grounded
  private bool _isGrounded; // Whether or not the player is grounded.
  const float ceilingRadius = .05f; // Radius of the overlap circle to determine if the player can stand up
  private bool _isLeftWallSliding;
  private bool _isRightWallSliding;
  const float sideRadius = .02f;
  private Rigidbody2D rb;
  private bool _isFacingRight = true; // For determining which way the player is currently facing.
  private Vector3 _velocity = Vector3.zero;

  Animator animator;

  [Header ("Events")]
  [Space]

  public UnityEvent OnLandEvent;

  [System.Serializable]
  public class BoolEvent : UnityEvent<bool> { }

  public BoolEvent OnCrouchEvent;
  private bool _wasCrouching = false;

  private void Awake () {
    rb = this.GetComponent<Rigidbody2D> ();
    animator = this.GetComponent<Animator> ();

    if (OnCrouchEvent == null)
      OnCrouchEvent = new BoolEvent ();
  }

  private void FixedUpdate () {
    bool wasGrounded = _isGrounded;
    _isGrounded = false;

    // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
    // This can be done using layers instead but Sample Assets will not overwrite your project settings.
    Collider2D[] colliders = Physics2D.OverlapCircleAll (_groundCheck.position, groundedRadius, _groundLayer);
    for (int i = 0; i < colliders.Length; i++) {
      if (colliders[i].gameObject != gameObject) {
        _isGrounded = true;
        if (!wasGrounded) {
          OnLandEvent.Invoke ();
          //  This should be in one of the functions called be the OnLandEvent
          //  But for now, since this is the only thing after landing, I will keep it here
          animator.SetBool ("Jump", false);
          break;
        }
      }
    }
  }

  public void Move (float move, bool crouch, bool jump) {
    // If crouching, check to see if the character can stand up
    if (!crouch) {
      // If the character has a ceiling preventing them from standing up, keep them crouching
      if (Physics2D.OverlapCircle (_ceilingCheck.position, ceilingRadius, _groundLayer)) {
        crouch = true;
      }
    }

    //only control the player if grounded or airControl is turned on
    if (_isGrounded || _airControl) {
      // If crouching
      if (crouch) {
        if (!_wasCrouching) {
          _wasCrouching = true;
          OnCrouchEvent.Invoke (true);
        }

        // Reduce the speed by the crouchSpeed multiplier
        move *= _crouchSpeed;

        // Disable one of the colliders when crouching
        if (_crouchDisableCollider != null)
          _crouchDisableCollider.enabled = false;
      } else {
        // Enable the collider when not crouching
        if (_crouchDisableCollider != null)
          _crouchDisableCollider.enabled = true;

        if (_wasCrouching) {
          _wasCrouching = false;
          OnCrouchEvent.Invoke (false);
        }
      }

      // Move the character by finding the target velocity
      Vector3 targetVelocity = new Vector2 (move, rb.velocity.y);
      // And then smoothing it out and applying it to the character
      float smoothness = _isGrounded ? _movementSmoothing : _airborneSmoothing;
      rb.velocity = Vector3.SmoothDamp (rb.velocity, targetVelocity, ref _velocity, smoothness);

      // If the input is moving the player right and the player is facing left...
      if (move > 0 && !_isFacingRight) {
        // ... flip the player.
        Flip ();
      }
      // Otherwise if the input is moving the player left and the player is facing right...
      else if (move < 0 && _isFacingRight) {
        // ... flip the player.
        Flip ();
      }
      animator.SetFloat ("Speed", Mathf.Abs (move));
    }
    Slide ();
    Jump (jump);
  }

  private void Flip () {
    // Switch the way the player is labelled as facing.
    _isFacingRight = !_isFacingRight;

    // Multiply the player's x local scale by -1.
    Vector3 theScale = transform.localScale;
    theScale.x *= -1;
    transform.localScale = theScale;
  }

  private void Jump (bool jump) {
    // If the player should jump...
    if (jump) {
      // Add a vertical force to the player.
      if (_isGrounded) {
        _isGrounded = false;
        animator.SetBool ("Jump", true);
        rb.AddForce (new Vector2 (0f, _jumpForce));
      } else if (_isLeftWallSliding || _isRightWallSliding) {
        animator.SetBool ("Jump", true);
        rb.AddForce (new Vector2 (_wallJumpForce * (_isLeftWallSliding ? -1 : 1), _jumpForce));
      }
    }
  }

  private void Slide () {
    _isLeftWallSliding = _isRightWallSliding = false;
    if (!_isGrounded && rb.velocity.y < 0) {
      _isLeftWallSliding = Physics2D.OverlapCircle (_leftSideCheck.position, sideRadius, _groundLayer);
      if (!_isLeftWallSliding) {
        _isRightWallSliding = Physics2D.OverlapCircle (_rightSideCheck.position, sideRadius, _groundLayer);
      }
      if (_isLeftWallSliding || _isRightWallSliding) {
        rb.velocity = new Vector2 (rb.velocity.x, _wallSlidingVelocity);
      }
    }
  }
}