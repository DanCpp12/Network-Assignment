using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RespawnUI : NetworkBehaviour
{
	[SerializeField] NetworkManager netwokManager;
	[SerializeField] GameObject panel;
	ulong PlayerID;
	NetworkObject LocalPlayer;

	private void Start()
	{
		PlayerID = netwokManager.LocalClientId;
	}
	private void Update()
	{
		if (PlayerID != netwokManager.LocalClientId || LocalPlayer != netwokManager.SpawnManager.GetPlayerNetworkObject(PlayerID))
		{
			PlayerID = netwokManager.LocalClientId;
			LocalPlayer = netwokManager.SpawnManager.GetPlayerNetworkObject(PlayerID);
		}

		if (LocalPlayer.IsLocalPlayer)
		{
			if (!LocalPlayer.gameObject.activeSelf)
			{
				panel.SetActive(true);
			}
		}
	}
}
