using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    public int damage = 10; // Урон, который враг наносит
    public float knockbackForce = 5f; // Сила отталкивания
    public float knockbackDuration = 0.2f; // Время отталкивания

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Наносим урон игроку
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized; // Направление от врага к игроку
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                // Если нужно замедлить движение игрока на время отталкивания
                StartCoroutine(DisableMovementForKnockback(playerRb, knockbackDuration));
            }
        }

    }

    private IEnumerator DisableMovementForKnockback(Rigidbody2D playerRb, float duration)
    {
        // Отключаем управление игроком (замените это на ваш метод отключения движения)
        PlayerController movement = playerRb.GetComponent<PlayerController>();
        if (movement != null)
        {
            movement.enabled = false; // Выключаем скрипт движения
        }

        yield return new WaitForSeconds(duration);

        // Включаем управление игроком обратно
        if (movement != null)
        {
            movement.enabled = true; // Включаем скрипт движения
        }
    }
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Осталось здоровья: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Враг уничтожен!");
        Destroy(gameObject); // Уничтожение врага
    }
}
