using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Attach to any weapons which are meant to
/// be wielded by the player, managing their damage,
/// stun time, attack count, sounds made when hitting
/// objects, etc.
/// </summary>
public class Weapon : MonoBehaviour
{

    [Header("Auto References")]
    [SerializeField] private CharacterStateManager _myState;
    [SerializeField] private CharacterAttackManager _myAttack;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _charObj;
    [SerializeField] private AudioSource _whoosh;
    [SerializeField] private AudioSource _hitWood;
    [SerializeField] private AudioSource _hitFlesh;

    [Header("Weapon Variables")]
    [SerializeField] private string _myName;
    [SerializeField] private float _damage;
    [SerializeField] private float _stunTime;
    [SerializeField] private float _knockBack;
    [SerializeField] private int _maxHits;
    [SerializeField] private int _hitsLeft;
    [SerializeField] private float _maxHitsBeforeRagdoll;
    [SerializeField] private float _currHitsForRagdoll;
    [SerializeField] private float _timeSinceLastHit;
    [SerializeField] private float _reflectTime;

    [Header("Enemy")]
    [SerializeField] private GameObject _lastEnemy;

     private void Awake() {
        _myState = transform.root.GetComponent<CharacterStateManager>();
        _myAttack = transform.root.GetComponent<CharacterAttackManager>();
        _charObj = transform.root.Find("MyObj");
        _anim = _charObj.GetComponent<Animator>();
        _whoosh = transform.Find("Sound").Find("Woosh").GetComponent<AudioSource>();
        _hitFlesh = transform.Find("Sound").Find("HitFlesh").GetComponent<AudioSource>();
        _hitWood = transform.Find("Sound").Find("HitWood").GetComponent<AudioSource>();
    }

    public void ResetHits() {
        _hitsLeft = _maxHits;
        Debug.Log(gameObject.name + " Hits Left: " + _hitsLeft);
    }

    public void PlaySound() {
        _whoosh.PlayDelayed(.5f);
    }

    void OnTriggerEnter(Collider collisionInfo) {

        // If the weapon hits an INTERACTABLE OBJECT, WHILE
        // attacking, WHILE it still has hits left during the
        // current attack sequence, and WHILE the time since the last
        // hit is greater than the set time between attacks
        if (collisionInfo.gameObject.tag == "Interactable"
            && _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Attacking
            && _hitsLeft >= 1) {
            
            // Run the method to hit objects
            HitObject(collisionInfo);
            
        }

        // The same as hitting an interactable object,
        // but in this case an ENEMY
        else if ((collisionInfo.gameObject.tag == "Enemy"
            || (collisionInfo.gameObject.tag == "Player" && transform.root.gameObject.name != "Player"))
            && _myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Attacking
            && _hitsLeft >= 1) {

            CharacterStateManager otherState = collisionInfo.gameObject.GetComponent<CharacterStateManager>();

            // If the enemy is currently NOT in a ragdoll state
            if (otherState.GetCurrentAction() != CharacterStateManager.CurrentAction.Ragdoll
                && otherState.GetAbleState() != CharacterStateManager.AbleState.Dead) {

                // Run the method to hit the enemy
                HitEnemy(collisionInfo);

            }
            
        }
        
    }

