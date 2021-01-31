using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    GameObject player;
    bool active;

    Vector3 velocity = new Vector3();
    bool grounded = false;

    float speed = 0.01f;
    float accelerationRate = 0.03f;
    float jumpSpeed = 0.01f;
    float gravity = -0.0001f;

    Vector3 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        active = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleHorizontalMovement();

        ApplyMovement();
    }

    void HandleHorizontalMovement()
    {
        Vector3 acceleration = (player.transform.position - this.transform.position).normalized * accelerationRate * Time.deltaTime;
        acceleration.y = 0;

        velocity += acceleration;
        if(velocity.magnitude > speed)
        {
            velocity = velocity.normalized * speed;
        }
    }

    void HandleJumpAndGravity()
    {
        if (!grounded)
        {
            velocity.y += gravity;
        }
        else
        {
            velocity.y = 0;
        }
    }

    void ApplyMovement()
    {
        prevPosition = this.transform.position;
        this.transform.position += velocity;
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
