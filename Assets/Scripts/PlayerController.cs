using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [Header("Movement variables")]
  public float moveSpeed;


  [Header("Jumping variables")]
  public bool isGrounded;
  public LayerMask groundLayer;
  public float jumpForce;
  public float secJumpForce;
  public int secondJumpImpusle; 
  private int jumpImpusles;
  private bool isReleasedJump;

  private Rigidbody2D rb;
  private Animator anim;

  // Input variables
  private float horizontal;
  private bool isJumpDown, isJumpUp, isJumping;

  // Start is called before the first frame update
  void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponentInParent<Animator>();
  }

  void UpdateInput() {
    horizontal = Input.GetAxis("Horizontal");
    isJumpDown = Input.GetKeyDown(KeyCode.Space);
    isJumpUp = Input.GetKeyUp(KeyCode.Space);
    isJumping = Input.GetKey(KeyCode.Space);
  }

  // Update is called once per frame
  void Update()
  {
    UpdateInput();

    // horizontal movement
    transform.Translate(new Vector3(horizontal, 0, 0) * moveSpeed * Time.deltaTime, Space.World);

    // check if grounded
    isGrounded = rb.IsTouchingLayers(groundLayer);

    if (isGrounded)
    {
      jumpImpusles = 0;
      if (isJumpDown) 
      {
        isReleasedJump = false;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
      }
    }
    else
    {
      // check if the player releases the jump button
      if (isJumpUp) 
      {
        isReleasedJump = true;
      }

      if (isReleasedJump == false && isJumping) 
      {
        ++jumpImpusles;
        if (jumpImpusles == secondJumpImpusle) 
        {
          //LeanTween.rotateAroundLocal(gameObject, Vector3.forward, 360.0f, 1.0f);
          anim.SetTrigger("backflip");
          rb.AddForce(Vector2.up * secJumpForce, ForceMode2D.Impulse);
        }
      }      
    }
  }
}
