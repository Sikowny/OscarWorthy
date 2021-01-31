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

    float attackRange = 2.25f;
    float attackWindUp = 2.0f;      // time the enemy must stand still before attacking
    float attackWindDown = 1.0f;    // time the enemy stands still after an attack
    float attackTimer;
    AttackState currAttackState = AttackState.OutOfRange;

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
        } else
        {
            HandleAttack();
        }

        ApplyMovement();
    }

    void HandleHorizontalMovement()
    {
        Vector3 vectorToPlayer = player.transform.position - this.transform.position;
        if (vectorToPlayer.magnitude <= attackRange)
        {
            currAttackState = AttackState.WindingUp;
            attackTimer = attackWindUp;
            velocity = new Vector3(0, 0, 0);
        }
        else {
            Vector3 acceleration = vectorToPlayer.normalized * accelerationRate * Time.deltaTime;
            acceleration.y = 0;

            velocity += acceleration;
            if (velocity.magnitude > speed)
            {
                velocity = velocity.normalized * speed;
            }
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
        if (currAttackState == AttackState.WindingUp) {
            attackTimer -= Time.deltaTime;
            if(attackTimer <= 0)
            {
                currAttackState = AttackState.Attacking;
            }
        } else if(currAttackState == AttackState.Attacking)
        {
            Vector3 attackDirection = (player.transform.position - this.transform.position).normalized;
            Instantiate(attackHitboxPrefab, transform.position + attackDirection * attackRange * 0.5f, transform.rotation);

            attackTimer = attackWindDown;
            currAttackState = AttackState.WindingDown;
        } else if(currAttackState == AttackState.WindingDown)
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
