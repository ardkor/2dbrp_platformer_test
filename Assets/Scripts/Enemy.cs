using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    public int damage = 10; 
    public float knockbackForce = 5f; 
    public float knockbackDuration = 0.2f; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized; // ����������� �� ����� � ������
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

               // StartCoroutine(DisableMovementForKnockback(playerRb, knockbackDuration));
            }
        }

    }

    private IEnumerator DisableMovementForKnockback(Rigidbody2D playerRb, float duration)
    {
        // ��������� ���������� ������� (�������� ��� �� ��� ����� ���������� ��������)
        PlayerController movement = playerRb.GetComponent<PlayerController>();
        if (movement != null)
        {
            movement.enabled = false; // ��������� ������ ��������
        }

        yield return new WaitForSeconds(duration);

        // �������� ���������� ������� �������
        if (movement != null)
        {
            movement.enabled = true; // �������� ������ ��������
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
