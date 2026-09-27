using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour

{
    [Header("UI")]
    public Slider healthSlider;
    public Slider staminaSlider;
    [SerializeField] private HealthStaminaSystem PlayerStats;
  
    public void UpdateSliders()
    {
        if (PlayerStats == null )
        {
            return;
        }
        if (healthSlider != null)
            healthSlider.value = PlayerStats.currentHealth;
        if (staminaSlider != null)
            staminaSlider.value = PlayerStats.currentStamina;
    }
}
