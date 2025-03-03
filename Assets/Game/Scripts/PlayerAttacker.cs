using System.Collections;
using System;
using UnityEngine;

public class PlayerAttacker : MonoBehaviour
{
    public event Action<bool> AttackingChanged;

    [SerializeField] private Transform attackPoint; 
    [SerializeField] private float attackRange = 0.5f; 
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackDuration = 0.67f; 

    private Vector3 _attackPosition;
    private float _attackDelay = 0.5f;
    private bool _isAttacking = false; 
    private bool _isAttackZoneOnLeft;

    public void SetAttackZoneFlip(bool flipped)
    {
        _isAttackZoneOnLeft = flipped;
    }
    public void PerformAttack()
    {
        SoundManager.Instance.PlaySound(SoundManager.attackSound);
        StartCoroutine(AttackCoroutine());
    }

    private void Start()
    {
        _attackPosition = attackPoint.position;
    }

    private IEnumerator AttackCoroutine()
    {
        _isAttacking = true;
        AttackingChanged?.Invoke(_isAttacking);

        yield return new WaitForSeconds(_attackDelay);
        _attackPosition = _isAttackZoneOnLeft ? new Vector3(transform.position.x + (transform.position.x - attackPoint.position.x), attackPoint.position.y) : attackPoint.position;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackPosition, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            SoundManager.Instance.PlaySound(SoundManager.hitSound);
            enemy.GetComponent<Enemy>()?.TakeDamage(attackDamage); 
        }

        yield return new WaitForSeconds(attackDuration - _attackDelay);

        _isAttacking = false;
        AttackingChanged?.Invoke(_isAttacking);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_attackPosition, attackRange); 
    }
}
