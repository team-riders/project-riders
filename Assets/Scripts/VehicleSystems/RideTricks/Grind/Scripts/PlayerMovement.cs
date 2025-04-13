using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerGrind))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 6f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private PlayerGrind grind;
    private Vector3 velocity;
    private bool isGrounded;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        grind = GetComponent<PlayerGrind>();
    }

    private void Update()
    {
        if (grind.IsGrinding()) return;

        isGrounded = controller.isGrounded;

        // Reset vertical velocity when grounded
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        // Horizontal movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = jumpForce;

            // Optional: Try grinding immediately after jump
            grind.TryAutoGrind(grind.autoGrindRadius);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
