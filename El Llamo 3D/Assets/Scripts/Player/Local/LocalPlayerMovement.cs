using UnityEngine;

public class LocalPlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravityMultiplier = 2f;
    public float jumpSpeed = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private Vector3 velocity;
    private bool isGrounded;

    private PlayerAnimations playerAnimations;

    private Vector2 movement;
    private bool isJumping = false;

    protected virtual void Start()
    {
        playerAnimations = GetComponent<PlayerAnimations>();
        movement = new Vector2();
    }

    protected virtual void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        Move(movement);

        playerAnimations?.WalkRunAnim(movement.magnitude);

        isJumping = Input.GetButtonDown("Jump");

        Jump(isJumping);

        // now we apply gravity
        velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void Move(Vector2 move)
    {
        Vector3 move3d = transform.right * move.x + transform.forward * move.y;

        controller.Move(move3d * speed * Time.deltaTime);
    }

    private void Jump(bool isJumping)
    {
        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpSpeed * Physics.gravity.y * -gravityMultiplier);
            SoundManager.instance.Play("Jump");
        }
    }
}
