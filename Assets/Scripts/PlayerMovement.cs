using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController2D))]
public class PlayerMovement : MonoBehaviour {
  private CharacterController2D _controller;
  private float _inputX;
  private bool _jump;

  public int movementSpeed = 50;

  void Start () {
    _controller = this.GetComponent<CharacterController2D>();
  }

  void Update () {
    ReadInput();
  }

  void FixedUpdate() {
    _controller.Move(_inputX * Time.fixedDeltaTime, false, _jump);
    _jump = false;
  }

  private void ReadInput() {
    _inputX = Input.GetAxisRaw("Horizontal") * movementSpeed;
    if (Input.GetButtonDown("Jump")) {
      _jump = true;
    }
  }
}