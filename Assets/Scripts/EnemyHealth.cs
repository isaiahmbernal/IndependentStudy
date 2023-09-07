using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator anim;
    [SerializeField] private Transform healthBar;
    [SerializeField] private UnityEngine.UI.Image healthImage;
    [SerializeField] private TMP_Text healthText;
    
    [Header("Health Variables")]
    [SerializeField] private bool isHurt;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currHealth;

    public void handleHealthChange(float damage, float stunTime) {

        currHealth -= damage;
        healthImage.fillAmount = currHealth / maxHealth;
        if (currHealth < 0) {
            healthText.text = "0";    
        } else {
            healthText.text = currHealth.ToString();            
        }

        StopCoroutine(Hurt(stunTime));
        StartCoroutine(Hurt(stunTime));

    }

    void FixedUpdate() {

        healthBar.transform.LookAt(Camera.main.transform.position);

    }

    private IEnumerator Hurt(float stunTime) {

        anim.SetBool("isHurt", false);
        isHurt = true;
        anim.SetBool("isHurt", isHurt);
        yield return new WaitForSeconds(stunTime);
        isHurt = false;
        anim.SetBool("isHurt", isHurt);

    }

}
