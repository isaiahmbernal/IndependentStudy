using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to characters whenever they're allowed
/// to fall and land. Checks if the character is
/// currently falling and checks for ground
/// collision to enable 
/// </summary>
public class CharacterGroundCheck : MonoBehaviour {

    [Header("References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private Rigidbody _rb;

    [Header("Variables")]
    [SerializeField] private float _landTime;

    private void FixedUpdate() {

        // If I'm NOT Grounded and I'm falling
        if (!_myState.GetIsGrounded() && _rb.velocity.y < -1f) {

            // If I'm not already falling, set to falling
            if (_myState.GetCurrentAction() != CharacterStateManager.CurrentAction.Falling) {
                _myState.CurrentActionChange(CharacterStateManager.CurrentAction.Falling);    
            }

        }

    }

    /// <summary>
    /// 
    /// Initiates a landing sequence for the character,
    /// changing its able state and current actions until
    /// the duration of the landing is over.
    /// 
    /// Run whenever the character lands on the ground
    /// or a surface.
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator Land() {

        Debug.Log(gameObject.name + " Touched Ground");
        
        _myState.SetIsGrounded(true);
        _myState.AbleStateChange(CharacterStateManager.AbleState.Incapacitated);
        _myState.CurrentActionChange(CharacterStateManager.CurrentAction.Landing);

        yield return new WaitForSeconds(_landTime);

        _myState.AbleStateChange(CharacterStateManager.AbleState.Normal);
        _myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);

    }

    private void OnCollisionEnter(Collision collisionInfo) {

        // If the character touches the ground while falling
        if ((!_myState.GetIsGrounded()) 
            && (collisionInfo.gameObject.tag == "Ground"
                || (Mathf.Abs(_rb.velocity.y) < 2f))) {
                StopCoroutine("Land");
                StartCoroutine("Land");
        }
        
    }

    private void OnCollisionExit(Collision collisionInfo) {

        // If the character is no longer touching the ground
        if (collisionInfo.gameObject.tag == "Ground"
            && ( _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Falling
            || _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Jumping)) {
            _myState.SetIsGrounded(false);
            // _myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);
            Debug.Log("NOT Grounded");
        }
        
    }

}
