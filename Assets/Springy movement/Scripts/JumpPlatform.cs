using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlatform : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 15f;
    float jumpUpForce;
    // Start is called before the first frame update
    void Start()
    {
        jumpUpForce = Mathf.Sqrt(jumpHeight * 2 * 49);
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerStateAgent>(out PlayerStateAgent agent)) return;
        agent.Jump(Vector3.up, jumpUpForce, 0f);
    }

    void OnDrawGizmos()
    {
        Vector3 jumpMaxPos = transform.position;
        jumpMaxPos.y = jumpHeight;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, jumpMaxPos);
        Gizmos.DrawSphere(jumpMaxPos, .1f);
    }
}
