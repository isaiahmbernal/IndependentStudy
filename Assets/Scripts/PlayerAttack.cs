using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    private enum WeaponState {
        SwordShield,
        Sword,
    }

    [Header("Attack")]
    [SerializeField] private PlayerMovement pm;
    [SerializeField] private Animator anim;

    [Header("Attack Variables")]
    [SerializeField] private PlayerWeapon wep;

    [Header("Attack State")]
    [SerializeField] private WeaponState weaponState = WeaponState.SwordShield;
    [SerializeField] private bool canAttack;
    [SerializeField] private bool startedAttack;
    [SerializeField] private bool isAttacking;

    [Header("Keybinds")]
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    public bool getIsAttacking() {
        return isAttacking;
    }

    void Update()
    {
        PlayerInput();
    }

    void FixedUpdate() {

        if (startedAttack) {
            StartCoroutine(Attack());
        }

    }

    private void PlayerInput() {

        // Light Attack
        if (Input.GetKeyDown(attackKey) && canAttack) {
            startedAttack = true;
        }

    }

    private IEnumerator Attack() {

        // Reset startedAttack
        startedAttack = false;

        // Start Attack
        canAttack = false;
        isAttacking = true;
        anim.SetBool("isAttacking", isAttacking);
        pm.setCanMove(false);

        // Wait To End Attack
        yield return new WaitForSeconds(wep.getTotalAttackTime());
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        pm.setCanMove(true);
        
        // Wait For Attack Cooldown
        yield return new WaitForSeconds(wep.getAttackCooldown());
        canAttack = true;
        wep.setHitsLeft();

    }

}
