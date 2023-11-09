using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIdleManager : MonoBehaviour
{
    
    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private CharacterAttackManager _myAttack;

    [Header("Taunt Variables")]
    [SerializeField] private float _swordTime;
    [SerializeField] private float _greatSwordTime;
    [SerializeField] private float _fistsTime;
    [SerializeField] private float _timeBeforeRetryTaunt;

    private void Awake() {
        _myState = GetComponent<CharacterStateManager>();
        _myAttack = GetComponent<CharacterAttackManager>();
    }

    private float GetTauntTime() {
        float tauntTime = 0;
        switch (_myAttack.GetWeaponState()) {
            case CharacterAttackManager.WeaponState.Sword:
                tauntTime = _swordTime;
                break;
            case CharacterAttackManager.WeaponState.GreatSword:
                tauntTime = _greatSwordTime;
                break;
            case CharacterAttackManager.WeaponState.Fists:
                tauntTime = _fistsTime;
                break;
        }
        return tauntTime;
    }

    private IEnumerator IdleAnim() {

        if (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Idle) {

            _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Taunt);

            yield return new WaitForSeconds(GetTauntTime());

            _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Taunt);

        } 
        
        else yield return new WaitForSeconds(_timeBeforeRetryTaunt);

    }

    

}
