using UnityEngine;
using Globals;

public class EnemyLongRangeAttack : MonoBehaviour
{
    private float shootTime = 1.0f;
    private float shootTimer;

    public void OnEnterAttack()
    {
        shootTimer = 0f;
    }

    /// <summary>
    /// Updates attack execution. Returns true when attack animation finishes.
    /// </summary>
    public bool UpdateAttack(EnemyMovement movement, Animator anim)
    {
        if (movement != null)
        {
            movement.StopHorizontal();
        }

        shootTimer += Time.deltaTime;
        if (shootTimer >= shootTime)
        {
            FirePoolBullet();
            shootTimer = 0f;
        }

        if (anim != null)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (!stateInfo.IsName(EnemyAnimName.attack))
            {
                anim.Play(EnemyAnimName.recharge);
                return true; // Attack cycle finished
            }
        }

        return false;
    }

    /// <summary>
    /// Spawns a bullet from the pool manager at the enemy's position and rotation.
    /// </summary>
    public void FirePoolBullet()
    {
        if (GameManager.Instance != null && GameManager.Instance.poolManager != null)
        {
            GameManager.Instance.poolManager.SpawnFromPool(
                PrefabName.bullet,
                transform.position,
                transform.rotation
            );
        }
    }
}
