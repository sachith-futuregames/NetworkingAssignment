using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] PlayerController controller;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();

    [HideInInspector] public NetworkVariable<int> ShieldHit = new NetworkVariable<int>(0);

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        currentHealth.Value = 100;
        controller.OnRespawnEvent += OnRespawn;
    }

    public override void OnNetworkDespawn()
    {
        controller.OnRespawnEvent -= OnRespawn;
        base.OnNetworkDespawn();

    }


    public void TakeDamage(int damage){
        if(ShieldHit.Value > 0)
        {
            ShieldHit.Value--;
            return;
        }
        damage = damage<0? damage:-damage;
        currentHealth.Value += damage;
        if(currentHealth.Value <= 0) 
        {
            controller.HandleDie();
        }
    }

    public void AddHealth(int Health)
    {
        currentHealth.Value = Mathf.Clamp((currentHealth.Value + Health), 0, 100);
    }

    public void AddShield()
    {
        ShieldHit.Value = 2;
    }

    private void OnRespawn(bool _Success)
    {
        currentHealth.Value = 100;
    }

}
