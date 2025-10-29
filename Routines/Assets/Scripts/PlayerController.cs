using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerController : MonoBehaviour
{
    public Sprite move_sprite;
    public Sprite stand_sprite;
    public float speed = 5f;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float moveX;
    private float moveY;
    private bool moving = false;

    public bool canMove = true;


    void Start()
    {
        speed = 4f;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // MoveAction.Enable(); 
    }

    void Update()
    {

        // Makes the player unabe to move in dialogue
        if (!canMove)
        {
            return;
        }

        moveX = 0f; //No movement
        moveY = 0f; //No movement

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            moveX = -1f; // move left
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            moveX = 1f; // move right
        }

        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
        {
            moveY = 1f; // move left
        }
        else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
        {
            moveY = -1f; // move right
        }
    }

    void FixedUpdate()
    {
        Vector2 newPos = rb.position + new Vector2(moveX, moveY) * speed * Time.fixedDeltaTime;

        if (moveX < 0)
        {
            spriteRenderer.flipX = false;
        }

        if (moveX > 0)
        {
            spriteRenderer.flipX = true;
        }

        /*if ((!moving) && (moveX != 0 || moveY != 0))
        {
            spriteRenderer.sprite = move_sprite;
            moving = true;
        }

        if (moving && moveX == 0 && moveY == 0)
        {
            spriteRenderer.sprite = stand_sprite;
            moving = false;
        } */
        
        rb.MovePosition(newPos);
    }
}
