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
    [Header("References")]
    [SerializeField] private CharacterStateManager myState;

    [Header("Taunt Variables")]
    [SerializeField] private float tauntTime;
    [SerializeField] private float timeBeforeTaunt;
    [SerializeField] private float timeSinceLastMovement;

    public void ResetTimeSinceLastMovement() {
        timeSinceLastMovement = 0;
    }
    
    private void FixedUpdate() {

        if (timeSinceLastMovement > timeBeforeTaunt) {
            StartCoroutine("PlayTaunt");
        } else {
            timeSinceLastMovement += Time.deltaTime;
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

    /// <summary>
    /// 
    /// Simply plays sets the taunt action and
    /// returns to idle after the set taunt time.
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayTaunt() {

        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Taunt);
        
        yield return new WaitForSeconds(tauntTime);
        
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);

    }

}
