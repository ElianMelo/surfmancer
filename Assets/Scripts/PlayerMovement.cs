using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    private Rigidbody playerRb;

    [Header("Spline Follower")]
    public SplineFollowerAttach splineFollowerAttach;
    public CapsuleCollider capsuleCollider;
    public GameObject splinePrefabUpLeft;
    public GameObject splinePrefabUpRight;
    public Transform splineSpawnPoint;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
        {
            playerRb.linearDamping = groundDrag;
        } else
        {
            playerRb.linearDamping = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            ExitSpline();
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            var objectInstance = Instantiate(splinePrefabUpLeft, splineSpawnPoint.position, Camera.main.transform.rotation);
            objectInstance.transform.Rotate(0f, 90f, 0f);
            //Destroy(objectInstance, 7f);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            var objectInstance = Instantiate(splinePrefabUpRight, splineSpawnPoint.position, Camera.main.transform.rotation);
            objectInstance.transform.Rotate(0f, 90f, 0f);
            //Destroy(objectInstance, 7f);
        }
    }

    public void ExitSpline()
    {
        //if (!splineFollowerAttach) return;
        splineFollowerAttach.ExitSpline(true);
        playerRb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
        {
            playerRb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }    
        // in air
        else if(!grounded)
        {
            playerRb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
            
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            playerRb.linearVelocity = new Vector3(limitedVel.x, playerRb.linearVelocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
        playerRb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
