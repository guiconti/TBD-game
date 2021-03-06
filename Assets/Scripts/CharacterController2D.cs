﻿using UnityEngine;
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
  [SerializeField] private Transform _sideCheck; // A position marking where to check for right side collisions
  [SerializeField] private Collider2D _crouchDisableCollider; // A collider that will be disabled when crouching

  const float groundedRadius = .05f; // Radius of the overlap circle to determine if grounded
  private bool _isGrounded; // Whether or not the player is grounded.
  const float ceilingRadius = .05f; // Radius of the overlap circle to determine if the player can stand up
  private bool _isWallSliding;
  const float sideRadius = .02f;
  private Rigidbody2D rb;
  [HideInInspector]
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
    if (OnCrouchEvent == null) {
      OnCrouchEvent = new BoolEvent ();
    }
  }

  private void FixedUpdate () {
    bool wasGrounded = _isGrounded;
    _isGrounded = false;

    Collider2D[] colliders = Physics2D.OverlapCircleAll (_groundCheck.position, groundedRadius, _groundLayer);
    for (int i = 0; i < colliders.Length; i++) {
      if (colliders[i].gameObject != gameObject) {
        _isGrounded = true;
        if (!wasGrounded && rb.velocity.y < 0) {
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
    if (!crouch && _wasCrouching) {
      // If the character has a ceiling preventing them from standing up, keep them crouching
      if (Physics2D.OverlapCircle (_ceilingCheck.position, ceilingRadius, _groundLayer)) {
        crouch = true;
      }
    }

    //only control the player if grounded or airControl is turned on
    if (_isGrounded || _airControl) {
      if (crouch) {
        if (!_wasCrouching) {
          _wasCrouching = true;
          OnCrouchEvent.Invoke (true);
          //  Should use the event
          animator.SetBool("Crouching", true);
        }
        move *= _crouchSpeed;
        if (_crouchDisableCollider != null) {
          _crouchDisableCollider.enabled = false;
        }
      } else {
        if (_crouchDisableCollider != null) {
          _crouchDisableCollider.enabled = true;
        }
        if (_wasCrouching) {
          _wasCrouching = false;
          OnCrouchEvent.Invoke (false);
          //  Should use the event
          animator.SetBool("Crouching", false);
        }
      }

      // Move the character by finding the target velocity
      Vector3 targetVelocity = new Vector2 (move, rb.velocity.y);
      // And then smoothing it out and applying it to the character
      float smoothness = _isGrounded ? _movementSmoothing : _airborneSmoothing;
      rb.velocity = Vector3.SmoothDamp (rb.velocity, targetVelocity, ref _velocity, smoothness);

      if (move > 0 && !_isFacingRight) {
        Flip ();
      }
      else if (move < 0 && _isFacingRight) {
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
    if (jump) {
      if (_isGrounded) {
        _isGrounded = false;
        animator.SetBool ("Jump", true);
        rb.AddForce (new Vector2 (0f, _jumpForce));
      } else if (_isWallSliding) {
        animator.SetBool ("Jump", true);
        rb.AddForce (new Vector2 (_wallJumpForce * (_isFacingRight ? -1 : 1), _jumpForce));
      }
    }
  }

  private void Slide () {
    _isWallSliding = false;
    if (!_isGrounded && rb.velocity.y < 0) {
      _isWallSliding = Physics2D.OverlapCircle (_sideCheck.position, sideRadius, _groundLayer);
      if (_isWallSliding) {
        rb.velocity = new Vector2 (rb.velocity.x, _wallSlidingVelocity);
      }
    }
    animator.SetBool ("WallSliding", _isWallSliding);
  }

  //  Public Getters
  public bool IsFacingRight() {
    return _isFacingRight;
  }

  public bool IsWallSliding() {
    return _isWallSliding;
  }

  public bool IsCrouching() {
    return _wasCrouching;
  }
}