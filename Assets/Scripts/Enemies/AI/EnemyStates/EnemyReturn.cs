using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyReturn : EnemyState
{
    public EnemyReturn(Enemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemyObject.ClearCheckedPositions();
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Execute()
    {
        enemyObject.ReturnToDefault();

        Vector2 direction = ((Vector2)enemyObject.StartPosition - enemyObject.Rb.position);
        Vector2 returnPosition = Vector2.MoveTowards(enemyObject.Rb.position, enemyObject.Rb.position + direction, enemyObject.MoveSpeed * Time.fixedDeltaTime);
        enemyObject.Rb.MovePosition(returnPosition);

        if (Vector2.Distance(enemyObject.Rb.position, enemyObject.StartPosition) < 0.05f)
        {
            enemyObject.ChangeState(enemyObject.ReturnDefaultState());
            return;
        }

        if (enemyObject.CurrentNoise != null || enemyObject.CanSeeTarget)
        {
            if (enemyObject.Suspicion >= Enemy.MiddleSuspicion)
            {
                EnemyState enemyState = enemyObject.EvaluateThreat();

                if (enemyState != null)
                    enemyObject.ChangeState(enemyState);
            }
        }


    }
}
