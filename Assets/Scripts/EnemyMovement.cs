using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Manages the movement of enemy NPCs.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private NavMeshAgent _myNavAgent;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Transform _myObjTransform;
    [SerializeField] private Transform _playerTransform;

    [Header("Inherent Variables")]
    [SerializeField] private float _timeBetweenPlayerChecks;

    [Header("Movement Variables")]
    [SerializeField] private float _myRadius;
    [SerializeField] private float _sightRange;
    [SerializeField] private bool _lookAtPlayer;

    private void Awake() {
        _myState = GetComponent<CharacterStateManager>();
        _myNavAgent = GetComponent<NavMeshAgent>();
        _rb = GetComponent<Rigidbody>();
        _myObjTransform = transform.Find("MyObj");
        _playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }

    private void Start() {

        // Start infinite checking for nearby player
        StartCoroutine("PlayerCheck");

    }

    private void Update() {

        // Making sure game object doesn't float away
        _myObjTransform.localPosition = Vector3.Lerp(_myObjTransform.localPosition, new Vector3(0, 0, 0), Time.deltaTime);
        // _myNavAgent.velocity = _rb.velocity;

    }

    private void FixedUpdate()
    {

        // Point the character model at the player
        if (_lookAtPlayer) {
            _myObjTransform.LookAt(new Vector3(_playerTransform.position.x, _myObjTransform.position.y, _playerTransform.position.z));
        }

    }

    /// <summary>
    /// 
    /// A routine to check if the player is nearby, and whether
    /// or not to look at the player, walk towards them, or to
    /// stop moving all together.
    /// 
    /// Runs at set intervals based on _timeBetweenPlayerChecks
    /// until the character is dead
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerCheck() {

        // Do this while we're not dead
        while (_myState.GetAbleState() != CharacterStateManager.AbleState.Dead) {

            // Get the distance between us and the player
            float dist = Vector3.Distance(_playerTransform.position, transform.position);

            // If we're within sight range of the player
            // we want to look at the player
            if (dist < _sightRange
                    && (_myState.GetAbleState() == CharacterStateManager.AbleState.Normal
                        || _myState.GetAbleState() == CharacterStateManager.AbleState.Rooted)
                    && (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Idle
                        || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking
                        || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running))
            {
                _lookAtPlayer = true;
            } else {
                _lookAtPlayer = false;  
            } 
            
            // If we're within sight range of the player
            // and we need to move towards the player
            if (dist <= _sightRange && dist > _myRadius
                && _myState.GetAbleState() == CharacterStateManager.AbleState.Normal
                && (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Idle
                    || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking
                    || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running)) 
            {
                if (_myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Running) {

                    _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Running);

                }

                // Move towards the player
                _myNavAgent.SetDestination(_playerTransform.position);

            } 

            // If we're at the the player
            if (dist <= _myRadius) {

                // If we were walking or running, set our action to idle
                if (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking
                    || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running) {

                        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);

                }

                // Stop moving
                _myNavAgent.SetDestination(transform.position);

                // 

                // && (_myState.GetAbleState() == CharacterStateManager.AbleState.Normal
                //     || _myState.GetAbleState() == CharacterStateManager.AbleState.Rooted))

                // // If we were walking or running, set our action to idle
                // if (_myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Walking
                //     || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Running) {

                //         _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);

                // }

                

            }
            
            yield return new WaitForSeconds(_timeBetweenPlayerChecks);

        }

        _lookAtPlayer = false;

    }

}
