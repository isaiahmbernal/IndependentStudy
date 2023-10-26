using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterAttackManager : MonoBehaviour
{
    
    public enum WeaponState {
        Sword,
        GreatSword,
        Fists
    }

    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private Animator _anim;
    [SerializeField] private TauntRandom _tauntRandom;

    [Header("Manual References")]
    [SerializeField] private Weapon _currWeapon;
    [SerializeField] private Weapon _sword;
    [SerializeField] private Weapon _greatSword;
    [SerializeField] private Weapon _fists;

    [Header("Weapon State")]
    [SerializeField] private WeaponState _weaponState;
    [SerializeField] private int _currMaxLAStages;

    [Header("Sword Variables")]
    [SerializeField] private int _swordLAStages;
    [SerializeField] private float _swordLABetweenTime;
    [SerializeField] private float _swordLACurrBetweenTime;
    [SerializeField] private float _swordLA1Time;
    [SerializeField] private float _swordLA2Time;

    [Header("Great Sword Variables")]
    [SerializeField] private int _greatSwordLAStages;
    [SerializeField] private float _greatSwordLABetweenTime;
    [SerializeField] private float _greatSwordLACurrBetweenTime;
    [SerializeField] private float _greatSwordLA1Time;
    [SerializeField] private float _greatSwordLA2Time;

    [Header("Fists Variables")]
    [SerializeField] private int _fistsLAStages;
    [SerializeField] private float _fistsLABetweenTime;
    [SerializeField] private float _fistsLACurrBetweenTime;
    [SerializeField] private float _fistsLA1Time;
    [SerializeField] private float _fistsLA2Time;

    private void Awake() {
        _myState = GetComponent<CharacterStateManager>();
        _anim = transform.Find("MyObj").GetComponent<Animator>();
        _tauntRandom = GetComponent<TauntRandom>();
    }

    private void Update() {
        if (_swordLACurrBetweenTime < _swordLABetweenTime) {
            _swordLACurrBetweenTime += Time.deltaTime;
        }
        if (_greatSwordLACurrBetweenTime < _greatSwordLABetweenTime) {
            _greatSwordLACurrBetweenTime += Time.deltaTime;
        }
        if (_fistsLACurrBetweenTime < _fistsLABetweenTime) {
            _fistsLACurrBetweenTime += Time.deltaTime;
        }
    }

    private void DisableAllWeapons() {
        _sword.gameObject.SetActive(false);
        _greatSword.gameObject.SetActive(false);
        _fists.gameObject.SetActive(false);
    }

    public void SetCurrentWeapon(WeaponState weaponState) {
        switch (weaponState) {
            case WeaponState.Sword:
                DisableAllWeapons();
                _sword.gameObject.SetActive(true);
                break;
            case WeaponState.GreatSword:
                DisableAllWeapons();
                _greatSword.gameObject.SetActive(true);
                break;
            case WeaponState.Fists:
                DisableAllWeapons();
                _fists.gameObject.SetActive(true);
                break;
        }
    }

    private void SetAllAnimWeaponsFalse() {
        _anim.SetBool("sword", false);
        _anim.SetBool("greatSword", false);
        _anim.SetBool("fists", false);
    }

    public WeaponState GetWeaponState() {
        return _weaponState;
    }

    public void SetWeaponState(WeaponState newWeaponState) {

        if (_weaponState == newWeaponState) return;

        if ((_myState.GetAbleState() != CharacterStateManager.AbleState.Normal
                && _myState.GetAbleState() != CharacterStateManager.AbleState.Rooted)
            || (_myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Idle
                && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Walking
                && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Running
                && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Jumping
                && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Falling)) {
                    return;
                }

        _tauntRandom.ResetTimeSinceLastMovement();
        _tauntRandom.StopTaunt();

        switch (newWeaponState) {

            case WeaponState.Sword:
                _weaponState = WeaponState.Sword;
                SetCurrentWeapon(WeaponState.Sword);
                _currWeapon = _sword;
                _currMaxLAStages = _swordLAStages;
                SetAllAnimWeaponsFalse();
                _anim.SetBool("sword", true);
                break;

            case WeaponState.GreatSword:
                _weaponState = WeaponState.GreatSword;
                SetCurrentWeapon(WeaponState.GreatSword);
                _currWeapon = _greatSword;
                _currMaxLAStages = _greatSwordLAStages;
                SetAllAnimWeaponsFalse();
                _anim.SetBool("greatSword", true);
                break;

            case WeaponState.Fists:
                _weaponState = WeaponState.Fists;
                SetCurrentWeapon(WeaponState.Fists);
                _currWeapon = _fists;
                _currMaxLAStages = _fistsLAStages;
                SetAllAnimWeaponsFalse();
                _anim.SetBool("fists", true);
                break;       

        }     

        _currWeapon.PlayUnsheathe();   

    }

    public void StopAllAttacks() {
        StopAllCoroutines();
    }

    public void StartLightAttack() {
        if (_anim.GetInteger("lightAttackStage") < _currMaxLAStages) {
            switch (_weaponState) {
                case WeaponState.Sword:
                    if (_swordLACurrBetweenTime >= _swordLABetweenTime) {
                        // Debug.Log("START SWORD LIGHT ATTACK: START COROUTINE");
                        StopCoroutine("AttackLight");
                        StartCoroutine("AttackLight");    
                    }
                    break;
                case WeaponState.GreatSword:
                    if (_greatSwordLACurrBetweenTime >= _greatSwordLABetweenTime) {
                        // Debug.Log("START GREAT SWORD LIGHT ATTACK: START COROUTINE");
                        StopCoroutine("AttackLight");
                        StartCoroutine("AttackLight");    
                    }
                    break;
                case WeaponState.Fists:
                    if (_fistsLACurrBetweenTime >= _fistsLABetweenTime) {
                        // Debug.Log("START FISTS LIGHT ATTACK: START COROUTINE");
                        StopCoroutine("AttackLight");
                        StartCoroutine("AttackLight");    
                    }
                    break;
            }
            
        }
    }

    public void ResetLightAttackStage() {
        _anim.SetInteger("lightAttackStage", 0);
    }

    private float GetLightAttackTime() {

        float attackTime = 1f;
        switch (_weaponState) {
            case WeaponState.Sword:
                switch (_anim.GetInteger("lightAttackStage")) {
                    case 1:
                        attackTime = _swordLA1Time;
                        break;
                    case 2:
                        attackTime = _swordLA2Time;
                        break;
                }
                break;
            case WeaponState.GreatSword:
                switch (_anim.GetInteger("lightAttackStage")) {
                    case 1:
                        attackTime = _greatSwordLA1Time;
                        break;
                    case 2:
                        attackTime = _greatSwordLA2Time;
                        break;
                }
                break;
            case WeaponState.Fists:
                switch (_anim.GetInteger("lightAttackStage")) {
                    case 1:
                        attackTime = _fistsLA1Time;
                        break;
                    case 2:
                        attackTime = _fistsLA2Time;
                        break;
                }
                break;
        }
        Debug.Log("Weapon: " + _weaponState + " | Stage: " + _anim.GetInteger("lightAttackStage") + " | Attack Time: " + attackTime);
        return attackTime;
    }

    private IEnumerator AttackLight() {

        _currWeapon.ResetHits();

        _currWeapon.PlayWhoosh();

        _swordLACurrBetweenTime = 0;
        _greatSwordLACurrBetweenTime = 0;
        _fistsLACurrBetweenTime = 0;

        // Debug.Log("START LIGHT ATTACK: DO COROUTINE");

        _anim.SetInteger("lightAttackStage", _anim.GetInteger("lightAttackStage") + 1);

        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Attacking);

        float attackTime = GetLightAttackTime();

        yield return new WaitForSeconds(attackTime);

        // If we were on the ground, go back to idle
        if (_myState.GetIsGrounded()) {
            _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);
        }
        // If we were NOT on the ground, continue falling
        else _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Falling);

        // Reset the total times the weapon can hit something
        _currWeapon.ResetHits();
        _anim.SetInteger("lightAttackStage", 0);

        // Debug.Log("START LIGHT ATTACK: FINISH COROUTINE");

    }

}
