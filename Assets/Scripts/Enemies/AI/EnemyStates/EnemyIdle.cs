using UnityEngine;

public class EnemyIdle : EnemyState
{
    public EnemyIdle(Enemy enemy) : base(enemy) { }

    private Collider2D[] collderArray;

    public override void Enter()
    {
        enemyObject.ReturnToDefault();
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Execute()
    {
        if (enemyObject.CurrentNoise != null || enemyObject.CanSeeTarget)
        {
            EnemyState enemyState = enemyObject.EvaluateThreat();

            if (enemyState != null)
                enemyObject.ChangeState(enemyState);
        }
    }
}
