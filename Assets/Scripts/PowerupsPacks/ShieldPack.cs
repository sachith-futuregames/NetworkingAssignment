using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShieldPack : NetworkBehaviour
{
    [SerializeField] GameObject ShieldPackPrefab;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {

            Health _Health = other.GetComponent<Health>();
            if (!_Health) return;
            _Health.AddShield();

            int xPosition = Random.Range(-5, 5);
            int yPosition = Random.Range(-3, 3);

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();



        }


    }
}
