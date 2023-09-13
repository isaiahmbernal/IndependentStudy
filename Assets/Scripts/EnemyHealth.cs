using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterStateManager myState;
    [SerializeField] private Transform myObj;
    [SerializeField] private EnemyRagdoll myRagdoll;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform healthBar;
    [SerializeField] private UnityEngine.UI.Image healthImage;
    [SerializeField] private TMP_Text healthText;
    
    [Header("Health Variables")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currHealth;

    void FixedUpdate() {
        healthBar.transform.LookAt(Camera.main.transform.position);
        healthBar.transform.position = new Vector3(myObj.position.x, myObj.position.y + 1.5f, myObj.position.z);
    }

    public void TakeDamage(float damage, float stunTime, bool willRagdoll) {

        currHealth -= damage;
        healthImage.fillAmount = currHealth / maxHealth;

        if (currHealth < 0) healthText.text = "0";    
        else healthText.text = currHealth.ToString();

        StopCoroutine(BasicHurt(stunTime));

        if (willRagdoll) myRagdoll.HandleRagdoll(stunTime);
        else StartCoroutine(BasicHurt(stunTime));

    }    

    private IEnumerator BasicHurt(float stunTime) {

        myState.AbleStateChange(CharacterStateManager.AbleState.Incapacitated);
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Stunned);

        yield return new WaitForSeconds(stunTime);
        
        myState.AbleStateChange(CharacterStateManager.AbleState.Normal);
        myState.CurrentActionChange(CharacterStateManager.CurrentAction.Idle);

    }


}
