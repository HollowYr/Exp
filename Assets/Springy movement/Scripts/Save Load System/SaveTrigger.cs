#define DEBUG
using UnityEngine;

public class SaveTrigger : ImprovedMonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<PlayerStateAgent>(out PlayerStateAgent agent)) return;
        GameData.Instance.Save();
    }
}

