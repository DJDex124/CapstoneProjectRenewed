using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour

{
    public static UIManager current { get; private set; }

    [Header("UI")]
    public Slider healthSlider;
    public Slider staminaSlider;

    void Awake()
    {
        if (current != null && current != this)
        {
            Destroy(gameObject);
        }
        else
        {
            current = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void UpdateSliders()
    {
        if (healthSlider != null)
            healthSlider.value = GameManager.current.currentHealth;
        if (staminaSlider != null)
            staminaSlider.value = GameManager.current.currentStamina;
    }
}
