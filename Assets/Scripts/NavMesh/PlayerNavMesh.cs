using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerNavMesh : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private Vector3 targetPosition;
    public void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        targetPosition = hit.point;

        if (targetPosition == null)
            return;

        navAgent.destination = targetPosition;

    }
}
