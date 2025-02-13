using System.Collections;
using System;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    public Transform attackPoint; // Точка удара
    public float attackRange = 0.5f; // Радиус удара
    public LayerMask enemyLayer; // Слой врагов
    public int attackDamage = 20; // Урон атаки
    public float attackDuration = 0.5f; // Время действия атаки

    private bool isAttacking = false; // Чтобы не спамить атаку

    public event Action<bool> AttackingChanged;

    private void Update()
    {
        // Проверяем, нажата ли клавиша атаки и не происходит ли уже атака
        
    }
    public void PerformAttack()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        AttackingChanged?.Invoke(isAttacking);

        // Находим врагов в зоне удара
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // Наносим урон всем врагам в зоне
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Попал во врага: " + enemy.name);
            enemy.GetComponent<Enemy>()?.TakeDamage(attackDamage); // Убедитесь, что у врага есть метод TakeDamage
        }

        // Ждем время действия атаки
        yield return new WaitForSeconds(attackDuration);

        // Завершаем атаку
        isAttacking = false;
        AttackingChanged?.Invoke(isAttacking);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange); // Для наглядности зоны удара
    }
}
