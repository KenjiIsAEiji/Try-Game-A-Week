﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveInput : MonoBehaviour
{
    [SerializeField] PlayerController player;

    PlayerInputAction input;

    private void Awake() => input = new PlayerInputAction();
    private void OnDestroy() => input.Disable();
    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void Update()
    {
        player.Velocity = input.Player.Move.ReadValue<Vector2>();
        player.LookPosition = input.Player.Look.ReadValue<Vector2>();
        player.FireFlag = input.Player.Fire.ReadValue<float>() > InputSystem.settings.defaultButtonPressPoint;
    }
   
}
