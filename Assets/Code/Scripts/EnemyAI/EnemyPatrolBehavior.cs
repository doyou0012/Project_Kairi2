using UnityEngine;

public class EnemyPatrolBehavior : MonoBehaviour
{
    private float patrolTime;
    private float patrolTimer;
    private bool isPatrolling;

    public void OnEnterPatrol()
    {
        isPatrolling = false;
        patrolTimer = 0f;
        patrolTime = Random.Range(2f, 3f);
    }

    /// <summary>
    /// Executes patrol logic. Returns true when patrol duration is completed.
    /// </summary>
    public bool UpdatePatrol(EnemyMovement movement, EnemyDataManager dataManager)
    {
        if (!isPatrolling)
        {
            movement.Flip();
            isPatrolling = true;
        }

        Vector2 dir = movement.GetFacingDirection();
        if (dataManager != null && dataManager._enemyStats != null)
        {
            movement.Move(dir, dataManager._enemyStats.PatrolSpeed);
        }

        patrolTimer += Time.deltaTime;
        if (patrolTimer >= patrolTime)
        {
            movement.StopHorizontal();
            return true; // Patrol completed
        }

        return false;
    }
}
