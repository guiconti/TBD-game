using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController2D))]
public class PlayerAttack : MonoBehaviour {
  private bool _canSlash = true;
  [SerializeField] private Transform _slashPosition;
  [SerializeField] private GameObject _slashObject;
  private Animator animator;
  private CharacterController2D controller;
  // Start is called before the first frame update
  void Start () {
    animator = this.GetComponent<Animator>();
    controller = this.GetComponent<CharacterController2D>();
  }

  // Update is called once per frame
  void Update () {
    ReadInput();
  }

  private void ReadInput() {
    if (Input.GetButtonDown("Slash") && !controller.IsWallSliding() && !controller.IsCrouching()) {
      Slash();
    }
  }

  private void Slash() {
    animator.SetTrigger("Slash");
    GameObject slashInstatiation = GameObject.Instantiate(_slashObject, _slashPosition.position, Quaternion.identity);
    if (!controller.IsFacingRight()) {
      Vector3 theScale = slashInstatiation.transform.localScale;
      theScale.x *= -1;
      slashInstatiation.transform.localScale = theScale;
    }
  }
}