using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
  public float horizontal;
  public bool isJumpDown, isJumpUp, isJumping;

  public void Zero()
  {
    horizontal = 0.0f;
    isJumpDown = false;
    isJumpUp = false;
    isJumping = false;
  }
}

public class PlayerController : MonoBehaviour
{
  public NarrativeController nc;

  // Input variables
  public PlayerInput pi;
  public bool isActive;

  [Header("Movement variables")]
  public float moveSpeed;
  public LayerMask wallLayer;
  //public float gravity;

  [Header("Jumping variables")]
  public bool isGrounded;
  public LayerMask groundLayer;
  public float jumpForce;
  public float secJumpForce;
  public int secondJumpImpusle; 
  private int jumpImpusles;
  private bool isReleasedJump;

  [Header("Effects")]
  public ShakeEffect shakeEffect;

  private Rigidbody2D rb;
  private Animator anim;
  private Vector3 origin;

  [Header("Flags")]
  public bool isRuleBroken;

  public bool IsRuleBroken { get {return isRuleBroken;}}
  public bool Active {
    get {return isActive;} 
    set
    {
      rb.simulated = value;
      isActive = value;
    }}

  // Start is called before the first frame update
  void Awake()
  {
    origin = transform.position;
    pi = new PlayerInput();
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponentInParent<Animator>();
    shakeEffect = new ShakeEffect();
  }

  private void Start() {
    shakeEffect.Init(transform);
    ResetPC();
  }

  void UpdateInput() {
    if (!isActive) return;
    pi.horizontal = Input.GetAxis("Horizontal");
    pi.isJumpDown = Input.GetKeyDown(KeyCode.Space);
    pi.isJumpUp = Input.GetKeyUp(KeyCode.Space);
    pi.isJumping = Input.GetKey(KeyCode.Space);
  }

  // Update is called once per frame
  void Update()
  {
    shakeEffect.Update();
    UpdateInput();

    Vector3 velocity = new Vector3(pi.horizontal, 0, 0) * moveSpeed * Time.deltaTime;

    // horizontal movement
    RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity, velocity.magnitude + transform.localScale.x / 2.0f, wallLayer);
    if (hit.collider == null)    
    {
      transform.Translate(velocity, Space.World);
    }
    //rb.AddForce(velocity, ForceMode2D.Impulse);

    // check if grounded
    isGrounded = rb.IsTouchingLayers(groundLayer);

    if (isGrounded)
    {
      jumpImpusles = 0;
      if (pi.isJumpDown) 
      {
        isReleasedJump = false;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        Toolbox.Instance.SfxMan.PlaySoundWithPitch("Jump", 0.9f, 1.1f, 0.05f);
      }
    }
    else
    {
      // check if the player releases the jump button
      if (pi.isJumpUp) 
      {
        isReleasedJump = true;
      }

      if (isReleasedJump == false && pi.isJumping) 
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

    // clamp velocity
    //rb.velocity = Vector2.ClampMagnitude(rb.velocity, 5.0f);
  }

  public void ResetPC()
  {
    pi.Zero();
    rb.velocity = Vector2.zero;
    isRuleBroken = false;
    transform.position = origin;
    shakeEffect.TriggerShake(0.25f);
    if (nc?.narrativeLevel <= 1) Active = false;
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.tag == "RuleTrigger")
    {
      isRuleBroken = true;
    }
  }
}
