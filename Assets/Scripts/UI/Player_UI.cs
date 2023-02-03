using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_UI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI staminaText;
    public TMPro.TextMeshProUGUI healthText;
    
    void Start()
    {
        staminaText.text = "100";
        healthText.text = "100";
    }
    void OnEnable()
    {
        FirstPersonController.OnStaminaChange += ChangeStaminaUI;
        FirstPersonController.OnHeal += ChangeHealthUI;
    }
    void OnDisable()
    {
        FirstPersonController.OnStaminaChange -= ChangeStaminaUI;
        FirstPersonController.OnHeal -= ChangeHealthUI;
    }
    void ChangeStaminaUI(float currentStamina)
    {
        staminaText.text = ((int)currentStamina).ToString();
    }
    void ChangeHealthUI(float currentHealth)
    {
        healthText.text = ((int)currentHealth).ToString();
    }
}