    /// <summary>
    /// 
    /// Called whenever the weapon hits an object,
    /// and determines what to do with the object
    /// based on its children's names and their tags.
    /// 
    /// </summary>
    /// <param name="collisionInfo"></param>
    private void HitObject(Collider collisionInfo) {

        // Decrement the remaining hits (or interactions)
        // the weapon has left during the current attack sequence
        _hitsLeft -= 1;
        _timeSinceLastHit = 0;

        // Add force to the object
        collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(_charObj.forward * _knockBack * 2, ForceMode.Force);

        // Get all children of the object to find
        // if it is breakable or has a material sound
        Transform[] children = collisionInfo.gameObject.GetComponentsInChildren<Transform>();

        bool reflected = true;

        for (int i = 0; i < children.Length; i++) {

            // If the child object's name is HitSound
            if (children[i].gameObject.name == "HitSound") {

                // Depending on the tag of the object,
                // play the correct sounds which are
                // attached to this weapon object
                switch (children[i].gameObject.tag) {
                    case "Wood":
                        Debug.Log("I Hit Wood: " + children[i].gameObject.GetInstanceID());
                        _whoosh.Stop();
                        _hitWood.Play();
                        break;
                }

            }

            // If the child object's name is breakable,
            // call its Break() method
            else if (children[i].gameObject.name == "Breakable") {
                collisionInfo.gameObject.GetComponent<ObjectBreakable>().Break();  
                reflected = false;
            }

        }

        if (reflected) StartCoroutine("Reflected");

    }

    /// <summary>
    /// 
    /// Called whenever the weapon hits an enemy,
    /// and determines what to do with the enemy.
    /// 
    /// </summary>
    /// <param name="collisionInfo"></param>
    private void HitEnemy(Collider collisionInfo) {

        Debug.Log("I Hit Enemy: " + collisionInfo.gameObject.GetInstanceID());

        // Decrement the remaining hits (or interactions)
        // the weapon has left during the current attack sequence
        _hitsLeft -= 1;
        _timeSinceLastHit = 0;

        // Stop the original weapon sound and
        // play the correct enemy hit sound
        _whoosh.Stop();
        _hitFlesh.PlayDelayed(.05f);

        // * NOTE *
        // The NavMeshAgent of the enemy MUST BE DISABLED to prevent
        // bugs after applying force to the object. It seems to lose
        // track of its true position and no longer correctly moves
        // to positions when using SetDestination().
        // collisionInfo.gameObject.GetComponent<NavMeshAgent>().enabled = false;

        // If this is the enemy we PREVIOUSLY HIT
        if (collisionInfo.gameObject.GetInstanceID() == _lastEnemy.GetInstanceID()) {

            // Increment the combo counter for how
            // many hits it will take until the enemy
            // goes into a ragdoll state
            _currHitsForRagdoll += 1;

            // If we HAVE hit the ragdoll hit count
            if (_currHitsForRagdoll >= _maxHitsBeforeRagdoll) {
                _currHitsForRagdoll = 0;
                // collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(_playerObj.forward * _knockBack * 2, ForceMode.Force);
                // Passing true within TakeDamage() will enable ragdoll
                collisionInfo.gameObject.GetComponent<CharacterHealthManager>().TakeDamage(_damage, _stunTime, true);
            } 

            // If we HAVE NOT reached the ragdoll hit count
            else {
                // collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(_playerObj.forward * _knockBack, ForceMode.Force);
                // Passing false within TakeDamage() will NOT enable ragdoll
                collisionInfo.gameObject.GetComponent<CharacterHealthManager>().TakeDamage(_damage, _stunTime, false);
            } 
            
        // If this IS NOT our previously hit enemy
        } else {

            // Keep track of this enemy
            _lastEnemy = collisionInfo.gameObject;

            // Reset the ragdoll hit combo
            _currHitsForRagdoll = 1;
            
            // collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(_playerObj.forward * _knockBack, ForceMode.Force);
            collisionInfo.gameObject.GetComponent<CharacterHealthManager>().TakeDamage(_damage, _stunTime, false);

        }
   
    }

    private IEnumerator Reflected() {

        Debug.Log("Reflected Coroutine");

        _myAttack.StopAllAttacks();
        string animName = _myName + "_Reflected";
        _anim.Play(animName);

        _myState.SetAbleState(CharacterStateManager.AbleState.Incapacitated);
        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Reflected);

        yield return new WaitForSeconds(_reflectTime);

        _myState.SetAbleState(CharacterStateManager.AbleState.Normal);
        _myState.SetCurrentAction(CharacterStateManager.CurrentAction.Idle);

        ResetHits();
        _myAttack.ResetLightAttackStage();

    }

}
