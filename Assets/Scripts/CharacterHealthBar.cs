using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterHealthBar : MonoBehaviour
{

    [Header("Auto References")]
    [SerializeField] private Transform _myObj;
    [SerializeField] private Image _healthImage;
    [SerializeField] private TMP_Text _healthText;

    private void Awake() {
        _myObj = transform.root.Find("MyObj");
        _healthImage = transform.Find("Health").GetComponent<Image>();
        _healthText = transform.Find("Health Number").GetComponent<TMP_Text>();
    }

    private void FixedUpdate() {
        transform.LookAt(Camera.main.transform.position);
        transform.position = new Vector3(_myObj.position.x, _myObj.position.y + 2f, _myObj.position.z);
    }

    public void UpdateHealthBar(float currHealth, float maxHealth) {

        _healthImage.fillAmount = currHealth / maxHealth;
        if (currHealth <= 0) _healthText.text = "Dead";
        else _healthText.text = currHealth.ToString();

    }
}
