using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Analytics;
using Unity.VisualScripting;
using System;

/// <summary>
/// Manages the movement of the player.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private CharacterAttackManager _myAttack;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private TauntRandom _taunt;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _orientation;

    [Header("Movement Variables")]
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _airSpeed;
    [SerializeField] private float _minAirSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private float _timeSinceLastJump;
    [SerializeField] private bool _countJumpTime;

    [Header("Keybinds")]
    [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode _runKey = KeyCode.LeftShift;

    [Header("Input")]
    [SerializeField] private float _horizontalInput;
    [SerializeField] private float _verticalInput;

    [Header("Misc")]
    [SerializeField] private Vector3 _moveDirection;

    private void Awake() {
        _myState = GetComponent<CharacterStateManager>();
        _myAttack = GetComponent<CharacterAttackManager>();
        _anim = transform.Find("MyObj").GetComponent<Animator>();
        _taunt = GetComponent<TauntRandom>();
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
        _orientation = transform.Find("Orientation");

        _timeSinceLastJump = _jumpCooldown;
        _airSpeed = _minAirSpeed;
    }

    private void Update() {

        // Check for player input every frame
        PlayerInput();
    }

    private void FixedUpdate() {


        if (_countJumpTime && _timeSinceLastJump < _jumpCooldown) {
            _timeSinceLastJump += Time.deltaTime;
        } else {
            _countJumpTime = false;
        }

        MovePlayer();
        SpeedControl();

    }

    /// <summary>
    /// 
    /// Checks for player input and is meant to be
    /// called within Update().
    /// 
    /// Determines whether the character should be idle,
    /// walking, running, or jumping, and changes the
    /// character state action accordingly.
    /// 
    /// </summary>
    private void PlayerInput() {

        // If the player is in a normal state and not currently attacking
        if (_myState.GetAbleState() == CharacterStateManager.AbleState.Normal
            && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Attacking) {

            // Get the movement input from the player
            // (whether there is any or not)
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            // If the player is pressing movement keys while NOT falling
            if ((Mathf.Abs(_horizontalInput) > 0.1f
                    || Mathf.Abs(_verticalInput) > 0.1f)
                    && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Falling) {

                // Reset the tracked time for the idle taunt
                _taunt.ResetTimeSinceLastMovement();

                // If the player is holding down the run key,
                // set action to run if it's not set already
                if (Input.GetKey(_runKey)) {
                    // if (_myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Running) {
                        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Running);
                    // }
                }

                // If the player is NOT holding down the run key,
                // set action to walking if it's not set already
                else {
                    // if (_myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Walking) {
                        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Walking);
                    // }
                }

            // If the player's not falling (and not pressing movement keys)
            } else if (_myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Falling) {

                // If the player's not already idling and not taunting,
                // set current action to idle
                if (_myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Idle
                    && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Taunt
                    // && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Falling
                    ) {
                    _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);
                }

            }

        } 

        // If the player was not in a normal state
        // or was attacking, set the movement input to 0
        else {

            _horizontalInput = 0;
            _verticalInput = 0;

        }

        if (Input.GetKeyDown(_jumpKey)) Debug.Log("Jump Key");
        // If the player presses the jump key and meets
        // all criteria to jump, including passing the jump cooldown,
        // play the Jump() method
        if (Input.GetKeyDown(_jumpKey)
            && _myState.GetAbleState() == CharacterStateManager.AbleState.Normal
            && _myState.GetIsGrounded()
            && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Attacking
            && _timeSinceLastJump >= _jumpCooldown) {
                Jump();
        }

        // Debug.Log("X / Y : " + _horizontalInput + " " + _verticalInput);

    }

    /// <summary>
    /// 
    /// Method for applying force to the player in order
    /// to move it.
    /// 
    /// Meant to be called within FixedUpdate().
    /// 
    /// </summary>
    private void MovePlayer() {

        // Calculate the player movement direction
        _moveDirection = _orientation.forward * _verticalInput + _orientation.right * _horizontalInput;
        
        // Only apply force if the player is in a normal state,
        // and apply different forces based on the current action,
        // i.e. running, walking, jumping, and falling
        if (_myState.GetAbleState() == CharacterStateManager.AbleState.Normal) {

            if (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running) {

                _rb.AddForce(_moveDirection.normalized * _runSpeed * 10f, ForceMode.Force);

            } else if (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking) {

                _rb.AddForce(_moveDirection.normalized * _walkSpeed * 10f, ForceMode.Force);

            } else if (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Jumping
                        || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Falling) {

                _rb.AddForce(_moveDirection.normalized * _airSpeed * 10f, ForceMode.Force);

            }

        }
        
    }

    /// <summary>
    /// 
    /// Used to limit the maxmimum velocity of the player.
    /// Since the player has constant force being applied
    /// to them every frame, we need to prevent it from
    /// getting too high.
    /// 
    /// </summary>
    private void SpeedControl() {

        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        // If the player is currently running, limit the
        // speed to the maxmimum running speed
        if (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running) {

            if (flatVel.magnitude > _runSpeed) {
                Vector3 limitedVel = flatVel.normalized * _runSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }

        }

        // If the player is currently jumping or falling,
        // limit the speed to the maxmimum running speed
        else if (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Falling
                || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Jumping) {

            if (flatVel.magnitude > _airSpeed) {
                Vector3 limitedVel = flatVel.normalized * _airSpeed / 3;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }

        }

        // If the player is currently walking, limit the
        // speed to the maxmimum walking speed
        else {

            if (flatVel.magnitude > _walkSpeed) {
                Vector3 limitedVel = flatVel.normalized * _walkSpeed;
                _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
            }

        }
    }

    /// <summary>
    /// 
    /// The sole method for initiating a player
    /// jump, switching to the jump action and adding
    /// upwards force to the player.
    /// 
    /// </summary>
    private void Jump() {

        // Debug.Log("Jump");

        Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        
        if (flatVel.magnitude > _minAirSpeed) _airSpeed = flatVel.magnitude;
        else _airSpeed = _minAirSpeed;

        // Start Jump
        if (_myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Running) {
            switch (_myAttack.GetWeaponState()) {
                case CharacterAttackManager.WeaponState.Sword:
                    _anim.Play("Sword_Jump");
                    break;
                case CharacterAttackManager.WeaponState.GreatSword:
                    _anim.Play("GreatSword_Jump");
                    break;
            }
        }
        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Jumping);
        _rb.AddForce(transform.up * _jumpForce, ForceMode.Force);
        _myState.SetIsGrounded(false);
        _timeSinceLastJump = 0;
        _countJumpTime = true;

    }

}