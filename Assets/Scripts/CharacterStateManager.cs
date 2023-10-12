using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The global state tracker / manager
/// for characters.
/// 
/// Meant to be the one solution / location
/// for changing current abilities (ability to
/// walk, run, jump, etc.) and for changing
/// current action (managing the animation state)
/// </summary>
public class CharacterStateManager : MonoBehaviour
{

    public enum AbleState {
        Normal,
        Incapacitated,
        Rooted,
        Dead,
    }

    public enum CurrentAction {
        Idle,
        StandingUp,
        Walking,
        Running,
        Jumping,
        Falling,
        Landing,
        Attacking,
        Stunned,
        Ragdoll,
        Taunt
    }

    [Header("References")]
    [SerializeField] private Animator _anim;
    // [SerializeField] private Ragdoll ragdoll;
    [SerializeField] private TauntRandom _tauntRandom;

    [Header("Abilities")]
    [SerializeField] private AbleState _currentAbleState;
    [SerializeField] private bool _canWalk;
    [SerializeField] private bool _canRun;
    [SerializeField] private bool _canJump;
    [SerializeField] private bool _canAttack;

    [Header("Current Action")]
    [SerializeField] private CurrentAction _currentAction;

    [Header("Misc State")]
    [SerializeField] private bool _isGrounded;

    public AbleState GetAbleState() {
        return _currentAbleState;
    }

    public CurrentAction GetCurrentAction() {
        return _currentAction;
    }

    public bool GetCanWalk() {
        return _canWalk;
    }

    public bool GetCanRun() {
        return _canRun;
    }

    public bool GetCanJump() {
        return _canJump;
    }

    public bool GetCanAttack() {
        return _canAttack;
    }

    public bool GetIsGrounded() {
        return _isGrounded;
    }

    public void SetIsGrounded(bool newIsGrounded) {
        _isGrounded = newIsGrounded;
    }

    /// <summary>
    /// 
    /// Used to change the character's current abilities,
    /// i.e. whether it can walk, run, jump, attack, etc.
    /// 
    /// Pass in a new able state from the AbleState enum to
    /// manipulate the character's abilities, altering all
    /// ability booleans depending on what state is being 
    /// switched to, and the current state can be globally
    /// checked within other scripts to determine what the
    /// character can and cannot do.
    /// 
    /// Example: We only want the character to be able to move
    /// if its state is set to Normal, so we simply gatekeep
    /// that code with CharacterStateManager.GetAbleState() ==
    /// CharacterStateManager.AbleState.Normal
    /// 
    /// </summary>
    /// <param name="newCurrentAction"></param>
    public void AbleStateChange(AbleState newAbleState) {

        _tauntRandom.ResetTimeSinceLastMovement();

        switch (newAbleState) {

            case AbleState.Normal:
                _currentAbleState = AbleState.Normal;
                _canWalk = true;
                _canRun = true;
                _canJump = true;
                _canAttack = true;
                break;


            case AbleState.Incapacitated:
                _currentAbleState = AbleState.Incapacitated;
                _canWalk = false;
                _canRun = false;
                _canJump = false;
                _canAttack = false;
                break;

            case AbleState.Rooted:
                _currentAbleState = AbleState.Rooted;
                _canWalk = false;
                _canRun = false;
                _canJump = false;
                _canAttack = true;
                break;

            case AbleState.Dead:
                _currentAbleState = AbleState.Dead;
                _canWalk = false;
                _canRun = false;
                _canJump = false;
                _canAttack = false;
                break;

        }

        Debug.Log(gameObject.name + " able state changes to " + _currentAbleState);

    }

    // Used to reset animation state
    public void SetAllAnimBoolsFalse() {

        _anim.SetBool("isIdle", false);
        _anim.SetBool("isStanding", false);
        _anim.SetBool("isWalking", false);
        _anim.SetBool("isRunning", false);
        _anim.SetBool("isJumping", false);
        _anim.SetBool("isFalling", false);
        _anim.SetBool("isLanding", false);
        _anim.SetBool("isAttacking", false);
        _anim.SetBool("isStunned", false);
        _anim.SetBool("isTaunting", false);

    }

