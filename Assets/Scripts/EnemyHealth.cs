using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnityEngine.UI.Image healthBar;
    
    [Header("Health Variables")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float currHealth;

    public void handleHealthChange(float damage) {

        currHealth -= damage;
        healthBar.fillAmount = currHealth / maxHealth;

    }

}
