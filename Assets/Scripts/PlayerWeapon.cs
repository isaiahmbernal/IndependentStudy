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


    void OnTriggerEnter(Collider collisionInfo) {

        if (collisionInfo.gameObject.tag == "Enemy" && pAtk.getIsAttacking()) {

            StartCoroutine(Hit(collisionInfo));
            
        }
        
    }

    private IEnumerator Hit(Collider collisionInfo) {

        yield return new WaitForSeconds(timeBeforeAttack);

        while (hitsLeft > 0) {

            collisionInfo.gameObject.GetComponent<EnemyHealth>().handleHealthChange(damage, stunTime);
            collisionInfo.gameObject.GetComponent<Rigidbody>().AddForce(playerObj.forward * knockBack, ForceMode.Force);
            hitsLeft -= 1;
            print("I Just Hit: " + collisionInfo.gameObject.tag);
            yield return new WaitForSeconds(timeBetweenAttacks);

        }

    }

}
