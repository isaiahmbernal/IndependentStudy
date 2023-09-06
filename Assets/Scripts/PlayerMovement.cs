using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerAttack pAtk;

    [Header("Ground Check")]
    [SerializeField] private bool isGrounded;

    [Header("Movement State")]
    [SerializeField] private bool canMove;
    [SerializeField] private bool isIdle;
    [SerializeField] private bool isWalking;
    [SerializeField] private bool isRunning;
    [SerializeField] private bool canJump;
    [SerializeField] private bool startedJump;
    [SerializeField] private bool isJumping;

    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;

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

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if (startedJump) {
            StartCoroutine(Jump());
        }

        MovementAnimations();

        SpeedControl();
        
    }

    private void PlayerInput() {

        // Movement
        if (canMove) {

            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            // If Player *IS NOT* Moving
            if (Mathf.Approximately(rb.velocity.x, 0f) && Mathf.Approximately(rb.velocity.y, 0f)) {

                isIdle = true;
                isWalking = false;
                isRunning = false;

            // If Player *IS* Moving
            } else if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f) {

                isIdle = false;

                if (Input.GetKey(runKey)) {

                    isWalking = false;
                    isRunning = true;

                } else {

                    isWalking = true;
                    isRunning = false;

                }

            }
        }

        // Jump
        if (Input.GetKeyDown(jumpKey) && canJump && isGrounded && !pAtk.getIsAttacking()) {
            startedJump = true;
        }

    }

    private void MovementAnimations() {

        anim.SetBool("isIdle", isIdle);
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isRunning", isRunning);

    }

    private void MovePlayer() {

        // Calculate Movement Direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        
        if (canMove) {
            // Add Force
            if (isRunning) {
                rb.AddForce(moveDirection.normalized * runSpeed * 10f, ForceMode.Force);
            } else {
                rb.AddForce(moveDirection.normalized * walkSpeed * 10f, ForceMode.Force);
            }
        }
        
    }

    private void SpeedControl() {

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit Velocity
        if (isRunning) {
            if (flatVel.magnitude > runSpeed) {
                Vector3 limitedVel = flatVel.normalized * walkSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        } else {
            if (flatVel.magnitude > walkSpeed) {
                Vector3 limitedVel = flatVel.normalized * walkSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private IEnumerator Jump() {

        // Reset startedJump
        startedJump = false;

        // Start Jump
        canJump = false;
        isJumping = true;
        rb.AddForce(transform.up * jumpForce, ForceMode.Force);

        // Wait For Jump Cooldown
        yield return new WaitForSeconds(jumpCooldown);
        isJumping = false;
        canJump = true;

    }

    public bool getCanMove() {
        return canMove;
    }

    public void setCanMove(bool canMove) {
        this.canMove = canMove;
    }

}