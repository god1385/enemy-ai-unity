using UnityEngine;

public class EnemySearch : EnemyState
{
    private float timer;
    private float searchTime;
    private float stateEnterTime;
    bool CanExit => Time.time - stateEnterTime > 0.5f;
    public EnemySearch(Enemy enemy) : base(enemy)
    {
        searchTime = enemy.SearchTime;
    }

    public override void Enter()
    {
        base.Enter();
        timer = 0f;
        Debug.Log("State:Search");
        stateEnterTime = Time.time;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Execute()
    {
        timer += Time.deltaTime;
        enemyObject.InvestigateAround();
        TryFindNewTarget();

        if (timer > searchTime)
        {
            enemyObject.ChangeState(new EnemyReturn(enemyObject));
        }


    }

    private void TryFindNewTarget()
    {
        if (enemyObject.CurrentNoise != null || enemyObject.CanSeeTarget)
        {
            EnemyState enemyState = enemyObject.EvaluateThreat();

            if (enemyState != null)
            {
                if (CanExit)
                    enemyObject.ChangeState(enemyState);
            }
        }
    }
}
