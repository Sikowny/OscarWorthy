using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Camera playerCam;

    public Vector3 Velocity = new Vector3();

    enum JumpState {grounded, rising, hovering, falling}

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

        fallReturnPosition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHorizontalMovement();

        HandleJumpAndGravity();

        ApplyMovement();

        CatchFallenPlayer();
    }

    void ApplyMovement()
    {
        transform.position += Velocity;
    }

    void HandleHorizontalMovement()
    {
        Vector3 movement = new Vector3();

        movement = GetHorizontalMovementInput();

        Vector3 worldMovement = ConvertHorizontalCameraToWorld(movement);

        Velocity = new Vector3(worldMovement.x, Velocity.y, worldMovement.z);
    }

    Vector3 GetHorizontalMovementInput()
    {
        Vector3 movement = new Vector3();

        movement.x = Input.GetAxis("KeyboardAD");
        movement.z = Input.GetAxis("KeyboardWS");

        movement = movement.normalized * speed;

        return movement;
    }

    Vector3 ConvertHorizontalCameraToWorld(Vector3 cameraMovement)
    {
        Vector3 cameraSpace = playerCam.transform.localEulerAngles;

        Vector3 worldMovement = Quaternion.Euler(0, cameraSpace.y, 0) * cameraMovement;

        return worldMovement;
    }

    void HandleJumpAndGravity()
    {
        if (Input.GetButtonDown("Jump") && grounded)
        {
            Velocity.y = jumpSpeed;   
            grounded = false;
        }

        if (!grounded)
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
            if(grounded)
            {
                fallReturnPosition = transform.position;
            }
            return false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            grounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            grounded = false;
        }
    }
}
