using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AmmoPack : NetworkBehaviour
{
    [SerializeField] GameObject AmmoPackPrefab;

    void OnTriggerEnter2D(Collider2D other)
    {


        if (IsServer)
        {

            FiringAction _FireScript = other.GetComponent<FiringAction>();
            if (!_FireScript) return;
            _FireScript.AddAmmo(10);

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();



        }


    }
}
