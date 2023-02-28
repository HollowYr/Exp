using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IkTest : MonoBehaviour
{
    [SerializeField] private float IKDistanseToGround = .056f;
    [SerializeField] private LayerMask layer;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

        Ray ray = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, IKDistanseToGround + 1f, layer, QueryTriggerInteraction.Ignore))
        {
            //if(hit.transform.tag = "walkable")
            {
                Vector3 footPosition = hit.point;
                footPosition.y += IKDistanseToGround;
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            }
        }

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

        ray = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, IKDistanseToGround + 1f))
        {
            //if(hit.transform.tag = "walkable")
            {
                Vector3 footPosition = hit.point;
                footPosition.y += IKDistanseToGround;
                animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            }
        }
    }
}
