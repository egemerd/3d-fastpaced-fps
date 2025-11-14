using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] private Transform target;

    Rigidbody rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        var delta = target.position - transform.position;
        delta.y = 0; 
        Vector3 direction = delta.normalized;

        //Debug.Log(direction);
        transform.LookAt(target);
        rb.linearVelocity = direction * Time.deltaTime * moveSpeed;
    }
}
