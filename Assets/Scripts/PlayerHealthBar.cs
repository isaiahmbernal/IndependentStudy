using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBar : MonoBehaviour
{
    
    [Header("Auto References")]
    [SerializeField] private Image _healthImage;
    [SerializeField] private TMP_Text _healthText;

    private void Awake() {
        _healthImage = transform.Find("Health").GetComponent<Image>();
        _healthText = transform.Find("Health Number").GetComponent<TMP_Text>();
    }

    public void UpdateHealthBar(float currHealth, float maxHealth) {

        _healthImage.fillAmount = currHealth / maxHealth;
        if (currHealth <= 0) _healthText.text = "Dead";
        else _healthText.text = currHealth.ToString();

    }

}
