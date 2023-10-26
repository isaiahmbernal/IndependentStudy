using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private CharacterAttackManager _myAttack;

    [Header("Input Keys")]
    [SerializeField] private KeyCode _attackKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode _wep1Key = KeyCode.Alpha1;
    [SerializeField] private KeyCode _wep2Key = KeyCode.Alpha2;

    private void Awake() {
        _myState = GetComponent<CharacterStateManager>();
        _myAttack = GetComponent<CharacterAttackManager>();
    }

    private void Update() {
        AttackInput();
    }

    private void AttackInput() {

        if (Input.GetKeyDown(_attackKey) && _myState.GetCanAttack()) {
            Debug.Log("INPUT: Light Attack");
            _myAttack.StartLightAttack();
        }

        // Set weapon to Sword
        if (Input.GetKeyDown(_wep1Key)) {
            Debug.Log("INPUT: Switch to Sword");
            _myAttack.SetWeaponState(CharacterAttackManager.WeaponState.Sword);
        }

        // Set weapon to Great Sword
        else if (Input.GetKeyDown(_wep2Key)) {
            Debug.Log("INPUT: Switch to Great Sword");
            _myAttack.SetWeaponState(CharacterAttackManager.WeaponState.GreatSword);
        }

    }

}
