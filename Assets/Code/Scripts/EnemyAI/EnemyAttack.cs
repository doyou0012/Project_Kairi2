using UnityEngine;
using Globals;

public class EnemyAttack : MonoBehaviour
{
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
