using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpeedBuffPack : NetworkBehaviour
{
    [SerializeField] GameObject SpeedBuffPackPrefab;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (IsServer)
        {

            PlayerController _PC = other.GetComponent<PlayerController>();
            if (!_PC) return;
            _PC._SpeedBuff();

            NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
            networkObject.Despawn();

        }


    }
}
