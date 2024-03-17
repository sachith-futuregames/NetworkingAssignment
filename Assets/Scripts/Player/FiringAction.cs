using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FiringAction : NetworkBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] GameObject clientSingleBulletPrefab;
    [SerializeField] GameObject serverSingleBulletPrefab;
    [SerializeField] Transform bulletSpawnPoint;

    NetworkVariable<float> CoolDownTime = new NetworkVariable<float>();

    NetworkVariable<int> _Ammo = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        playerController.onFireEvent += Fire;
        if(IsServer)
        {
            _Ammo.Value = 10;
            playerController.OnRespawnEvent += OnRespawn;
        }
    }

    public void AddAmmo(int _AdditionalAmmo)
    {
        _Ammo.Value += _AdditionalAmmo; 
    }

    private void Fire(bool isShooting)
    {

        if (isShooting)
        {
            ShootLocalBullet();
        }
    }

    private void Update()
    {
        if (IsServer && CoolDownTime.Value > float.Epsilon)
        {
            CoolDownTime.Value -= Time.deltaTime;
        }
    }

    [ServerRpc]
    private void ShootBulletServerRpc()
    {
        if (_Ammo.Value <= 0 || CoolDownTime.Value > float.Epsilon)
            return;
        GameObject bullet = Instantiate(serverSingleBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
        _Ammo.Value--;
        CoolDownTime.Value = 0.5f;
        ShootBulletClientRpc();
        
    }

    [ClientRpc]
    private void ShootBulletClientRpc()
    {
        
        GameObject bullet = Instantiate(clientSingleBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
    }

    private void ShootLocalBullet()
    {
        //GameObject bullet = Instantiate(clientSingleBulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        //Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());

        ShootBulletServerRpc();
    }

    private void OnRespawn(bool Sucess)
    {
        _Ammo.Value = 10;
    }
}
