using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // public InputAction MoveAction;
    public float speed = 10f;
    private Rigidbody2D rb;
    private float moveX;
    private float moveY;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // MoveAction.Enable(); 
    }

    void Update()
    {
        // Vector2 move = MoveAction.ReadValue<Vector2>();
        // Debug.Log(move);
        // Vector2 position = (Vector2)transform.position + move * 0.1f;
        // transform.position = position;
        
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
        rb.MovePosition(newPos);
    }
}
