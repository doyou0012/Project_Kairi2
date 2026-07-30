using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private ObjectFlip objectFlip;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        objectFlip = GetComponent<ObjectFlip>();
    }

    /// <summary>
    /// Moves the Rigidbody2D horizontally.
    /// </summary>
    public void Move(Vector2 direction, float speed)
    {
        rb.linearVelocity = new Vector2(direction.x * speed, rb.linearVelocity.y);
        UpdateFacing(direction.x);
    }

    /// <summary>
    /// Stops horizontal movement.
    /// </summary>
    public void StopHorizontal()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    /// <summary>
    /// Stops all movement.
    /// </summary>
    public void StopAll()
    {
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Flips the character using ObjectFlip if available, otherwise manually.
    /// </summary>
    public void Flip()
    {
        if (objectFlip != null)
        {
            objectFlip.Flip();
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// Updates facing direction based on movement direction.
    /// </summary>
    public void UpdateFacing(float directionX)
    {
        if (directionX != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Sign(directionX) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    /// <summary>
    /// Returns movement direction based on scale.
    /// </summary>
    public Vector2 GetFacingDirection()
    {
        return transform.localScale.x < 0f ? Vector2.right : Vector2.left;
    }
}
