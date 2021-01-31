using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Camera playerCam;

    public Vector3 Velocity = new Vector3();

    enum JumpState {grounded, rising, hovering, falling}

    public bool grounded = false;

    float speed = 0.02f;
    float jumpSpeed = 0.04f;
    float gravity = -0.0002f;
    float hoverGravity = -0.00001f;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleHorizontalMovement();

        HandleJumpAndGravity();

        ApplyMovement();
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
