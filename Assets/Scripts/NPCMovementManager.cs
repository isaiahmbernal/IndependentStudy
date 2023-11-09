using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages the movement of enemy NPCs.
/// </summary>
public class NPCMovementManager : MonoBehaviour
{
    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private CharacterAttackManager _myAttack;
    [SerializeField] private TauntRandom _taunt;
    [SerializeField] private NavMeshAgent _myNavAgent;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _myObjTransform;

    [Header("Inherent Variables")]
    [SerializeField] private float _timeBetweenTargetChecks;

    [Header("Movement Variables")]
    [SerializeField] private List<Transform> _patrolPoints;
    [SerializeField] private int _currPoint;
    [SerializeField] private float _pointStopTime;
    [SerializeField] private bool _lookAtPoint;
    [SerializeField] private Transform _target;
    [SerializeField] private float _myRadius;
    [SerializeField] private float _sightRange;
    [SerializeField] private bool _lookAtTarget;

    private void Awake() {
        _myState = GetComponent<CharacterStateManager>();
        _myAttack = GetComponent<CharacterAttackManager>();
        _myNavAgent = GetComponent<NavMeshAgent>();
        _myNavAgent.updateRotation = false;
        _rb = GetComponent<Rigidbody>();
        _myObjTransform = transform.Find("MyObj");
    }

    public Transform GetTarget() {
        return _target;
    }

    public void SetTarget(Transform newTarget) {
        _target = newTarget;
    }

    public void SetLookAt(bool newLookAtTarget) {
        _lookAtTarget = newLookAtTarget;
    }

    private void Start() {

        // Start infinite checking for nearby player
        StartCoroutine("Movement");

    }

    private void Update() {

        // Making sure game object doesn't float away
        _myObjTransform.localPosition = Vector3.Lerp(_myObjTransform.localPosition, new Vector3(0, 0, 0), Time.deltaTime);
        // _myNavAgent.velocity = _rb.velocity;

    }

    private void FixedUpdate()
    {

        // Point the character model at the player
        if (_lookAtTarget
            && (_myState.GetAbleState() != CharacterStateManager.AbleState.Dead)
            && (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Idle
                || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking
                || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running)) {
            _myObjTransform.LookAt(new Vector3(_target.position.x, _myObjTransform.position.y, _target.position.z));
        }
        else if (_target == null && _lookAtPoint) {
            _myObjTransform.LookAt(new Vector3(
                _patrolPoints[_currPoint].position.x,
                _myObjTransform.position.y,
                _patrolPoints[_currPoint].position.z));
        }

    }

    /// <summary>
    /// 
    /// A routine to check if the target is nearby, and whether
    /// or not to look at the target, walk towards them, or to
    /// stop moving all together.
    /// 
    /// Runs at set intervals based on _timeBetweenTargetChecks
    /// until the character is dead
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Movement() {

        // Do this while we're not dead
        while (_myState.GetAbleState() != CharacterStateManager.AbleState.Dead) {

            if (_myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Idle
                && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Walking
                && _myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Running) {
                    yield return new WaitForSeconds(_timeBetweenTargetChecks);
                    continue;
            }

            // If we have a target
            if (_target != null) {

                // Get the distance between us and the target
                float dist = Vector3.Distance(_target.position, transform.position);

                if (dist > _myRadius) {
                    if (_myState.GetCanRun()) {
                        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Running);
                        // Move towards the player
                        _myNavAgent.SetDestination(_target.position);
                    }
                }

                // If we're at the the target
                if (dist <= _myRadius) {

                    // If we were walking or running, set our action to idle
                    if (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking
                        || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running) {

                            _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);

                    }

                    // Stop moving
                    _myNavAgent.SetDestination(transform.position);

                    _myAttack.StartLightAttack();

                }
            
            }

            // If we don't have a target
            else {

                // Get the distance between us and the patrol point
                float dist = Vector3.Distance(_patrolPoints[_currPoint].position, transform.position);

                // If we haven't reached the patrol point
                if (dist > _myRadius) {

                    // If we can walk
                    if (_myState.GetCanWalk()) {
                        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Walking);
                        _myNavAgent.SetDestination(_patrolPoints[_currPoint].position);
                        _lookAtPoint = true;
                        // Debug.Log($"{gameObject.name} ({gameObject.GetInstanceID()}) Moving To: {_patrolPoints[_currPoint].gameObject.name}");
                    }

                }

                // If we have reached the patrol point
                else {
                    _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);
                    _lookAtPoint = false;
                    // Debug.Log($"{gameObject.name} ({gameObject.GetInstanceID()}) Waiting At: {_patrolPoints[_currPoint].gameObject.name}");
                    yield return new WaitForSeconds(_pointStopTime);
                    SetNextPatrolPoint();
                }

            }

            yield return new WaitForSeconds(_timeBetweenTargetChecks);

        }

    }

    private void SetNextPatrolPoint() {
        if (_currPoint == _patrolPoints.Count - 1) _currPoint = 0;
        else _currPoint += 1;
    }

    public void ResetMovementCoroutine() {
        StopCoroutine("Movement");
        StartCoroutine("Movement");
    }

}