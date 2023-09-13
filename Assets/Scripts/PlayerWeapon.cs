using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private ThirdPersonCamera cam;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObj;
    [SerializeField] private PlayerAttack pAtk;
    [SerializeField] private PlayerMovement pMove;

    [Header("Weapon Variables")]
    [SerializeField] private float damage;
    [SerializeField] private float stunTime;
    [SerializeField] private float knockBack;
    [SerializeField] private float timeBeforeAttack;
    [SerializeField] private float timeBetweenAttacks;
    [SerializeField] private float totalAttackTime;
    [SerializeField] private float attackCooldown;
    [SerializeField] private int maxHits;
    [SerializeField] private int hitsLeft;
    [SerializeField] private float maxHitsBeforeRagdoll;
    [SerializeField] private float currHitsForRagdoll;
    [SerializeField] private float timeSinceLastHit;

    [Header("Enemy")]
    [SerializeField] private GameObject enemy;

    public float getDamage() {
        return damage;
    }

    public float getTotalAttackTime() {
        return totalAttackTime;
    }

    public float getAttackCooldown() {
        return attackCooldown;
    }

    public void setHitsLeft() {
        hitsLeft = maxHits;
    }

    private void FixedUpdate() {
        if (timeSinceLastHit < attackCooldown) {
            timeSinceLastHit += Time.deltaTime;
        }
    }


    void OnTriggerEnter(Collider collisionInfo) {

        // If We Hit An Enemy
        if (collisionInfo.gameObject.tag == "Enemy" && pAtk.getIsAttacking() && timeSinceLastHit >= attackCooldown) {

            timeSinceLastHit = 0;

            // If This *IS* Our Previously Hit Enemy
            if (collisionInfo.gameObject.GetInstanceID() == enemy.GetInstanceID()) {

                currHitsForRagdoll += 1;

                // If We *HAVE* Reached The Ragdoll Hit Count
                if (currHitsForRagdoll >= maxHitsBeforeRagdoll) {
                    currHitsForRagdoll = 0;
                    print("Will Ragdoll");
                    StartCoroutine(Hit(collisionInfo, true));
                
                // If We *HAVE NOT* Reached The Ragdoll Hit Count
                } else {
                    StartCoroutine(Hit(collisionInfo, false));   
                    print("Will NOT Ragdoll");
                } 
                
            // If This *IS NOT* Our Previously Hit Enemy
            } else {

                enemy = collisionInfo.gameObject;
                currHitsForRagdoll = 1;
                print("Will NOT Ragdoll");
                StartCoroutine(Hit(collisionInfo, false));

            }
            
        }
        
    }

    private IEnumerator Hit(Collider collisionInfo, bool willRagdoll) {

        GameObject enemy = collisionInfo.gameObject;

        yield return new WaitForSeconds(timeBeforeAttack);

        while (hitsLeft > 1) {

            enemy.GetComponent<EnemyHealth>().TakeDamage(damage, stunTime, false);
            hitsLeft -= 1;
            yield return new WaitForSeconds(timeBetweenAttacks);

        }

        if (willRagdoll) {
            enemy.GetComponent<Rigidbody>().AddForce(playerObj.forward * knockBack * 2, ForceMode.Force);
        } else {
            enemy.GetComponent<Rigidbody>().AddForce(playerObj.forward * knockBack, ForceMode.Force);
        }
        
        enemy.GetComponent<EnemyHealth>().TakeDamage(damage, stunTime, willRagdoll);

        print("I Just Hit: " + enemy.tag);

    }

}
