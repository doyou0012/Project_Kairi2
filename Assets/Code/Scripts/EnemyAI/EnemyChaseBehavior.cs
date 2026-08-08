using UnityEngine;

public enum ChaseResult
{
    Chasing,
    InAttackRange,
    TargetLost
}

public class EnemyChaseBehavior : MonoBehaviour
{
    private float chaseTime = 2f;
    private float chaseTimer;

    public void OnEnterChase()
    {
        chaseTimer = 0f;
        chaseTime = 2f;
    }

    /// <summary>
    /// Executes chase logic. Returns ChaseResult based on target distance and visibility.
    /// </summary>
    public ChaseResult UpdateChase(Transform target, EnemyMovement movement, EnemySight sight, EnemyDataManager dataManager)
    {
        chaseTimer += Time.deltaTime;

        bool playerVisible = sight != null && sight.IsPlayerInRange();
        if (playerVisible)
        {
            chaseTimer = 0f;
        }
        else if (chaseTimer >= chaseTime)
        {
            return ChaseResult.TargetLost;
        }

        if (target != null && dataManager != null && dataManager._enemyStats != null)
        {
            Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
            movement.Move(dir, dataManager._enemyStats.ChaseSpeed);

            float dist = Vector2.Distance(transform.position, target.position);
            if (dist <= dataManager._enemyStats.AttackRange)
            {
                movement.StopHorizontal();
                return ChaseResult.InAttackRange;
            }
        }

        return ChaseResult.Chasing;
    }
}
