using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool _dead;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        EventBus.Instance.PlayerDamaged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0 && !_dead)
        {
            Die();
        }
    }

    private void Die()
    {
        _dead = true;
        EventBus.Instance.PlayerDied?.Invoke();
    }
}