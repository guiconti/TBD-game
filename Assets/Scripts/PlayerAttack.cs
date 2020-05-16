using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController2D))]
public class PlayerAttack : MonoBehaviour {
  private bool _canSlash = true;
  [SerializeField] private Transform _slashPosition;
  [SerializeField] private GameObject _slashObject;
  [SerializeField] private Transform _shootPosition;
  [SerializeField] private GameObject _shootObject;
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
    if (!controller.IsWallSliding() && !controller.IsCrouching()) {
      if (Input.GetButtonDown("Slash")) {
        Slash();
      } else if (Input.GetButtonDown("Shoot")) {
        Shoot();
      }
    }
  }

  private GameObject InstantiateAttack(GameObject gameObject, Vector3 position) {
    GameObject attackInstatiation = GameObject.Instantiate(gameObject, position, Quaternion.identity);
    if (!controller.IsFacingRight()) {
      attackInstatiation.GetComponent<Attack>().ReverseDirection();
    }
    return attackInstatiation;
  }

  private void Slash() {
    animator.SetTrigger("Slash");
    InstantiateAttack(_slashObject, _slashPosition.position);
  }

  private void Shoot() {
    animator.SetTrigger("Shoot");
    InstantiateAttack(_shootObject, _shootPosition.position);
  }
}