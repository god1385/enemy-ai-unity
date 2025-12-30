using UnityEngine;

public abstract class EnemyState
{
    protected Enemy enemyObject;

    public EnemyState(Enemy enemy)
    {
        enemyObject = enemy;
    }

    public virtual void Enter() {}
    public virtual void Execute() { }
    public virtual void Exit() {}
}
