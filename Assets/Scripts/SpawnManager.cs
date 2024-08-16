using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
	[SerializeField] NetworkManager netwokManager;
	[SerializeField] GameObject AsteroidPrefab;
	public float AsteroidSpawnTime;
	float Timer;
	System.Random rand = new System.Random();

    private void Update()
    {
		if (IsHost || IsServer)
		{
			if (Timer >= AsteroidSpawnTime)
			{
				SpawnAsteroids();
				Timer = 0;
			}
			else { Timer += Time.deltaTime; }
		}
    }
    public void OnSpawnPlayer(ulong ClientId)
	{
		NetworkObject net = netwokManager.SpawnManager.GetPlayerNetworkObject(ClientId);
		SpawnPlayerRPC(ClientId);
		net.gameObject.GetComponent<HealthManager>().ReviveRPC();
		net.gameObject.GetComponent<Player>().bCanShoot = true;
	}
	[Rpc(SendTo.Everyone)]
	private void SpawnPlayerRPC(ulong ClientId)
	{
		NetworkObject net = netwokManager.SpawnManager.GetPlayerNetworkObject(ClientId);
		net.gameObject.transform.position = Vector3.zero;
	}

	private void SpawnAsteroids()
	{
		NetworkObject ob = Instantiate(AsteroidPrefab).GetComponent<NetworkObject>();
		ob.transform.position = random_point_on_circle(10);
		ob.Spawn(true);
	}


	Vector2 random_point_on_circle(float radius)
	{
		float angle = rand.Next(360);
		float value = angle * (MathF.PI / 180f);

		return new Vector2(MathF.Cos(value), MathF.Sin(value)) * radius;
		
		/*float angle = Random.rotation.eulerAngles.z * Math.TAU;
		return new Vector2(Math.Sin(angle), Math.Cos(angle)) * radius;*/
	}
}
