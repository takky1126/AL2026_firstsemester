using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * speed;

        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false; // 右を向く
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;  // 左を向く
        }
        else if (moveInput.y != 0)
        {
            // 【追加】横の入力がなく、縦（上下）に動いている時は反転を戻す（下向きに戻る）
            spriteRenderer.flipX = false;
        }
    }
}