    /// <summary>
    /// 
    /// Used to change the character's current action state,
    /// including changing the animation.
    /// 
    /// Pass in a new action from the CurrentAction enum
    /// to manipulate the character's animation state,
    /// switching to that animation and allowing for global
    /// access between scripts to check the character's
    /// current state
    /// 
    /// Example: If the character shouldn't be able to
    /// jump while attacking, the jump method will be
    /// gatekept with CharacterStateManager.GetCurrentAction()
    /// != to CharacterStateManager.CurrentAction.Attacking
    /// 
    /// </summary>
    /// <param name="newCurrentAction"></param>
    public void CurrentActionChange(CurrentAction newCurrentAction) {

        _tauntRandom.ResetTimeSinceLastMovement();

        if (!_isGrounded) {

            Debug.Log(gameObject.name + " NOT GROUNDED during action change");

            if (newCurrentAction != CurrentAction.Falling
                && newCurrentAction != CurrentAction.Stunned
                && newCurrentAction != CurrentAction.Ragdoll
                && newCurrentAction != CurrentAction.Attacking) {

                    Debug.Log(gameObject.name + " CANCELED action: newCurrentAction = " + newCurrentAction);
                    return;

                }
        }

        switch (newCurrentAction) {

            case CurrentAction.Idle:
                _currentAction = CurrentAction.Idle;
                SetAllAnimBoolsFalse();
                _anim.SetBool("isIdle", true);
                break;

            case CurrentAction.StandingUp:
                _currentAction = CurrentAction.StandingUp;
                SetAllAnimBoolsFalse();
                _tauntRandom.StopTaunt();
                _anim.SetBool("isStanding", true);
                break;

            case CurrentAction.Walking:
                _currentAction = CurrentAction.Walking;
                SetAllAnimBoolsFalse();
                _tauntRandom.StopTaunt();
                _anim.SetBool("isWalking", true);
                break;

            case CurrentAction.Running:
                _currentAction = CurrentAction.Running;
                SetAllAnimBoolsFalse();
                _tauntRandom.StopTaunt();
                _anim.SetBool("isRunning", true);
                break;

            case CurrentAction.Jumping:
                _currentAction = CurrentAction.Jumping;
                SetAllAnimBoolsFalse();
                _tauntRandom.StopTaunt();
                _anim.SetBool("isJumping", true);
                break;

            case CurrentAction.Falling:
                _currentAction = CurrentAction.Falling;
                SetAllAnimBoolsFalse();
                _tauntRandom.StopTaunt();
                _anim.SetBool("isFalling", true);
                break;

            case CurrentAction.Landing:
                _currentAction = CurrentAction.Landing;
                SetAllAnimBoolsFalse();
                _tauntRandom.StopTaunt();
                _anim.SetBool("isLanding", true);
                break;

            case CurrentAction.Attacking:
                _currentAction = CurrentAction.Attacking;
                SetAllAnimBoolsFalse();
                _tauntRandom.StopTaunt();
                _anim.SetBool("isAttacking", true);
                break;

            case CurrentAction.Stunned:
                _currentAction = CurrentAction.Stunned;
                SetAllAnimBoolsFalse();
                _tauntRandom.StopTaunt();
                _anim.SetBool("isStunned", true);
                break;

            case CurrentAction.Ragdoll:
                _currentAction = CurrentAction.Ragdoll;
                SetAllAnimBoolsFalse();
                _tauntRandom.StopTaunt();
                break;

            case CurrentAction.Taunt:
                _currentAction = CurrentAction.Taunt;
                SetAllAnimBoolsFalse();
                _anim.SetBool("isTaunting", true);
                break;

        }

        Debug.Log(gameObject.name + " current action changes to " + _currentAction);

    }

}