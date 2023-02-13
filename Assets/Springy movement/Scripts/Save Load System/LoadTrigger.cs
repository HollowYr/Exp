#define DEBUG
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadTrigger : ImprovedMonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerStateAgent>(out PlayerStateAgent agent)) return;
        agent.transform.position = GameData.Instance.Load().GetPlayerPosition();
    }
}

