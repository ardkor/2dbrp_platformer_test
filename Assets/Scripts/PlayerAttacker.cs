using System.Collections;
using System;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    public Transform attackPoint; // ����� �����
    public float attackRange = 0.5f; // ������ �����
    public LayerMask enemyLayer; // ���� ������
    public int attackDamage = 20; // ���� �����
    public float attackDuration = 0.5f; // ����� �������� �����

    private bool isAttacking = false; // ����� �� ������� �����

    public event Action<bool> AttackingChanged;

    private void Update()
    {
        // ���������, ������ �� ������� ����� � �� ���������� �� ��� �����
        
    }
    public void PerformAttack()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        AttackingChanged?.Invoke(isAttacking);

        // ������� ������ � ���� �����
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        // ������� ���� ���� ������ � ����
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("����� �� �����: " + enemy.name);
            enemy.GetComponent<Enemy>()?.TakeDamage(attackDamage); // ���������, ��� � ����� ���� ����� TakeDamage
        }

        // ���� ����� �������� �����
        yield return new WaitForSeconds(attackDuration);

        // ��������� �����
        isAttacking = false;
        AttackingChanged?.Invoke(isAttacking);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange); // ��� ����������� ���� �����
    }
}
