using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthPack : NetworkBehaviour
{
    [SerializeField] GameObject HealthPackPrefab;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {

            Health _Health = other.GetComponent<Health>();
            if (!_Health) return;
            _Health.AddHealth(50);

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();



        }


    }
}
