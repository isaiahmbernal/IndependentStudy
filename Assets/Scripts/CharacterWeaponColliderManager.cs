using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeaponColliderManager : MonoBehaviour
{

    [Header("Auto References")]
    [SerializeField] private CharacterAttackManager _myAttack;

    private void Awake() {
        _myAttack = transform.root.GetComponent<CharacterAttackManager>();
    }

    public void EnableWeaponCollider() {
        _myAttack.GetCurrentWeapon().EnableCollider();
    }

    public void DisableWeaponCollider() {
        _myAttack.GetCurrentWeapon().DisableCollider();
    }
}
