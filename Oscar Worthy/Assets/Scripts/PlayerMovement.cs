using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Camera playerCam;
    GroundCollider groundCollider;

    public Vector3 Velocity = new Vector3();
    public enum JumpState {grounded, rising, hovering, falling, freeFalling}
    public enum RunState {running, idle}

    public JumpState jumpState = JumpState.falling;
    public RunState runState = RunState.idle;

    public bool grounded = false;

    Vector3 fallReturnPosition;     // The position to return the player to if they fall off the edge
    float returnHeight = -20.0f;    // The height at which the player is teleported back onto the level after falling off
    
    float speed = 0.02f;
    float jumpSpeed = 0.04f;
    float gravity = -0.0002f;
    float hoverGravity = -0.00001f;

    // this can be moved to another script later
    int health = 5;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        groundCollider = transform.Find("GroundCollider").gameObject.GetComponent<GroundCollider>();

        groundCollider.OnGroundEnter += OnGroundEnter;
        groundCollider.OnGroundExit += OnGroundExit;

        fallReturnPosition = this.transform.position;

        //capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleJumpAndGravity();

        HandleHorizontalMovement();

        TurnTowardMovingDirection();

        ApplyMovement();

        CatchFallenPlayer();
    }

    void ApplyMovement()
    {
        //CheckForWalls();

        transform.position += Velocity;
    }

    //// Making the assumption that we're using a Capsule for our collider. Not good form, but not much time
    //CapsuleCollider capsuleCollider;
    //void CheckForWalls()
    //{
    //    RaycastHit hit;

    //    Vector3 p1 = transform.position - capsuleCollider.center + Vector3.up * capsuleCollider.radius;
    //    Vector3 p2 = transform.position + capsuleCollider.center - Vector3.up * capsuleCollider.radius;

    //    if (Physics.CapsuleCast(p1, p2, capsuleCollider.radius, Velocity, out hit)){
    //        if (hit.distance < Velocity.magnitude)
    //            Velocity = Velocity.normalized * hit.distance;
    //    }
    //}

    public float turnspeed = 1.5f;
    void TurnTowardMovingDirection()
    {
        Vector3 velocityNoY = new Vector3(Velocity.x, 0, Velocity.z);

        if (velocityNoY.magnitude > 0)
        {
            float angle = Vector3.SignedAngle(transform.forward, velocityNoY, Vector3.up);// Quaternion.Angle(transform.rotation, Quaternion.LookRotation(velocityNoY));

            transform.Rotate(0, Mathf.Clamp(angle, -turnspeed, turnspeed), 0);
        }

        //if (velocityNoY.magnitude > 0)
        //    transform.rotation = Quaternion.LookRotation(velocityNoY, Vector3.up);
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

        if (movement.magnitude > 0)
        {
            runState = RunState.running;
        }
        else
        {
            runState = RunState.idle;
        }

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

    float hoverTimer = 0;

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
            hoverTimer = 1.5f;
            Velocity.y = 0;
        }

        if (jumpState == JumpState.hovering)
        {
            hoverTimer -= Time.deltaTime;
            if (hoverTimer < 0) jumpState = JumpState.freeFalling;
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
            ReceiveDamage(1);
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

    private List<GameObject> currentGrounds = new List<GameObject>(); 
    private void OnGroundEnter(object _, Collider ground)
    {
        currentGrounds.Add(ground.gameObject);

        RefreshGroundState();
    }

    private void OnGroundExit(object _, Collider ground)
    {
        currentGrounds.Remove(ground.gameObject);

        RefreshGroundState();
    }

    private void RefreshGroundState()
    {
        if (currentGrounds.Count > 0)
        {
            jumpState = JumpState.grounded;
        }
        else
        {
            jumpState = JumpState.falling;
        }
    }
    
    public bool ReceiveDamage(int d)
    {
        Debug.Log("oof!");
        health -= d;
        if (health <= 0)
        {
            Debug.Log("You ded");
            return true;
        }
        return false;
    }
}
