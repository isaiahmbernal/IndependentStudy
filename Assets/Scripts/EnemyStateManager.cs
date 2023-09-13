using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateManager : MonoBehaviour
{

    public enum AbleState {
        Normal,
        Incapacitated
    }

    public enum CurrentAction {
        Idle,
        StandingUp,
        Walking,
        Running,
        Jumping,
        Attacking,
        Stunned,
        Ragdoll
    }

    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private EnemyRagdoll ragdoll;

    [Header("Abilities")]
    [SerializeField] private AbleState currentAbleState;
    [SerializeField] private bool canWalk;
    [SerializeField] private bool canRun;
    [SerializeField] private bool canJump;
    [SerializeField] private bool canAttack;

    [Header("Current Action")]
    [SerializeField] private CurrentAction currentAction;
    

    public void AbleStateChange(AbleState newAbleState) {

        switch (newAbleState) {

            case (AbleState.Normal):
                currentAbleState = AbleState.Normal;
                canWalk = true;
                canRun = true;
                canJump = true;
                canAttack = true;
                break;


            case (AbleState.Incapacitated):
                currentAbleState = AbleState.Incapacitated;
                canWalk = false;
                canRun = false;
                canJump = false;
                canAttack = false;
                break;

        }

    }

    public void CurrentActionChange(CurrentAction newCurrentAction) {

        switch (newCurrentAction) {

            case (CurrentAction.Idle):
                currentAction = CurrentAction.Idle;
                anim.SetBool("isIdle", true);
                anim.SetBool("isStandingUp", false);
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isStunned", false);
                break;

            case (CurrentAction.StandingUp):
                currentAction = CurrentAction.StandingUp;
                anim.SetBool("isIdle", false);
                anim.SetBool("isStandingUp", true);
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isStunned", false);
                break;

            case (CurrentAction.Walking):
                currentAction = CurrentAction.Walking;
                anim.SetBool("isIdle", false);
                anim.SetBool("isStandingUp", false);
                anim.SetBool("isWalking", true);
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isStunned", false);
                break;

            case (CurrentAction.Running):
                currentAction = CurrentAction.Running;
                anim.SetBool("isIdle", false);
                anim.SetBool("isStandingUp", false);
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", true);
                anim.SetBool("isJumping", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isStunned", false);
                break;

            case (CurrentAction.Jumping):
                currentAction = CurrentAction.Jumping;
                anim.SetBool("isIdle", false);
                anim.SetBool("isStandingUp", false);
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", true);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isStunned", false);
                break;

            case (CurrentAction.Attacking):
                currentAction = CurrentAction.Attacking;
                anim.SetBool("isIdle", false);
                anim.SetBool("isStandingUp", false);
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", false);
                anim.SetBool("isAttacking", true);
                anim.SetBool("isStunned", false);
                break;

            case (CurrentAction.Stunned):
                currentAction = CurrentAction.Stunned;
                anim.SetBool("isIdle", false);
                anim.SetBool("isStandingUp", false);
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isStunned", true);
                break;

            case (CurrentAction.Ragdoll):
                currentAction = CurrentAction.Ragdoll;
                anim.SetBool("isIdle", false);
                anim.SetBool("isStandingUp", false);
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", false);
                anim.SetBool("isJumping", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isStunned", false);
                break;

        }

    }

}
