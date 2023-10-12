using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// <c>PlayerAttack</c>
/// manages the player's attack input / state.
/// Not to be confused with PlayerWeapon.cs, which
/// manages all damage and interaction with other objects
/// </summary>
public class PlayerAttack : MonoBehaviour
{

    private enum WeaponState {
        SwordShield,
        Sword,
    }

    [Header("References")]
    [SerializeField] private PlayerMovement pm;
    [SerializeField] private Animator anim;
    [SerializeField] private CharacterStateManager myState;
    [SerializeField] private PlayerWeapon wep;

    [Header("Attack State")]
    [SerializeField] private WeaponState weaponState = WeaponState.SwordShield;

    [Header("Keybinds")]
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    private void Update() {
        // Check for player input every frame
        PlayerInput();
    }

    /// <summary>
    /// 
    /// Checks for player input and is meant to be
    /// called within Update().
    /// 
    /// It starts the attack sequence, while also determining
    /// whether or not the player can attack based on
    /// able state and current action.
    /// 
    /// </summary>
    private void PlayerInput() {

        // Attack if the player is NOT incapacitated
        // and not already attacking
        if (Input.GetKeyDown(attackKey)
            && (myState.GetAbleState() != CharacterStateManager.AbleState.Incapacitated)
            && (myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Attacking)) {
                // Debug.Log("Pressed Attack");
                StopCoroutine("Attack");
                StartCoroutine("Attack");
        }

    }

    /// <summary>
    /// 
    /// Initiates the player attack sequence which
    /// alternates through able states and current
    /// actions based on the times set within the
    /// currently attached weapon (see Class <c>PlayerWeapon</c>).
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack() {

        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Attacking);

        // Play the basic sound of the weapon
        wep.PlaySound();

        yield return new WaitForSeconds(wep.GetTotalAttackTime());

        // If we were on the ground, go back to idle
        if (myState.GetIsGrounded()) {
            myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);
        }
        // If we were NOT on the ground, continue falling
        else {
            myState.CurrentActionChange(CharacterStateManager.CurrentAction.Falling);
        }
        
        // Reset the total times the weapon can hit something
        wep.ResetHits();

    }

}
