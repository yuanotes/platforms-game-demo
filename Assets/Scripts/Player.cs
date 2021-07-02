using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
  [SerializeField] float runSpeed = 5f;
  [SerializeField] float jumpHeight = 5f;
  [SerializeField] float climbSpeed = 5f;
  new Rigidbody2D rigidbody = null;
  CapsuleCollider2D feedCollider = null;
  BoxCollider2D feetCollider = null;
  Animator animator = null;
  private Vector2 previousMoveInput;
  private float previousJumpInput;
  private float initialGravity = 0;

  void Start()
  {
    Controls controls = new Controls();
    controls.Player.Run.canceled += onMove;
    controls.Player.Run.performed += onMove;
    controls.Player.Jump.canceled += onJump;
    controls.Player.Jump.performed += onJump;
    controls.Enable();

    rigidbody = GetComponent<Rigidbody2D>();
    feedCollider = GetComponent<CapsuleCollider2D>();
    feetCollider = GetComponent<BoxCollider2D>();
    animator = GetComponent<Animator>();

    initialGravity = rigidbody.gravityScale;
  }
  // Update is called once per frame
  void Update()
  {
    run();
    jump();
    climb();
    flipSrite();
  }
  private void run()
  {
    Vector2 velocity = rigidbody.velocity;
    velocity.x = previousMoveInput.x * runSpeed;
    rigidbody.velocity = velocity;

    if (Mathf.Abs(rigidbody.velocity.x) > Mathf.Epsilon)
    {
      animator.SetBool("Running", true);
    }
    else
    {
      animator.SetBool("Running", false);
    }

  }
  private void jump()
  {
    if (
      feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) ||
      feedCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))
    )
    {
      animator.SetBool("Jumping", false);
      if (previousJumpInput > 0)
      {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpHeight);
      }
      return;
    }
    animator.SetBool("Jumping", true);
  }
  private void climb() {
    if (!feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))) {
      animator.SetBool("Climbing", false);
      rigidbody.gravityScale = initialGravity;
      return;
    }
    animator.SetBool("Climbing", Mathf.Abs(previousMoveInput.y) > Mathf.Epsilon);
    // jumping
    if (previousJumpInput > 0) {
      return;
    }
    rigidbody.gravityScale = 0f;
    rigidbody.velocity = new Vector2(rigidbody.velocity.x, previousMoveInput.y * climbSpeed);
  }
  private void flipSrite()
  {
    bool playerIsMoving = Mathf.Abs(rigidbody.velocity.x) > Mathf.Epsilon;
    if (playerIsMoving)
    {
      transform.localScale = new Vector2(Mathf.Sign(rigidbody.velocity.x), 1);
    }
  }
  private void onJump(InputAction.CallbackContext ctx)
  {
    previousJumpInput = ctx.ReadValue<float>();
  }
  private void onMove(InputAction.CallbackContext ctx)
  {
    previousMoveInput = ctx.ReadValue<Vector2>();
  }
}
