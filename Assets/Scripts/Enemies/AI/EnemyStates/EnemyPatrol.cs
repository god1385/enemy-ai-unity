using UnityEngine;

public class EnemyPatrol : EnemyState
{
    public EnemyPatrol(Enemy enemy) : base(enemy) { }

    private Collider2D[] collderArray;
    private int patrolIndex = 0;
    private Vector2 currentPatrolPoint;

    public override void Enter()
    {
        base.Enter();
        currentPatrolPoint = enemyObject.PatrolPositionsList[patrolIndex].position;
    }

    public override void Exit()
    {
        base.Exit();
        patrolIndex = 0;
    }

    public override void Execute()
    {
        MoveToNextPoint();
        enemyObject.RotateToPoint(currentPatrolPoint);

        if (enemyObject.CurrentNoise != null || enemyObject.CanSeeTarget)
        {
            EnemyState enemyState = enemyObject.EvaluateThreat();

            if (enemyState != null)
                enemyObject.ChangeState(enemyState);
        }

        if (Vector2.Distance(enemyObject.transform.position, currentPatrolPoint) < 0.1f)
        {
            patrolIndex += 1;

            if (patrolIndex >= enemyObject.PatrolPositionsList.Count)
            {
                patrolIndex = 0;
                currentPatrolPoint = enemyObject.StartPosition;
            }
            else
                currentPatrolPoint = enemyObject.PatrolPositionsList[patrolIndex].position;
        }
    }

    private void MoveToNextPoint()
    {
        Vector2 direction = (currentPatrolPoint - enemyObject.Rb.position);
        Vector2 targetPosition = Vector2.MoveTowards(enemyObject.Rb.position, enemyObject.Rb.position + direction, enemyObject.MoveSpeed * Time.fixedDeltaTime);
        enemyObject.Rb.MovePosition(targetPosition);
    }
}
