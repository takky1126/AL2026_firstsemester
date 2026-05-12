using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private Rigidbody2D m_rigidbody;//Rigidbody2Dへのショートカット
    //private float m_fAxisHorizontal;//左右方向の入力を受け取る
    //private float m_fAxisVertical;  //上下方向の入力を受け取る
    private float m_fSpeed = 10f;    //キャラクターのスピード
    private Vector2 moveInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    m_fAxisHorizontal = Input.GetAxisRaw("Horizontal"); //左右
    //    m_fAxisVertical = Input.GetAxisRaw("Vertical");     //上下
    //}

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    private void FixedUpdate()
    {
        //m_rigidbody.linearVelocity = new Vector2(
        //    m_fAxisHorizontal * m_fSpeed,   //左右移動に移動速度を与える
        //    m_fAxisVertical * m_fSpeed      //上下移動に移動速度を与える
        //);

        m_rigidbody.linearVelocity = moveInput * m_fSpeed;
    }
}
