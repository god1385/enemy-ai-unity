using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour, IDamagable
{
    [Range(0f, 50f)]
    [SerializeField] private float maxHealth;
    [SerializeField] private SpriteRenderer characterSprite;
    [SerializeField] private float damageFlashDuration = 0.3f;

    public float currentHealth;
    private float hitFlashTimer;
    private Color defaultColor;

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;

    private void Start()
    {
        defaultColor = characterSprite.color;
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        StopCoroutine(DamageFlash());
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0f)
            Destroy(gameObject);
    }

    private IEnumerator DamageFlash()
    {
        float time = 0f;
        Color startColor = characterSprite.color;
        Color targetColor = Color.red;

        while (time < damageFlashDuration / 2)
        {
            time += Time.deltaTime;
            characterSprite.color = Color.Lerp(startColor, targetColor, time);
            yield return null;
        }

        time = 0f;
        startColor = characterSprite.color;
        targetColor = defaultColor;

        while (time < damageFlashDuration / 2)
        {
            time += Time.deltaTime;
            characterSprite.color = Color.Lerp(startColor, targetColor, time);
            yield return null;
        }

        characterSprite.color = defaultColor;
    }
}
