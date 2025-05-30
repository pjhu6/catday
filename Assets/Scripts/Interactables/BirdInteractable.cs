using UnityEngine;
using System.Collections;

public class BirdInteractable : AbstractInteractable
{
    public SpriteAnimator spriteAnimator;
    public float jumpForce = 5f;
    public float moveSpeed = 2f;
    public Vector3 moveDirection = Vector3.back;
    public bool isRandomDirection = false;

    public float fallGravityMultiplier = 0.3f;
    public float riseGravityMultiplier = 1.5f;

    private Rigidbody rb;
    private DialogueObject dialogueObject;
    private int numHits = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // We apply custom gravity manually

        dialogueObject = GetComponent<DialogueObject>();
    }

    public override void Click()
    {
        spriteAnimator.Play();
        JumpAndMove();
        StartCoroutine(PlayAnimationWhileMoving());

        numHits++;
        if (numHits == 3)
        {
            Debug.Log("Hit more than 3 times - triggering dialogue");
            dialogueObject.TriggerDialogue();
        }
    }

    public override void Interact()
    {
        Debug.Log("Interacting with bird - not implemented");
    }

    private IEnumerator PlayAnimationWhileMoving()
    {
        yield return new WaitForSeconds(0.1f);

        // check if it's falling
        while (rb.linearVelocity.magnitude > 0.1f)
        {
            yield return null;
        }

        spriteAnimator.Stop();
    }

    void FixedUpdate()
    {
        // Custom gravity: fall slowly, rise quickly
        if (rb.linearVelocity.y < 0)
        {
            // Falling — weaker gravity
            rb.AddForce(Physics.gravity * fallGravityMultiplier, ForceMode.Acceleration);
        }
        else
        {
            // Rising — stronger gravity
            rb.AddForce(Physics.gravity * riseGravityMultiplier, ForceMode.Acceleration);
        }
    }

    void JumpAndMove()
    {
        // Reset velocity (Y only)
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // Apply jump force (vertical only)
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // Determine horizontal direction
        Vector3 horizontalDir = moveDirection;
        if (isRandomDirection)
        {
            horizontalDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        }

        // Ensure direction is horizontal
        horizontalDir = new Vector3(horizontalDir.x, 0f, horizontalDir.z).normalized;

        // Apply horizontal movement force
        rb.AddForce(horizontalDir * moveSpeed, ForceMode.VelocityChange);
    }
}
