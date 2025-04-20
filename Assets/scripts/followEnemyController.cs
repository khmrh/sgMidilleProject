using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using UnityEngine;

public class followEnemyController : MonoBehaviour
{
    public float moveSpeed = .5f;
    public float raycastDistance = .2f;
    public float traceDistance = 2f;

    private Transform player;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {

        Vector2 direction = player.position - transform.position;

        if (direction.magnitude > traceDistance)
            return;
            
        Vector2 directionNomalized = direction.normalized;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directionNomalized, raycastDistance);
        Debug.DrawRay(transform.position, directionNomalized * raycastDistance, UnityEngine.Color.red);
        foreach (RaycastHit2D rhit in hits)
        {
            if (rhit.collider != null && rhit.collider.CompareTag("Obstacle"))
            {
                Vector3 alternativeDirection = Quaternion.Euler(0f, 0f, -90f) * direction;
                transform.Translate(alternativeDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(direction *  moveSpeed * Time.deltaTime);
            }
        }
    }
}
