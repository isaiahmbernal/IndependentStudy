using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

/// <summary>
/// Manages the health of enemy NPCs.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Auto References")]
    [SerializeField] private CharacterStateManager myState;
    [SerializeField] private Ragdoll myRagdoll;
    [SerializeField] private NavMeshAgent myNavAgent;

    [Header("Manual References")]
    [SerializeField] private Transform myObj;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform healthBar;
    [SerializeField] private UnityEngine.UI.Image healthImage;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private AudioSource hurtSound;
    
    [Header("Health Variables")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currHealth;
    [SerializeField] private float myStunTime;

    private void Awake() {

        // Auto-grab references
        myState = gameObject.GetComponent<CharacterStateManager>();
        myRagdoll = gameObject.GetComponent<Ragdoll>();
        myNavAgent = gameObject.GetComponent<NavMeshAgent>();

        healthText.text = currHealth.ToString();
        
    }

    private void FixedUpdate() {
        healthBar.transform.LookAt(Camera.main.transform.position);
        healthBar.transform.position = new Vector3(myObj.position.x, myObj.position.y + 1.5f, myObj.position.z);
    }

    /// <summary>
    /// 
    /// Method to be accessed by the object that hurt this
    /// character, passing in the damage that will be dealt
    /// to the health of this character, the time this
    /// character will be stunned for, and the time it will
    /// be ragdolled for.
    /// 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="stunTime"></param>
    /// <param name="willRagdoll"></param>
    public void TakeDamage(float damage, float stunTime, bool willRagdoll) {

        // Set all health / damage values
        currHealth -= damage;
        healthImage.fillAmount = currHealth / maxHealth;
        myStunTime = stunTime;

        if (currHealth <= 0) healthText.text = "Dead";    
        else healthText.text = currHealth.ToString();

        // If the character is already iterating
        // through the BasicHurt coroutine, end it
        // so we can start over again
        StopCoroutine("BasicHurt");


        // Determine what able state / actions
        // the character should be in
        if (currHealth <= 0) {
            myState.AbleStateChange(CharacterStateManager.AbleState.Dead);
            healthBar.gameObject.SetActive(false);
            hurtSound.PlayDelayed(.4f);
            myRagdoll.HandleRagdoll(stunTime);
        }

        else if (willRagdoll) {
            hurtSound.PlayDelayed(.4f);
            myRagdoll.HandleRagdoll(stunTime);
        }
        
        else StartCoroutine("BasicHurt");

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

        Debug.Log(gameObject.name + " started BasicHurt()");

        myState.AbleStateChange(CharacterStateManager.AbleState.Incapacitated);
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Stunned);

        hurtSound.PlayDelayed(.4f);

        yield return new WaitForSeconds(myStunTime);

        // * NOTE *
        // Within PlayerWeapon.cs in HitEnemy() we disabled the
        // NavMeshAgent to avoid bugs involving adding force, so
        // now we must re-enable it.
        myNavAgent.enabled = true;
        
        myState.AbleStateChange(CharacterStateManager.AbleState.Normal);
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);

    }


}
