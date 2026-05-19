using UnityEngine;
using UnityEngine.InputSystem; // これが必要！

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    Rigidbody2D rb;
    Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // 新しい入力システムから呼ばれる関数
    public void OnMove(InputValue value)
    {
        // 入力値を Vector2 (XとYの向き) として受け取る
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // 物理移動の処理は同じ
        rb.linearVelocity = moveInput * speed;
    }
}