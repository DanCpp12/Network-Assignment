using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
	[SerializeField] NetworkManager netwokManager;
	[SerializeField] GameObject spawnManager;
	
	public void Host()
	{
		netwokManager.StartHost();
	}
	public void Join()
	{
		netwokManager.StartClient();
	}
	public void Leave()
	{
		netwokManager.DisconnectClient(netwokManager.LocalClientId);
		netwokManager.Shutdown();
	}
	public void Quit()
	{
		Application.Quit();
	}
	public void Respawn()
	{
		SpawnRPC(netwokManager.LocalClientId);
	}

	[Rpc(SendTo.Server)]
	private void SpawnRPC(ulong ClientId)
	{
		spawnManager.GetComponent<SpawnManager>().OnSpawnPlayer(ClientId);
	}
}
