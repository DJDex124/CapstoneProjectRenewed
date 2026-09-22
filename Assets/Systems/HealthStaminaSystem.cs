using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthStaminaSystem : MonoBehaviour
{
    
    public bool canSprint = true;
    public bool canJump = true;
    public bool canLoseStamina = true;
    


    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public Slider healthSlider;
    public Slider staminaSlider;

    private void Awake()
    {
        
    }
    public void UpdateSliders()
    {

        if (healthSlider != null)
            healthSlider.value = currentHealth;
        if (staminaSlider != null)
            staminaSlider.value = currentStamina;
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (staminaSlider != null)
        {
            staminaSlider.minValue = 0f;
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
    }

    void Update()
    {
        UpdateSliders();

        if (currentHealth <= 0)
            GameManager.current.Die();
        if (currentStamina <= 0)
                canSprint = false;
            else
                canSprint = true;
        if (currentStamina <= 10)
            canJump = false;
        else
            canJump = true;


    }


    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        Debug.Log("Health: " + currentHealth);
        if (currentHealth <= 0)
        {
            GameManager.current.Die();
        }
    }
    public void UseStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina - amount, 0f, maxStamina);

    }
    public void RegenerateStamina(float amount)
    {
        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);

    }


    public IEnumerator disableStamina(float time)
    {
        canLoseStamina = false;
        
        yield return new WaitForSeconds(time);
  
        canLoseStamina = true;

    }
    public void healPlayer(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        ;
    }
}
