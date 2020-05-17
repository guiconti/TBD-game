using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController2D))]
public class PlayerAttack : MonoBehaviour {
  private bool _canSlash = true;
  [SerializeField] private Transform _slashPosition;
  [SerializeField] private GameObject _slashObject;
  [SerializeField] private Transform _shootPosition;
  [SerializeField] private GameObject _energyBallObject;
  [SerializeField] private GameObject _freezeBallObject;
  private int _freezeBallMax = 20;
  private Animator animator;
  private CharacterController2D controller;
  private List<FreezeBall> _freezeBalls = new List<FreezeBall>();

  void Start () {
    animator = this.GetComponent<Animator>();
    controller = this.GetComponent<CharacterController2D>();
  }

  void Update () {
    ReadInput();
  }

  private void ReadInput() {
    if (!controller.IsWallSliding() && !controller.IsCrouching()) {
      if (Input.GetButtonDown("Slash")) {
        Slash();
      } else if (Input.GetButtonDown("Shoot")) {
        ShootEnergyBall();
      } else if (Input.GetButtonDown("ShootFreeze")) {
        ShootFreezeBall();
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

  private void ShootEnergyBall() {
    animator.SetTrigger("Shoot");
    InstantiateAttack(_energyBallObject, _shootPosition.position);
  }

  private void ShootFreezeBall() {
    animator.SetTrigger("FreezeBall");
    GameObject freezeBallGameObject = InstantiateAttack(_freezeBallObject, _shootPosition.position);
    if (_freezeBalls.Count >= _freezeBallMax) {
      _freezeBalls.RemoveAt(0);
    }
    _freezeBalls.Add(freezeBallGameObject.GetComponent<FreezeBall>());
  }
}