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
    [Header("References")]
    [SerializeField] private CharacterStateManager myState;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator anim;
    [SerializeField] private TauntRandom taunt;

    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float airSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float landTime;
    [SerializeField] private float timeSinceLanding;
    [SerializeField] private bool countJumpTime;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    [Header("Input")]
    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;

    [Header("Misc")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Vector3 moveDirection;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update() {
        // Check for player input every frame
        PlayerInput();
    }

    private void FixedUpdate() {


        if (countJumpTime && timeSinceLanding < jumpCooldown) {
            timeSinceLanding += Time.deltaTime;
        } else {
            countJumpTime = false;
        }

        // If I'm NOT Grounded
        if (!myState.GetIsGrounded() && rb.velocity.y < -1f) {

            if (myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Falling) {
                myState.CurrentActionChange(CharacterStateManager.CurrentAction.Falling);    
            }

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
        if (myState.GetAbleState() == CharacterStateManager.AbleState.Normal
            && myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Attacking) {

            // Get the movement input from the player
            // (whether there is any or not)
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            // If the player is pressing movement keys while NOT falling
            if ((Mathf.Abs(horizontalInput) > 0.1f
                    || Mathf.Abs(verticalInput) > 0.1f)
                    && myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Falling) {

                // Reset the tracked time for the idle taunt
                taunt.ResetTimeSinceLastMovement();

                // If the player is holding down the run key,
                // set action to run if it's not set already
                if (Input.GetKey(runKey)) {
                    if (myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Running) {
                        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Running);
                    }
                }

                // If the player is NOT holding down the run key,
                // set action to walking if it's not set already
                else {
                    if (myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Walking) {
                        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Walking);
                    }
                }

            // If the player's not falling (and not pressing movement keys)
            } else if (myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Falling) {

                // If the player's not already idling and not taunting,
                // set current action to idle
                if (myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Idle
                    && myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Taunt
                    // && myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Falling
                    ) {
                    myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);
                }

            }

        } 

        // If the player was not in a normal state
        // or was attacking, set the movement input to 0
        else {

            horizontalInput = 0;
            verticalInput = 0;

        }

        // If the player presses the jump key and meets
        // all criteria to jump, including passing the jump cooldown,
        // play the Jump() method
        if (Input.GetKeyDown(jumpKey)
            && myState.GetAbleState() == CharacterStateManager.AbleState.Normal
            && myState.GetIsGrounded()
            && myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Attacking
            && timeSinceLanding >= jumpCooldown) {
                Jump();
        }

        // Debug.Log("X / Y : " + horizontalInput + " " + verticalInput);

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
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        // Only apply force if the player is in a normal state,
        // and apply different forces based on the current action,
        // i.e. running, walking, jumping, and falling
        if (myState.GetAbleState() == CharacterStateManager.AbleState.Normal) {

            if (myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running) {

                rb.AddForce(moveDirection.normalized * runSpeed * 10f, ForceMode.Force);

            } else if (myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking) {

                rb.AddForce(moveDirection.normalized * walkSpeed * 10f, ForceMode.Force);

            } else if (myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Jumping
                        || myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Falling) {

                rb.AddForce(moveDirection.normalized * airSpeed * 10f, ForceMode.Force);

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

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // If the player is currently running, limit the
        // speed to the maxmimum running speed
        if (myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running) {

            if (flatVel.magnitude > runSpeed) {
                Vector3 limitedVel = flatVel.normalized * walkSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }

        }
        // If the player is currently walking, limit the
        // speed to the maxmimum walking speed
        else {

            if (flatVel.magnitude > walkSpeed) {
                Vector3 limitedVel = flatVel.normalized * walkSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
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

        Debug.Log("Jump");

        // Start Jump
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Jumping);
        anim.Play("Jump");
        rb.AddForce(transform.up * jumpForce, ForceMode.Force);
        myState.SetIsGrounded(false);
        timeSinceLanding = 0;

    }

    /// <summary>
    /// 
    /// Initiates a landing sequence for the player,
    /// changing its able state and current actions until
    /// the duration of the landing is over.
    /// 
    /// Run whenever the character lands on the ground
    /// or a surface.
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Land() {

        // Debug.Log("Touched Ground");
        
        myState.SetIsGrounded(true);
        myState.AbleStateChange(CharacterStateManager.AbleState.Incapacitated);
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Landing);
        
        if (timeSinceLanding == 0) {
            countJumpTime = true;
        }

        yield return new WaitForSeconds(landTime);
        myState.AbleStateChange(CharacterStateManager.AbleState.Normal);
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);

    }

    private void OnCollisionEnter(Collision collisionInfo) {

        // If the player touches the ground while falling
        if ((!myState.GetIsGrounded()) 
            && (collisionInfo.gameObject.tag == "Ground"
                || (Mathf.Abs(rb.velocity.y) < 2f))) {
                StopCoroutine("Land");
                StartCoroutine("Land");
        }
        
    }

    private void OnCollisionExit(Collision collisionInfo) {

        // If the player is no longer touching the ground
        if (collisionInfo.gameObject.tag == "Ground"
            || myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Falling
            || myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Jumping) {
            myState.SetIsGrounded(false);
            // myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);
            Debug.Log("NOT Grounded");
        }
        
    }

}