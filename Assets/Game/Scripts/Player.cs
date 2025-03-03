using UnityEngine;
using System;
using System.Collections;

public class Player : MonoBehaviour
{
    public bool Invincible => _invincible;
    public bool Dead => _dead;

    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float minAlpha = 0.5f;
    [SerializeField] private float maxAlpha = 1f;
    [SerializeField] private bool loopFade = true;       

    private SpriteRenderer spriteRenderer;

    private int currentHealth;
    private bool _dead;
    private bool _invincible;
    private float _invincibleDuration;

    private Coroutine twinkleCoroutine;
    private Coroutine fadeCoroutine;

    public void TakeDamage(int damage, float knockbackDuration)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        EventBus.Instance.playerDamaged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0 && !_dead)
        {
            EventBus.Instance.playerDied?.Invoke();
        }
        else
        {
            StartInvincible(knockbackDuration);
        }
    }
    private void StartInvincible(float knockbackDuration)
    {
        _invincibleDuration = knockbackDuration;
        StartCoroutine(InvincibleTime());
    }

    private void OnEnable()
    {
        EventBus.Instance.playerDied += EndInvincible;
        EventBus.Instance.playerDied += OnDied;
        EventBus.Instance.gameFinished += OnGameFinished;
    }

    private void OnDisable()
    {
        EventBus.Instance.playerDied -= EndInvincible;
        EventBus.Instance.playerDied -= OnDied;
        EventBus.Instance.gameFinished -= OnGameFinished;
    }

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
    private void StartFading()
    {
        if (twinkleCoroutine == null)
        {
            twinkleCoroutine = StartCoroutine(FadeLoop());
        }
    }
    private void EndInvincible()
    {
        loopFade = false;
        _invincible = false;
        StopFading();

        Color color = spriteRenderer.color;
        color.a = maxAlpha;
        spriteRenderer.color = color;
    }

    private void StopFading()
    {
        if (twinkleCoroutine != null)
        {
            StopCoroutine(twinkleCoroutine);
            twinkleCoroutine = null;
        }
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private IEnumerator FadeLoop()
    {
        while (loopFade)
        {
            yield return fadeCoroutine = StartCoroutine(ChangeAlpha(minAlpha, fadeDuration));

            yield return StartCoroutine(ChangeAlpha(maxAlpha, fadeDuration));
        }
    }

    private IEnumerator ChangeAlpha(float targetAlpha, float duration)
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

        color.a = targetAlpha;
        spriteRenderer.color = color;
    }

    private void OnDied()
    {
        _dead = true;
        SoundManager.Instance.PlaySound(SoundManager.deathSound);
    }
    private void OnGameFinished()
    {
        EndInvincible();
        _invincible = true;
    }
}