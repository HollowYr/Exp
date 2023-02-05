using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MovementData", order = 1)]
public class MovementData : ScriptableObject
{
    [SerializeField] private Transform player;
    [SerializeField] internal readonly float gravity = 49f;
    [Foldout("Movement"), SerializeField] internal float jumpHeight = 3f;
    [Foldout("Movement"), SerializeField] internal float jumpUpForce = 10f;
    [Foldout("Movement"), SerializeField] internal float jumpSideForce = 30f;
    [Foldout("Movement"), SerializeField] internal ForceMode jumpForceMode = ForceMode.Impulse;
    [Foldout("Movement"), SerializeField] internal float movementSpeed = 10f;
    [Foldout("Movement"), SerializeField] internal float rotationSpeed = 10f;
    [Foldout("Movement"), SerializeField] internal float playerDesiredFloatHeight = 2f;

    [Foldout("Wallrun"), SerializeField] internal float distanceToDetectWall = .5f;
    [Foldout("Wallrun"), SerializeField] internal float distanceToWallOnRun = .5f;
    [Foldout("Wallrun"), SerializeField] internal float stickToWallPower = 50f;
    [Foldout("Wallrun"), SerializeField] internal float maxTime = 5f;
    [Foldout("Wallrun"), SerializeField] internal float additionalSpeed = 50f;
    [Foldout("Wallrun"), SerializeField] internal float wallSnapTime = .15f;
    [Foldout("Wallrun"), SerializeField] internal float wallSnapMaxAngle = 30f;
    [Foldout("Wallrun"), SerializeField] internal float delayForNextWall = .2f;
    [Foldout("Wallrun"), SerializeField, Layer] internal int layerWall;

    [Foldout("Player"), SerializeField] internal float playerRadius = 1f;
    [Foldout("GrindRails"), SerializeField, Layer] internal int layerRails;
    [Foldout("GrindRails"), SerializeField] internal float railsMovementOffset = 1f;
    [Foldout("GrindRails"), SerializeField] internal float railsMovementSpeed = 7f;
    [Foldout("GrindRails"), SerializeField] internal float delayForRailsState = 1f;


    [Foldout("Animation"), SerializeField,]
    private Animator animator;
    [Foldout("Animation"), SerializeField, Dropdown("Animations")]
    internal string jumpingUp = "Jumping_Up";
    [Foldout("Animation"), SerializeField, Dropdown("Animations")]
    internal string animationJumpingDown = "Jumping_Down";
    [Foldout("Animation"), SerializeField, Dropdown("Animations")]
    internal string animationWallRun = "";
    [Foldout("Animation"), SerializeField, Dropdown("Animations")]
    internal string animationRailGrinding = "";

    [Foldout("Animation"), SerializeField, AnimatorParam("animator")]
    internal string isWallOnTheLeft;
    [Foldout("Animation"), SerializeField]
    internal string movementTree = "Movement Tree";
    [Foldout("Animation"), SerializeField]
    private const float runAnimationDefaultSpeed = 12.42302f;
    [Foldout("Animation"), SerializeField]
    private float coefficient;

    [Button("remap")]
    private void SetAnimationSpeed()
    {
        float result = 0;
        result = movementSpeed.Remap(0, runAnimationDefaultSpeed, 0, 1);
        coefficient = result;
    }
    private List<string> Animations()
    {
        List<string> animations = new List<string>();
        GameObject animator = player.GetComponentInChildren<Animator>().gameObject;
        AnimationClip[] clips = AnimationUtility.GetAnimationClips(animator);
        animations = clips.Select(t => t.name).ToList();
        return animations;
    }

    void OnValidate()
    {
        playerRadius = player.GetComponentInChildren<CapsuleCollider>().radius;
        animator = player.GetComponentInChildren<Animator>();
        jumpUpForce = Mathf.Sqrt(jumpHeight * 2 * gravity);
    }
}