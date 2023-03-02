using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;


public class CharacterMovement : MonoBehaviour
{
    public Animator animator;
    private PolyNavAgent _agent;
    private bool isMoving;

    private void Start()
    {
        _agent = GetComponent<PolyNavAgent>();
        animator = GetComponent<Animator>();
        _agent.maxSpeed = 0.5f;
        _agent.maxForce = 1;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _agent.SetDestination(mousePosition);
            //isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (_agent.hasPath && _agent.remainingDistance > _agent.stoppingDistance)
        {
            Vector2 moveDirection = (_agent.nextPoint - (Vector2)transform.position).normalized;
            transform.up = moveDirection;
            transform.position = Vector2.MoveTowards(transform.position, _agent.nextPoint, _agent.maxSpeed * Time.deltaTime);
            isMoving = true;

            // keep the character upright
            transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            if (isMoving)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}


