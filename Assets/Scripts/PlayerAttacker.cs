using System.Collections;
using System;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    public Transform attackPoint; // Точка удара
    public float attackRange = 0.5f; // Радиус удара
    public LayerMask enemyLayer; // Слой врагов
    public int attackDamage = 20; // Урон атаки
    public float attackDuration = 0.6f; // Время действия атаки

    private bool isAttacking = false; // Чтобы не спамить атаку

    public event Action<bool> AttackingChanged;
    private Vector3 attackPosition;

    private float attackDelay = 0.5f;
    private bool _flipped; 

    private void Start()
    {
        attackPosition = attackPoint.position;
    }
    public void UpdateFlipped(bool flipped) 
    {
        _flipped = flipped;
    }
    public void PerformAttack()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        AttackingChanged?.Invoke(isAttacking);

        yield return new WaitForSeconds(attackDelay);
        attackPosition = _flipped ? new Vector3(transform.position.x + (transform.position.x - attackPoint.position.x), attackPoint.position.y) : attackPoint.position;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Попал во врага: " + enemy.name);
            enemy.GetComponent<Enemy>()?.TakeDamage(attackDamage); 
        }

        yield return new WaitForSeconds(attackDuration - attackDelay);

        isAttacking = false;
        AttackingChanged?.Invoke(isAttacking);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPosition, attackRange); 
        //Gizmos.DrawWireSphere(attackPoint.position, attackRange); 
        //Gizmos.DrawWireSphere( new Vector3(attackPosition.x, attackPosition.y,  0), attackRange); 
    }
}
