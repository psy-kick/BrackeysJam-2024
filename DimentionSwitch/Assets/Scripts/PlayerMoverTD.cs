using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMoverTD : MonoBehaviour
{
    public Rigidbody2D rb;
    Vector2 Movement;
    Vector2 Movedir;
    Vector2 LastMovedir;
    Vector2 Rolldir;
    public float MoveSpeed = 5f;
    public Animator anim;
    public float RollSpeed;
    bool isAttacking;
    private enum State
    {
        Normal,
        Rolling,
        Attack,
    }
    private State state;
    private void Awake()
    {
        state = State.Normal;
    }
    // Start is called before the first frame update
    void Start()
    {
        state = State.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                Movement.x = Input.GetAxisRaw("Horizontal");
                Movement.y = Input.GetAxisRaw("Vertical");
                if ((Movement.x == 0 && Movement.y == 0) && Movedir.x != 0 || Movedir.y != 0)
                {
                    LastMovedir = Movedir;
                }
                Movedir = new Vector2(Movement.x, Movement.y).normalized;
                if (Input.GetKey(KeyCode.Space))
                {
                    Rolldir = Movedir;
                    RollSpeed = 20f;
                    state = State.Rolling;
                }
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    isAttacking = true;
                    state = State.Attack;
                }
                break;
            case State.Rolling:
                float RollSpeedDropMul = 5f;
                RollSpeed -= RollSpeed * RollSpeedDropMul * Time.deltaTime;
                float RollSpeedMin = 5f;
                if (RollSpeed < RollSpeedMin)
                {
                    state = State.Normal;
                }
                break;
            case State.Attack:
                Attack();
                if (isAttacking == false)
                {
                    StartCoroutine(AttackDelay());
                }
                break;
        }
        UpdateAnimations();
    }
    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(0.01f);
        anim.SetBool("Attack", false);
        state = State.Normal;
    }
    private void Attack()
    {
        isAttacking = false;
        anim.SetBool("Attack", true);
    }

    private void FixedUpdate()
    {
        switch(state)
        {
            case State.Normal:
                rb.MovePosition(rb.position + Movement * MoveSpeed * Time.fixedDeltaTime);
                break;
            case State.Rolling:
                DodgeRoll();
                break;
        }
    }

    private void DodgeRoll()
    {
        rb.velocity = Rolldir * RollSpeed;
    }

    private void UpdateAnimations()
    {
        anim.SetFloat("xdirection", Movedir.x);
        anim.SetFloat("ydirection", Movedir.y);
        anim.SetFloat("xLastMoveDir", LastMovedir.x);
        anim.SetFloat("yLastMoveDir", LastMovedir.y);
        anim.SetFloat("RunMagnitude", Movedir.magnitude);
        anim.SetFloat("RollMagnitude",RollSpeed);
    }
}
