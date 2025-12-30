using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyChase : EnemyState
{
    private Vector2 lastSeenPosition;
    private float lookTimer = 0f;
    private Vector2 direction;
    private Vector2 targetPos;
    private Vector2 target;
    public EnemyChase(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        lookTimer = 0f;
        Debug.Log("State:Chase");
        enemyObject.ClearCheckedPositions();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Execute()
    {
        if (enemyObject.CanSeeTarget)
        {
            target = enemyObject.CurrentSeenTargetPosition.position;
            direction = target - enemyObject.Rb.position;
            targetPos = Vector2.MoveTowards(enemyObject.Rb.position, enemyObject.Rb.position + direction, enemyObject.MoveSpeed * Time.fixedDeltaTime);
            enemyObject.RotateToPoint(target);
            lastSeenPosition = target;
            enemyObject.Rb.MovePosition(targetPos);
            lookTimer = 0f;

            if (Vector2.Distance(enemyObject.Rb.position, target) <= enemyObject.AttackRadius)
            {
                enemyObject.ChangeState(new EnemyAttack(enemyObject));
            }
        }
        else if (Vector2.Distance((Vector2)enemyObject.transform.position, lastSeenPosition) > 0.2f)
        {
            enemyObject.RotateToPoint(lastSeenPosition);
            direction = lastSeenPosition - enemyObject.Rb.position;
            targetPos = Vector2.MoveTowards(enemyObject.Rb.position, enemyObject.Rb.position + direction, enemyObject.MoveSpeed * Time.fixedDeltaTime);
            enemyObject.Rb.MovePosition(targetPos);
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
}
