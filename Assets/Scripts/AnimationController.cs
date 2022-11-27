using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float dampTime = .1f;
    
    Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = rigidbody.velocity;
        velocity.y = 0;

        float speed = velocity.magnitude;
        animator.SetFloat("Speed", speed, dampTime, Time.deltaTime);
    } 
}
