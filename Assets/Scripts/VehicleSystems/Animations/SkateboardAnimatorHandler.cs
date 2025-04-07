using UnityEngine;

public class SkateboardAnimatorHandler : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the skateboard's velocity
        Vector3 velocity = rb.linearVelocity;

        // Calculate the speed of the skateboard
        float speed = velocity.magnitude;

        // Set the animator parameter based on the speed
        animator.SetFloat("Speed", speed);
    }
}
