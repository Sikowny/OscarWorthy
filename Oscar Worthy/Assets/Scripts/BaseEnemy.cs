using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    enum AttackState { OutOfRange, WindingUp, Attacking, WindingDown };

    GameObject player;
    bool active;

    Vector3 velocity = new Vector3();
    bool grounded = false;

    float speed = 0.01f;
    float accelerationRate = 0.03f;
    float jumpSpeed = 0.01f;
    float gravity = -0.0001f;

    float playerDetectionRange = 15.0f; // How close the player must be for the enemy to pursue them

    float attackRange = 2.25f;
    float attackWindUp = 1.0f;      // time the enemy must stand still before attacking
    float attackWindDown = 0.5f;    // time the enemy stands still after an attack
    float attackTimer;
    AttackState currAttackState = AttackState.OutOfRange;
    Vector3 attackDirection;

    public GameObject attackHitboxPrefab;

    Vector3 prevPosition;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        active = false;
        attackTimer = attackWindUp;
    }

    // Update is called once per frame
    void Update()
    {
        if (currAttackState == AttackState.OutOfRange)
        {
            HandleHorizontalMovement();
        }
        else
        {
            HandleAttack();
        }

        ApplyMovement();
    }

    void HandleHorizontalMovement()
    {
        Vector3 vectorToPlayer = player.transform.position - this.transform.position;

        if (vectorToPlayer.magnitude <= playerDetectionRange)
        {
            // Rotate to look at the player
            // To ensure the enemy remains upright, the enemy's own y-position is used when determining where to look
            Vector3 lookTarget = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
            transform.LookAt(lookTarget);
        }

        // check if the enemy is over solid ground
        RaycastHit hit;
        Vector3 rayOffset = transform.position + transform.TransformDirection(Vector3.forward) * 0.2f + new Vector3(0, 1, 0);
        if (Physics.Raycast(rayOffset, transform.TransformDirection(Vector3.down), out hit))
        {
            // solid ground - can move
            // Note that the enemy will keep moving as long as it detects solid ground below it, even if it's lower, so it can go off ledges
            // Gravity isn't implemented yet
            if (vectorToPlayer.magnitude <= attackRange)
            {
                currAttackState = AttackState.WindingUp;
                attackTimer = attackWindUp;
                attackDirection = (player.transform.position - this.transform.position).normalized;

                velocity = new Vector3(0, 0, 0);
            }
            else if (vectorToPlayer.magnitude <= playerDetectionRange)
            {
                Vector3 acceleration = vectorToPlayer.normalized * accelerationRate * Time.deltaTime;
                acceleration.y = 0;

                velocity += acceleration;
                if (velocity.magnitude > speed)
                {
                    velocity = velocity.normalized * speed;
                }
            }
            else
            {
                // If the player is out of range, the enemy decelerates and comes to a rest
                if (velocity.magnitude > 0.001f)
                {
                    velocity -= velocity * 2.0f * Time.deltaTime;
                }
                else
                {
                    velocity = new Vector3(0, 0, 0);
                }
            }
        }
        else
        {
            // no solid ground; stop moving
            velocity = new Vector3(0, 0, 0);
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

    void HandleAttack()
    {
        if (currAttackState == AttackState.WindingUp)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                currAttackState = AttackState.Attacking;
            }
        }
        else if (currAttackState == AttackState.Attacking)
        {
            Vector3 attackHitboxPos = transform.position + (attackDirection * attackRange * 0.5f) + new Vector3(0, 1.25f, 0);
            Instantiate(attackHitboxPrefab, attackHitboxPos, transform.rotation);

            attackTimer = attackWindDown;
            currAttackState = AttackState.WindingDown;
        }
        else if (currAttackState == AttackState.WindingDown)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                currAttackState = AttackState.OutOfRange;
            }
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
