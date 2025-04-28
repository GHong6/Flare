using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private float health;
    private float stamina;
    private float lerpTimer;

    [Header("Health Bar")]
    public float maxHealth = 100f;
    public float chipSpeed = 1f;

    public Image HP1;
    public Image HP2;
    public Image HP3;

    public Color activeColor = Color.white;  // Full health color
    public Color grayColor = Color.gray;     // Gray color for low 

    [Header("Stamina Bar")]
    public float maxStamina = 10f;
    public float staminaDrainRate = 5f;   // Drain rate increased for faster effect
    public float staminaRegenRate = 3f;   // Faster regeneration
    public float regenDelay = 1f;         // Delay before regeneration starts
    public Image frontStaminaBar;




    public bool isRegeneratingStamina = false;
    private Coroutine regenCoroutine;

    [Header("Damage Overlay")]
    public Image overlay;
    public float duration;
    public float fadeSpeed;

    private float durationTimer;

    void Start()
    {
        health = maxHealth;
        stamina = maxStamina;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
        UpdateHealthUI();
        UpdateStaminaUI();
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        UpdateHealthUI();
        UpdateStaminaUI();
        //Debug.Log(stamina);

        if (overlay.color.a > 0)
        {
            if (health < maxHealth * 0.25f) return;
            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    public void UpdateHealthUI()
    {

        float hFraction = health / maxHealth;
        
        UpdateHPIndicators();
    }

    private void UpdateHPIndicators()
    {
        float healthPercentage = health / maxHealth;

        if (healthPercentage > 0.75f)
        {
            HP1.color = activeColor;
            HP2.color = activeColor;
            HP3.color = activeColor;
        }
        else if (healthPercentage > 0.50f)
        {
            HP1.color = grayColor;
            HP2.color = activeColor;
            HP3.color = activeColor;
        }
        else if (healthPercentage > 0.25f)
        {
            HP1.color = grayColor;
            HP2.color = grayColor;
            HP3.color = activeColor;
        }
        else
        {
            HP1.color = grayColor;
            HP2.color = grayColor;
            HP3.color = grayColor;
            overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1); // Overlay stays on
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    void UpdateStaminaUI()
    {
        if (frontStaminaBar != null)
        {
            frontStaminaBar.fillAmount = stamina / maxStamina;
            //Debug.Log($"Stamina UI Updated: {stamina}");
        }
    }

    public void SetStamina(float newStamina)
    {
        stamina = newStamina;
        UpdateStaminaUI();

        // Stop regen if stamina is being manually updated
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            isRegeneratingStamina = false;
        }
    }

    public void ConsumeStamina()
    {
        // Only allow stamina consumption if stamina is greater than 0 and not regenerating
        if (stamina > 0 && !isRegeneratingStamina)
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            UpdateStaminaUI();

            //Debug.Log($"Consuming Stamina: {stamina}");

            // If stamina is drained, start regeneration
            if (stamina <= 0)
            {
                if (regenCoroutine == null)
                {
                    regenCoroutine = StartCoroutine(RegenerateStamina());
                }
            }
        }
        else if (stamina == 0 && regenCoroutine == null)
        {
            // Optionally debug or inform that stamina is empty and needs to regenerate
            // Debug.Log("Stamina is empty, waiting for regeneration to reach 100%");
        }
    }

    public void StartStaminaRegen()
    {
        if (regenCoroutine == null)
        {
            regenCoroutine = StartCoroutine(RegenerateStamina());
        }
    }

    private IEnumerator RegenerateStamina()
    {
        isRegeneratingStamina = true;
        //Debug.Log("Stamina Regeneration Started...");

        yield return new WaitForSeconds(regenDelay);

        while (stamina < maxStamina)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            UpdateStaminaUI();
            //Debug.Log($"Regenerating Stamina: {stamina}");
            yield return null;
        }

        
        isRegeneratingStamina = false;
        regenCoroutine = null;
    }

    

    public bool CanUseStamina()
{
    return stamina > 0; // Allows movement to check if sprinting is possible
}

    public float GetStamina()
    {
        return stamina; // Return current stamina value
    }

}
