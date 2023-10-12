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
public class PlayerWeapon : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private ThirdPersonCamera cam;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private CharacterStateManager myState;
    // [SerializeField] private Rigidbody rb;

    [Header("Audio")]
    [SerializeField] private AudioSource whoosh;
    [SerializeField] private AudioSource hitWood;
    [SerializeField] private AudioSource hitFlesh;

    [Header("Weapon Variables")]
    [SerializeField] private float damage;
    [SerializeField] private float stunTime;
    [SerializeField] private float knockBack;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float totalAttackTime;
    [SerializeField] private int maxHits;
    [SerializeField] private int hitsLeft;
    [SerializeField] private float maxHitsBeforeRagdoll;
    [SerializeField] private float currHitsForRagdoll;
    [SerializeField] private float timeSinceLastHit;

    [Header("Enemy")]
    [SerializeField] private GameObject lastEnemy;

    public float GetTotalAttackTime() {
        return totalAttackTime;
    }

    public void ResetHits() {
        hitsLeft = maxHits;
    }

    public void PlaySound() {
        whoosh.PlayDelayed(.5f);
    }

    private void Update() {
        if (timeSinceLastHit < timeBetweenAttacks) {
            timeSinceLastHit += Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider collisionInfo) {

        // If the weapon hits an INTERACTABLE OBJECT, WHILE
        // attacking, WHILE it still has hits left during the
        // current attack sequence, and WHILE the time since the last
        // hit is greater than the set time between attacks
        if (collisionInfo.gameObject.tag == "Interactable"
            && myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Attacking
            && hitsLeft >= 1
            && timeSinceLastHit >= timeBetweenAttacks) {
            
            // Run the method to hit objects
            HitObject(collisionInfo);
            
        }

        // The same as hitting an interactable object,
        // but in this case an ENEMY
        else if (collisionInfo.gameObject.tag == "Enemy"
            && myState.GetCurrentAction() == CharacterStateManager.CurrentAction.Attacking
            && hitsLeft >= 1
            && timeSinceLastHit >= timeBetweenAttacks) {

            // If the enemy is currently NOT in a ragdoll state
            if (collisionInfo.gameObject.GetComponent<CharacterStateManager>().GetCurrentAction()
                != CharacterStateManager.CurrentAction.Ragdoll) {

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
        hitsLeft -= 1;
        timeSinceLastHit = 0;

        // Add force to the object
        collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(playerObj.forward * knockBack * 2, ForceMode.Force);

        // Get all children of the object to find
        // if it is breakable or has a material sound
        Transform[] children = collisionInfo.gameObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; i++) {

            // If the child object's name is HitSound
            if (children[i].gameObject.name == "HitSound") {

                // Depending on the tag of the object,
                // play the correct sounds which are
                // attached to this weapon object
                switch (children[i].gameObject.tag) {
                    case "Wood":
                        Debug.Log("I Hit Wood: " + children[i].gameObject.GetInstanceID());
                        whoosh.Stop();
                        hitWood.Play();
                        break;
                }

            }

            // If the child object's name is breakable,
            // call its Break() method
            else if (children[i].gameObject.name == "Breakable") {
                collisionInfo.gameObject.GetComponent<ObjectBreakable>().Break();        
            }

        }
    }

    /// <summary>
    /// 
    /// Called whenever the weapon hits an enemy,
    /// and determines what to do with the enemy.
    /// 
    /// </summary>
    /// <param name="collisionInfo"></param>
    private void HitEnemy(Collider collisionInfo) {

        // Decrement the remaining hits (or interactions)
        // the weapon has left during the current attack sequence
        hitsLeft -= 1;
        timeSinceLastHit = 0;

        // Stop the original weapon sound and
        // play the correct enemy hit sound
        whoosh.Stop();
        hitFlesh.PlayDelayed(.05f);

        Debug.Log("I Hit Enemy: " + collisionInfo.gameObject.GetInstanceID());

        // * NOTE *
        // The NavMeshAgent of the enemy MUST BE DISABLED to prevent
        // bugs after applying force to the object. It seems to lose
        // track of its true position and no longer correctly moves
        // to positions when using SetDestination().
        collisionInfo.gameObject.GetComponent<NavMeshAgent>().enabled = false;

        // If this is the enemy we PREVIOUSLY HIT
        if (collisionInfo.gameObject.GetInstanceID() == lastEnemy.GetInstanceID()) {

            // Increment the combo counter for how
            // many hits it will take until the enemy
            // goes into a ragdoll state
            currHitsForRagdoll += 1;

            // If we HAVE hit the ragdoll hit count
            if (currHitsForRagdoll >= maxHitsBeforeRagdoll) {
                currHitsForRagdoll = 0;
                // collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(playerObj.forward * knockBack * 2, ForceMode.Force);
                // Passing true within TakeDamage() will enable ragdoll
                collisionInfo.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage, stunTime, true);
            } 

            // If we HAVE NOT reached the ragdoll hit count
            else {
                // collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(playerObj.forward * knockBack, ForceMode.Force);
                // Passing false within TakeDamage() will NOT enable ragdoll
                collisionInfo.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage, stunTime, false);
            } 
            
        // If this IS NOT our previously hit enemy
        } else {

            // Keep track of this enemy
            lastEnemy = collisionInfo.gameObject;

            // Reset the ragdoll hit combo
            currHitsForRagdoll = 1;
            
            // collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(playerObj.forward * knockBack, ForceMode.Force);
            collisionInfo.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage, stunTime, false);

        }
   
    }

}
