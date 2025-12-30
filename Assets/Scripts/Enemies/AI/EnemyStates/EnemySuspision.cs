using UnityEngine;

public class EnemySuspision : EnemyState
{
    public EnemySuspision(Enemy enemy) : base(enemy) { }

    private float lookTimer;


    public override void Enter()
    {
        lookTimer = 0f;
        base.Enter();
        Debug.Log("State:Susp");
    }

    public override void Exit()
    {
        lookTimer = 0f;
        base.Exit();
    }

    public override void Execute()
    {
        lookTimer += Time.fixedDeltaTime;
        enemyObject.InvestigateAround();

        if (enemyObject.CurrentNoise != null || enemyObject.CanSeeTarget)
        {
            if (enemyObject.Suspicion >= Enemy.MiddleSuspicion)
            {
                EnemyState enemyState = enemyObject.EvaluateThreat();

                if (enemyState != null)
                    enemyObject.ChangeState(enemyState);

                return;
            }
        }

        if (lookTimer > enemyObject.SearchTime)
        {
            enemyObject.ChangeState(new EnemyReturn(enemyObject));
        }

    }
}
