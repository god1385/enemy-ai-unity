using UnityEngine;

public class EnemyInvestigate : EnemyState
{
    private float stateEnterTime;
    bool CanExit => Time.time - stateEnterTime > 0.5f;
    public EnemyInvestigate(Enemy enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("State:Invest");
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Execute()
    {
        if (enemyObject.CurrentNoise == null)
        {
            enemyObject.ChangeState(new EnemySearch(enemyObject));
            return;
        }

        enemyObject.RotateToPoint(enemyObject.CurrentNoise.transform.position);
        Vector2 direction = (Vector2)enemyObject.CurrentNoise.transform.position - enemyObject.Rb.position;

        if (direction.magnitude < 0.2f)
        {
            if (CanExit)
            {
                enemyObject.CLearCurrentNoise();
                enemyObject.ChangeState(new EnemySearch(enemyObject));
                return;
            }
        }

        Vector2 targetPos = Vector2.MoveTowards(enemyObject.Rb.position, enemyObject.Rb.position + direction, enemyObject.MoveSpeed * Time.fixedDeltaTime);

        enemyObject.Rb.MovePosition(targetPos);

        if (enemyObject.Suspicion >= Enemy.LowSuspicion)
        {
            if (enemyObject.CanSeeTarget)
                enemyObject.ChangeState(new EnemyChase(enemyObject));
        }
    }
}
