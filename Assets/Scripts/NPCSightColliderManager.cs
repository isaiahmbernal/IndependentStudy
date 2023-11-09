using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSightColliderManager : MonoBehaviour
{
    [Header("Auto References")]
    [SerializeField] private GameObject _myRootObj;
    [SerializeField] private NPCMovementManager _myMovement;

    [Header("Aggression Tags")]
    [SerializeField] private List<String> _aggressiveTags = new List<String>();

    [Header("Sight Variables")]
    [SerializeField] private float _timeUntilLost;
    

    private void Awake() {
        _myRootObj = transform.root.gameObject;
        _myMovement = transform.root.GetComponent<NPCMovementManager>();
    }
    
    private void OnTriggerEnter(Collider collisionInfo) {

        if (_aggressiveTags.Contains(collisionInfo.gameObject.name)) {

            Transform currTarget;
            if (_myMovement.GetTarget() != null) {

                currTarget = _myMovement.GetTarget();

                if (currTarget.gameObject.GetInstanceID() == collisionInfo.gameObject.GetInstanceID()) {
                    StopCoroutine("OutOfRangeTimer");
                    Debug.Log($"{_myRootObj.name} ({_myRootObj.GetInstanceID()}) Re-Found Its Target: {currTarget.name} ({currTarget.gameObject.GetInstanceID()})");
                    return;
                }

                float currTargetDist = Vector3.Distance(currTarget.position, transform.position);
                float nextTargetDist = Vector3.Distance(collisionInfo.transform.position, transform.position);

                if (nextTargetDist < currTargetDist) {
                    StopCoroutine("OutOfRangeTimer");
                    _myMovement.SetTarget(collisionInfo.transform);
                    _myMovement.SetLookAt(true);
                    Debug.Log($"{_myRootObj.name} ({_myRootObj.GetInstanceID()}) Found A New Target: {collisionInfo.gameObject.name} ({collisionInfo.gameObject.GetInstanceID()})");
                }
            }

            else {
                _myMovement.SetTarget(collisionInfo.transform);
                _myMovement.SetLookAt(true);
                _myMovement.ResetMovementCoroutine();
                Debug.Log($"{_myRootObj.name} ({_myRootObj.GetInstanceID()}) Found A Target: {collisionInfo.gameObject.name} ({collisionInfo.gameObject.GetInstanceID()})");
            }

        }

    }

    private void OnTriggerExit(Collider collisionInfo) {

        if (_myMovement.GetTarget() == null) return;

        if (collisionInfo.gameObject.GetInstanceID() ==_myMovement.GetTarget().gameObject.GetInstanceID()) {
            Debug.Log($"{_myRootObj.name}'s ({_myRootObj.GetInstanceID()}) Target [{collisionInfo.gameObject.name} ({collisionInfo.gameObject.GetInstanceID()})] Is Out Of Range");
            StopCoroutine("OutOfRangeTimer");
            StartCoroutine("OutOfRangeTimer");
        }

    }

    private IEnumerator OutOfRangeTimer() {
        yield return new WaitForSeconds(_timeUntilLost);
        _myMovement.SetTarget(null);
        _myMovement.SetLookAt(false);
        Debug.Log($"{_myRootObj.name} ({_myRootObj.GetInstanceID()}) Lost Its Target");
    }

}
