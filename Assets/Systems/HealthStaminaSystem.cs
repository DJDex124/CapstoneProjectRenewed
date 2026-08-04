using System.Collections;
using UnityEngine;

public class HealthStaminaSystem : MonoBehaviour
{
    public static HealthStaminaSystem current { get; private set; }

    public bool canSprint = true;
    public bool canJump = true;
    public bool canLoseStamina = true;


    [Header("Health")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;


    private void Awake()
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

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (UIManager.current == null)
        {
            Debug.LogWarning("UIManager not found. Make sure it is loaded before GameManager.");
            return;
        }
        if (UIManager.current.healthSlider != null)
        {
            UIManager.current.healthSlider.minValue = 0f;
            UIManager.current.healthSlider.maxValue = maxHealth;
            UIManager.current.healthSlider.value = currentHealth;
        }

        if (UIManager.current.staminaSlider != null)
        {
            UIManager.current.staminaSlider.minValue = 0f;
            UIManager.current.staminaSlider.maxValue = maxStamina;
            UIManager.current.staminaSlider.value = currentStamina;
        }



    }

    // Update is called once per frame
    void Update()
    {
        if (UIManager.current == null)
        {
            Debug.LogWarning("UIManager not found. Make sure it is loaded before GameManager.");
            return;
        }
        UIManager.current.UpdateSliders();

        if (currentHealth <= 0)
            //Die();
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
        Debug.Log("Health: " + currentHealth);
    }
}
