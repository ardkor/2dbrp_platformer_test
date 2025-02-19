using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    public int damage = 10; 
    public float knockbackForce = 5f; 
    public float knockbackDuration = 0.6f; 

    private void OnCollisionStay2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null && !player.Invincible && !player.Dead)
        {
            
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            PlayerController movement = player.GetComponent<PlayerController>();
            if (playerRb != null)
            {
                //player.StartInvincible(knockbackDuration);
                player.TakeDamage(damage, knockbackDuration);
                movement.DisableMovementForKnockback(knockbackDuration);
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                //movement.AddVelocity(knockbackDirection * knockbackForce);
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }
            
        }

    }

    
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject); 
    }
}
