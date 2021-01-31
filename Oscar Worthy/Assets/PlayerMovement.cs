using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Camera playerCam;
    GroundCollider groundCollider;

    public Vector3 Velocity = new Vector3();
    enum JumpState {grounded, rising, hovering, falling, freeFalling}

    JumpState jumpState = JumpState.falling;

    public bool grounded = false;

    Vector3 fallReturnPosition;     // The position to return the player to if they fall off the edge
    float returnHeight = -20.0f;    // The height at which the player is teleported back onto the level after falling off
    
    float speed = 0.02f;
    float jumpSpeed = 0.04f;
    float gravity = -0.0002f;
    float hoverGravity = -0.00001f;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        groundCollider = transform.Find("GroundCollider").gameObject.GetComponent<GroundCollider>();

        groundCollider.OnGroundEnter += OnGroundEnter;
        groundCollider.OnGroundExit += OnGroundExit;

        fallReturnPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        HandleJumpAndGravity();

        HandleHorizontalMovement();

        ApplyMovement();

        CatchFallenPlayer();
    }

    void ApplyMovement()
    {
        transform.position += Velocity;
    }

#region horizontal movement

    void HandleHorizontalMovement()
    {
        Vector3 movement = new Vector3();

        movement = GetHorizontalMovementInput();

        Vector3 worldMovement = ConvertHorizontalCameraToWorld(movement);

        Vector3 slopeMovement = HandleSlopes(worldMovement);

        if (jumpState == JumpState.grounded)
        {
            Velocity = slopeMovement;
        }
        else
        {
            Velocity = new Vector3(worldMovement.x, Velocity.y, worldMovement.z);
        }

    }

    Vector3 GetHorizontalMovementInput()
    {
        Vector3 movement = new Vector3();

        movement.x = Input.GetAxisRaw("KeyboardAD");
        movement.z = Input.GetAxisRaw("KeyboardWS");

        movement = movement.normalized * speed;

        return movement;
    }

    Vector3 ConvertHorizontalCameraToWorld(Vector3 cameraMovement)
    {
        Vector3 cameraSpace = playerCam.transform.localEulerAngles;

        Vector3 worldMovement = Quaternion.Euler(0, cameraSpace.y, 0) * cameraMovement;

        return worldMovement;
    }

    Vector3 HandleSlopes(Vector3 movement)
    {
        if (jumpState == JumpState.grounded)
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;

            int layerMask = 1 << 6;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) < 45)
                {
                    Vector3 groundNormal = hit.normal;
                    movement = Vector3.ProjectOnPlane(movement, groundNormal);
                }
            }
        }
        return movement;
    }

#endregion

    void HandleJumpAndGravity()
    {
        if (Input.GetButtonDown("Jump") && jumpState == JumpState.grounded)
        {
            Velocity.y = jumpSpeed;
            jumpState = JumpState.rising;
        }

        // Transition to hovering if we're rising and just started falling
        if ((jumpState == JumpState.rising || jumpState == JumpState.falling) && Velocity.y < 0 && Input.GetButton("Jump"))
        {
            jumpState = JumpState.hovering;
            Velocity.y = 0;
        }

        // Transition to falling if we're hovering and we let go of the spacebar
        if (jumpState == JumpState.hovering && !Input.GetButton("Jump"))
        {
            jumpState = JumpState.freeFalling;
        }

        // Apply gravity based on our current state
        if (jumpState == JumpState.hovering)
        {
            Velocity.y += hoverGravity;
        }
        else if(jumpState == JumpState.freeFalling || jumpState == JumpState.falling || jumpState == JumpState.rising)
        {
            Velocity.y += gravity;
        }
        else
        {
            Velocity.y = 0;
        }
    }

    // Returns the player to the level if they fall off
    bool CatchFallenPlayer()
    {
        if(transform.position.y < returnHeight)
        {
            transform.position = fallReturnPosition;
            Velocity = new Vector3(0.0f, 0.0f, 0.0f);
            return true;
        } else
        {
            //if(jumpState == JumpState.grounded)
            //{
            //    fallReturnPosition = transform.position;
            //}
            return false;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag == "ground")
    //    {
    //        jumpState = JumpState.grounded;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.tag == "ground")
    //    {
    //        jumpState = JumpState.falling;
    //    }
    //}

    private GameObject currentGround;
    private void OnGroundEnter(object _, Collider ground)
    {
        currentGround = ground.gameObject;
        jumpState = JumpState.grounded;
    }

    private void OnGroundExit(object _, Collider ground)
    {
        if (currentGround == ground.gameObject)
        {
            currentGround = null;
            jumpState = JumpState.falling;
        }
    }
}
