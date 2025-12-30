using UnityEngine;
using static UnityEngine.UI.Image;

public class EnemyAttack : EnemyState
{
    private float attackTimer;
    private float lookTimer;
    private Vector2 lastSeenPosition;
    private Vector2 target;
    public EnemyAttack(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        attackTimer = 0f;
        Debug.Log("State:Attack");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Execute()
    {
        if (enemyObject.CanSeeTarget && enemyObject.CurrentSeenTargetPosition != null)
        {
            target = enemyObject.CurrentSeenTargetPosition.position;
            float direction = Vector2.Distance(enemyObject.Rb.position, target);

            if (direction > enemyObject.AttackRadius + 0.2f)
            {
                enemyObject.ChangeState(new EnemyChase(enemyObject));
                return;
            }
            enemyObject.RotateToPoint(target);
            attackTimer += Time.fixedDeltaTime;

            if (attackTimer >= enemyObject.AttackCooldown)
            {
                attackTimer = 0f;
                Attack();

            }
            lookTimer = 0f;
        }
        else
        {
            lookTimer += Time.fixedDeltaTime;
            if (lookTimer >= 1f)
            {
                enemyObject.ChangeState(new EnemySearch(enemyObject));
            }
        }
    }

    private void Attack()
    {
        Vector2 direction = (Vector2)enemyObject.CurrentSeenTargetPosition.position - enemyObject.Rb.position;
        RaycastHit2D hit = Physics2D.Raycast(enemyObject.transform.position, direction, enemyObject.AttackRadius, enemyObject.PlayerLayer);

        if (hit.collider != null)
        {
            if (hit.transform.TryGetComponent<IDamagable>(out var hitTarget))
                hitTarget.TakeDamage(enemyObject.AttackDamage);
        }
        Debug.DrawRay(enemyObject.transform.position, direction * enemyObject.AttackRadius, Color.red);
    }
}
