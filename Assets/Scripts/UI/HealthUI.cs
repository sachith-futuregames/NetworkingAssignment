using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
   [SerializeField] Image healthBarImage;
   [SerializeField] Image shieldImage;
    [SerializeField] Health health;
   /// <summary>
   /// Start is called on the frame when a script is enabled just before
   /// any of the Update methods is called the first time.
   /// </summary>
   void Start()
   {
       health.currentHealth.OnValueChanged += UpdateHealthUI;
       health.ShieldHit.OnValueChanged += UpdateShieldUI;
   }

    private void UpdateHealthUI(int previousValue, int newValue)
    {
        healthBarImage.fillAmount = (float)newValue / 100;
    }

    private void UpdateShieldUI(int previousValue, int newValue)
    {
        Color _ShieldColor;
        switch(newValue)
        {
            case 1:
                _ShieldColor = Color.yellow;
                break;
            case 2:
                _ShieldColor = Color.green;
                break;
            default:
                _ShieldColor = Color.white;
                break;
        }
        shieldImage.color = _ShieldColor;
        shieldImage.enabled = newValue > 0;
    }
}
