using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to a character to enable it to play
/// an idle / taunt animation at set intervals
/// if the character has not done any actions
/// in that amount of time
/// </summary>
public class TauntRandom : MonoBehaviour
{
    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private CharacterAttackManager _myAttack;

    [Header("Taunt Variables")]
    [SerializeField] private float _tauntTime;
    [SerializeField] private float _swordTime;
    [SerializeField] private float _greatSwordTime;
    [SerializeField] private float _fistsTime;
    [SerializeField] private float _timeBeforeTaunt;
    [SerializeField] private float _timeSinceLastMovement;

    private void Awake() {
        _myState = GetComponent<CharacterStateManager>();
        _myAttack = GetComponent<CharacterAttackManager>();
    }

    public void ResetTimeSinceLastMovement() {
        _timeSinceLastMovement = 0;
    }
    
    private void FixedUpdate() {

        if (_timeSinceLastMovement > _timeBeforeTaunt
            && _myState.GetAbleState() != CharacterStateManager.AbleState.Dead) {
                StartCoroutine("PlayTaunt");
        }
        
        else if (_myState.GetAbleState() != CharacterStateManager.AbleState.Dead) {
            _timeSinceLastMovement += Time.deltaTime;
        }

    }

    /// <summary>
    /// 
    /// Meant to be accessed within the
    /// CharacterStateManager after every action
    /// change to stop the tuant from executing.
    /// 
    /// </summary>
    public void StopTaunt() {
        StopCoroutine("PlayTaunt");
    }

    private float GetTauntTime() {
        float tauntTime = _tauntTime;
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

    /// <summary>
    /// 
    /// Simply plays sets the taunt action and
    /// returns to idle after the set taunt time.
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayTaunt() {

        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Taunt);
        
        yield return new WaitForSeconds(GetTauntTime());
        
        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);

    }

}
