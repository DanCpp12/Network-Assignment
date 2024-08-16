using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthManager : NetworkBehaviour
{
	public float Max_Health;
	[SerializeField] NetworkVariable<float> Health = new NetworkVariable<float>(0);

	private void Start()
	{
        ResetHealthRPC();
	}
    [Rpc(SendTo.Server)]
    public void ResetHealthRPC()
	{
		Health.Value = Max_Health;
	}

	public void Damage(float damage)
	{
		DamageRPC(damage);
	}
    [Rpc(SendTo.Server)]
	private void DamageRPC(float damage)
	{
        Health.Value -= damage;
        if (Health.Value <= 0)
        {
            KillRPC();
        }
    }

    [Rpc(SendTo.Everyone)]
	public void KillRPC()
	{
		NetworkObject net = gameObject.GetComponent<NetworkObject>();
		if (net.IsPlayerObject)
		{
            net.gameObject.SetActive(false);
			net.gameObject.GetComponent<Player>().bCanShoot = false;
        }
		else
		{
			net.Despawn();
		}
	}
    [Rpc(SendTo.Everyone)]
	public void ReviveRPC()
	{
        NetworkObject net = gameObject.GetComponent<NetworkObject>();
        if (net.IsPlayerObject)
        {
            net.gameObject.SetActive(true);
			ResetHealthRPC();
        }
    }
}
