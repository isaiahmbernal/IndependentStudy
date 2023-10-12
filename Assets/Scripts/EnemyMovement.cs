using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages the movement of enemy NPCs.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStateManager myState;
    [SerializeField] private Transform myObjTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private NavMeshAgent myNavAgent;

    [Header("Movement Variables")]
    [SerializeField] private float myRadius;
    [SerializeField] private float sightRange;
    [SerializeField] private float timeBetweenPlayerChecks;
    [SerializeField] private bool lookAtPlayer;

    private void Start() {

        // Start infinite checking for nearby player
        StartCoroutine("PlayerCheck");

    }

    private void Update() {

        // Making sure game object doesn't float away
        myObjTransform.localPosition = Vector3.Lerp(myObjTransform.localPosition, new Vector3(0, 0, 0), Time.deltaTime);

    }

    private void FixedUpdate()
    {

        // Point the character model at the player
        if (lookAtPlayer) {
            myObjTransform.LookAt(new Vector3(playerTransform.position.x, myObjTransform.position.y, playerTransform.position.z));
        }

    }

    /// <summary>
    /// 
    /// A routine to check if the player is nearby, and whether
    /// or not to look at the player, walk towards them, or to
    /// stop moving all together.
    /// 
    /// Runs at set intervals based on timeBetweenPlayerChecks
    /// until the character is dead
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerCheck() {

        // Do this while we're not dead
        while (myState.GetAbleState() != CharacterStateManager.AbleState.Dead) {

            // Get the distance between us and the player
            float dist = Vector3.Distance(playerTransform.position, transform.position);

            // If we're within sight range of the player
            // we want to look at the player
            if (dist < sightRange
                    && (myState.GetAbleState() == CharacterStateManager.AbleState.Normal
                        || myState.GetAbleState() == CharacterStateManager.AbleState.Rooted)
                    && (myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Idle
                        || myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking
                        || myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running))
            {
                lookAtPlayer = true;
            } else {
                lookAtPlayer = false;  
            } 
            
            // If we're within sight range of the player
            // and we need to move towards the player
            if (dist <= sightRange && dist > myRadius
                && myState.GetAbleState() == CharacterStateManager.AbleState.Normal
                && (myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Idle
                    || myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking
                    || myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running)) 
            {
                if (myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Running) {

                    myState.CurrentActionChange(CharacterStateManager.CurrentAction.Running);

                }

                // Move towards the player
                myNavAgent.SetDestination(playerTransform.position);

            } 

            // If we're at the the player
            if (dist <= myRadius
                && (myState.GetAbleState() == CharacterStateManager.AbleState.Normal
                    || myState.GetAbleState() == CharacterStateManager.AbleState.Rooted)) {

                // If we were walking or running, set our action to idle
                if (myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking
                    || myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running) {

                        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);

                }

                // Stop moving
                myNavAgent.SetDestination(transform.position);

            }
            
            yield return new WaitForSeconds(timeBetweenPlayerChecks);

        }

        lookAtPlayer = false;

    }

}
