using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRagdoll : MonoBehaviour
{

    
    [Header("References")]
    [SerializeField] private CharacterStateManager myState;
    [SerializeField] private Rigidbody[] ragdollRigidbodies;
    [SerializeField] private Rigidbody myRb;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform myTransform;
    [SerializeField] private Transform myHips;

    [Header("Ragdoll Variables")]
    [SerializeField] float timeBeforeRagdoll;
    [SerializeField] float standupTime;

    void Awake() {

        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        DisableRagdoll();

    }

    private void DisableRagdoll() {

        anim.enabled = true;

        foreach (var rigidbody in ragdollRigidbodies) {
            rigidbody.isKinematic = true;
        }

        myRb.isKinematic = false;
        myTransform.position = myHips.position;
        // myTransform.eulerAngles = new Vector3(myTransform.eulerAngles.x, myHips.eulerAngles.y, myTransform.eulerAngles.z);
        // Vector3 currHipsPosition = myHips.position;
        
        // StartCoroutine(MoveToRagdoll(currHipsPosition));
    }

    private IEnumerator MoveToRagdoll(Vector3 currHipsPosition) {

        float timeSinceStarted = 0f;
        while (true) {

            timeSinceStarted += Time.deltaTime;
            myTransform.position = Vector3.Lerp(myTransform.position, currHipsPosition, timeSinceStarted * 20);

            // If the object has arrived, stop the coroutine
            if (myTransform.position == currHipsPosition) yield break;

            // Otherwise, continue next frame
            yield return null;

        }

    }

    private void EnableRagdoll() {
        
        anim.enabled = false;
        
        foreach (var rigidbody in ragdollRigidbodies) {
            rigidbody.isKinematic = false;
        }
        myRb.isKinematic = true;

    }

    public void HandleRagdoll(float stunTime) {

        StartCoroutine(Ragdoll(stunTime));

    }

    private IEnumerator Ragdoll(float stunTime) {

        myState.AbleStateChange(CharacterStateManager.AbleState.Incapacitated);
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Stunned);

        yield return new WaitForSeconds(timeBeforeRagdoll);

        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Ragdoll);
        EnableRagdoll();

        yield return new WaitForSeconds(stunTime);

        DisableRagdoll();
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.StandingUp);

        yield return new WaitForSeconds(standupTime);

        myState.AbleStateChange(CharacterStateManager.AbleState.Normal);
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);

    }

}
