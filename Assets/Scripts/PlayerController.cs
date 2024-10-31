using System.Collections;
using System.Collections.Generic;
using TTSDK;
using UnityEngine;
//using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public int Boundary;
    public int ply_Boundary;

    BoxCollider2D bobyCollider;
    Rigidbody2D r_Player;
    Animator a_Player;
    public Joystick moveJoystick;

    public float JumpUpForce = 13f;
    public float JumpDownForce = 10f;

    void Start()
    {
        bobyCollider = GetComponent<BoxCollider2D>();
        r_Player = GetComponent<Rigidbody2D>();
        a_Player = GetComponent<Animator>();
    }

    void Update()
    {
        Boundary = Global.Boundary;
        ply_Boundary = TT.PlayerPrefs.GetInt("Boundary");
        Move();
    }

    private void FixedUpdate()
    {
        Jump();
    }

    void Move()
    {
        if (a_Player.GetCurrentAnimatorStateInfo(0).IsName("knockdown"))
        {
            return;
        }
        if (Global.IsOnFloor)
        {
            if (moveJoystick.Horizontal == 0)
            {
                //在地板上，摇杆没动
                a_Player.Play("idle");
            }
            else
            {
                //在地板上，摇杆动了
                a_Player.Play("Run");
            }
            //在地板上，只有水平移动
            r_Player.velocity = new(Global.PlayerRunSpeed * moveJoystick.Horizontal, 0);
        }
        else
        {
            //空中水平方向移动
            if (moveJoystick.Horizontal != 0)
            {
                r_Player.velocity = new(Global.PlayerRunSpeed * (moveJoystick.Horizontal / Mathf.Abs(moveJoystick.Horizontal)), r_Player.velocity.y);
            }
            else
            {
                r_Player.velocity = new(0, r_Player.velocity.y);
            }
        }
        
        if (moveJoystick.Horizontal != 0)
        {
            transform.localScale = new Vector3(0.5f* (moveJoystick.Horizontal / Mathf.Abs(moveJoystick.Horizontal)),0.5f,1f);//player 朝向
        }
    }

    void Jump()
    {
        if (Global.IsJumpStart)
        {
            Global.IsJumpStart = false;
            //按了跳就设置isTrigger为true
            bobyCollider.isTrigger = true;
            if (moveJoystick.Vertical < 0 && Mathf.Abs(moveJoystick.Vertical) > Mathf.Abs(moveJoystick.Horizontal) && Global.IsOnFloor)
            {
                JumpDown();//只有在地板上才向下跳，其余自由落体
            }
            else
            {
                JumpUp();
            }
        }

        if (r_Player.velocity.y< 0)
        {
            //下落为碰撞器
            Global.isJumpUp = false;
            if (!a_Player.GetCurrentAnimatorStateInfo(0).IsName("knockdown"))
            {
                a_Player.Play("jump fall");
            }
            
            if (!Global.isJumpDown)
            {
                //下跳过程为触发器，整体离开了地板再变成碰撞器
                bobyCollider.isTrigger = false;
            }
        }
        //上升为触发器
        if (r_Player.velocity.y>0)
        {
            bobyCollider.isTrigger = true;
        }
    }

    //IEnumerator GroundCapsulleColliderTimmerFuc()
    //{
    //    yield return new WaitForSeconds(0.3f);
    //    Global.isJumpDown = false;
    //}

    void JumpUp()
    {
        //上跳
        //Debug.Log("上跳");
        //Debug.Log("isTrigger = " + bobyCollider.isTrigger);
        Global.IsOnFloor = false;
        Global.isJumpUp = true;
        a_Player.Play("jump up");
        r_Player.velocity = Vector2.zero;
        r_Player.AddForce(Vector2.up * JumpUpForce, ForceMode2D.Impulse);
    }

    void JumpDown()
    {
        //下跳
        //Debug.Log("下跳");
        //Debug.Log("isTrigger = " + bobyCollider.isTrigger);
        Global.IsOnFloor = false;
        Global.isJumpDown = true;
        a_Player.Play("jump fall");
        r_Player.AddForce(-Vector2.up * JumpDownForce);
        //StartCoroutine(GroundCapsulleColliderTimmerFuc());
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        bobyCollider.isTrigger = true;
    //    }
    //}
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            bobyCollider.isTrigger = true;
            Global.isJumpDown = true;//防止下跳变成碰撞器
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("floor"))
        {
            bobyCollider.isTrigger = false;
            Global.isJumpDown = false;//下跳结束
        }
    }
}
