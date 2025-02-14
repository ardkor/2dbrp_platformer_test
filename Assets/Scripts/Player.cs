using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool _dead;
    private bool _invincible;
    public SpriteRenderer spriteRenderer; // Ссылка на SpriteRenderer
    public float fadeDuration = 0.5f;       // Время на изменение прозрачности
    public float minAlpha = 0.5f;         // Минимальная прозрачность
    public float maxAlpha = 1f;          // Максимальная прозрачность
    public bool loopFade = true;         // Если true, эффект будет повторяться

    private float _invincibleDuration;

    private Coroutine fadeCoroutine;
    public bool Invincible => _invincible;
    public bool Dead => _dead;

    private void OnEnable()
    {
        EventBus.Instance.PlayerDied += EndInvincible;
    }

    private void OnDisable()
    {
        EventBus.Instance.PlayerDied -= EndInvincible;
    }

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    public void StartInvincible(float knockbackDuration)
    {
        _invincibleDuration = knockbackDuration;
        StartCoroutine(InvincibleTime());
    }
    private IEnumerator InvincibleTime()
    {
        fadeDuration = _invincibleDuration / 4;
        loopFade = true;
        _invincible = true;
        StartFading();
        yield return new WaitForSeconds(_invincibleDuration);
        EndInvincible();
    }
    public void EndInvincible()
    {
        loopFade = false;
        _invincible = false;
        StopFading();

        Color color = spriteRenderer.color;
        color.a = maxAlpha;
        spriteRenderer.color = color;
    }
    public IEnumerator FadeLoop()
    {
        while (loopFade)
        {
            yield return StartCoroutine(ChangeAlpha(minAlpha, fadeDuration));

            yield return StartCoroutine(ChangeAlpha(maxAlpha, fadeDuration));
        }
    }

    public IEnumerator ChangeAlpha(float targetAlpha, float duration)
    {
        if (spriteRenderer == null) yield break;

        Color color = spriteRenderer.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            color.a = newAlpha;
            spriteRenderer.color = color;
            yield return null;
        }

        // Устанавливаем точное значение прозрачности (исключаем ошибки округления)
        color.a = targetAlpha;
        spriteRenderer.color = color;
    }

    public void StopFading()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    public void StartFading()
    {
        if (fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeLoop());
        }
    }
    public void TakeDamage(int damage, float knockbackDuration)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        EventBus.Instance.PlayerDamaged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0 && !_dead)
        {
            Die();
        }
        else 
        {
            StartInvincible(knockbackDuration);
        }
    }

    private void Die()
    {
        _dead = true;
        EventBus.Instance.PlayerDied?.Invoke();
        SoundManager.Instance.PlaySound(SoundManager.deathSound);
    }
}