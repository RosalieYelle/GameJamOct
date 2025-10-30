using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public bool canMove = true;
    public InputAction MoveAction;

    private Rigidbody2D rb;
    private Animator animator;
    //To flip the sprite
    private SpriteRenderer spriteRenderer;
    public float moveSpeed = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        MoveAction.Enable();
    }

    void FixedUpdate()
    {
        // Makes the player unabe to move in dialogue
        if (!canMove)
        {
            animator.SetFloat("MoveX", 0);
            return;
        }

        Vector2 move = MoveAction.ReadValue<Vector2>();
        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
        animator.SetFloat("MoveX", move.x);

        //Flip the animation
        if (move.x > 0.1f)
        {
           spriteRenderer.flipX = true; //Sprite facing right
        }else if(move.x < -0.1f){
           spriteRenderer.flipX = false; //Sprite facing left
        }
    }
}
