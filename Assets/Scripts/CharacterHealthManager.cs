using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealthManager : MonoBehaviour
{
    
    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private Ragdoll _myRagdoll;
    [SerializeField] private AudioSource _hurtSound;
    [SerializeField] private CharacterHealthBar _charHealthBar;

    [Header("Health Variables")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currHealth;
    [SerializeField] private float _myStunTime;

    private void Awake() {

        _myState = GetComponent<CharacterStateManager>();
        _myRagdoll = GetComponent<Ragdoll>();
        _hurtSound = transform.Find("HurtSound").GetComponent<AudioSource>();

        if (transform.Find("Canvas") == null) {
            Debug.Log(gameObject.name + ": No Canvas");
        }

        else if (transform.Find("Canvas").Find("HealthBar") == null) {
            Debug.Log(gameObject.name + ": No Health Bar");
        }

        else {
          _charHealthBar = transform.Find("Canvas").Find("HealthBar").GetComponent<CharacterHealthBar>();  
        } 

    }

    public void TakeDamage(float damage, float stunTime, bool willRagdoll) {

        Debug.Log(gameObject.name + ": Take Damage");

        // Set all health / damage values
        _currHealth -= damage;

        if (_charHealthBar) _charHealthBar.UpdateHealthBar(_currHealth, _maxHealth);

        _myStunTime = stunTime;

        // If the character is already iterating
        // through the BasicHurt coroutine, end it
        // so we can start over again
        StopCoroutine("BasicHurt");
        // Debug.Log(gameObject.name + ": Stop Basic Hurt");
        // StopAllCoroutines();


        // Determine what able state / actions
        // the character should be in
        if (_currHealth <= 0) {
            _myState.SetAbleState(CharacterStateManager.AbleState.Dead);
            Debug.Log(gameObject.name + ": Dead");
            if (_charHealthBar) {
                _hurtSound.PlayDelayed(.4f);
                _charHealthBar.gameObject.SetActive(false);
                _myRagdoll.HandleRagdoll(stunTime);
            } 
        }

        else if (willRagdoll) {
            _hurtSound.PlayDelayed(.4f);
            Debug.Log(gameObject.name + ": Will Ragdoll");
            _myRagdoll.HandleRagdoll(stunTime);
        }
        
        else {
            // Debug.Log(gameObject.name + ": Do Basic Hurt");
            StartCoroutine("BasicHurt");
        }

    }    

    /// <summary>
    /// 
    /// Initiates a basic damage / hurt sequence for
    /// the character, alternating between able and current
    /// actions until the stun is over.
    /// 
    /// Runs whenever TakeDamage() is called without
    /// enabling ragdoll effects.
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator BasicHurt() {

        // Debug.Log(gameObject.name + ": Started Basic Hurt");

        _myState.SetAbleState(CharacterStateManager.AbleState.Incapacitated);
        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Stunned);

        _hurtSound.PlayDelayed(.4f);

        yield return new WaitForSeconds(_myStunTime);

        // * NOTE *
        // Within PlayerWeapon.cs in HitEnemy() we disabled the
        // NavMeshAgent to avoid bugs involving adding force, so
        // now we must re-enable it.
        // _myNavAgent.enabled = true;
        
        _myState.SetAbleState(CharacterStateManager.AbleState.Normal);
        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);

        // Debug.Log(gameObject.name + ": Finished Basic Hurt");

    }

}
