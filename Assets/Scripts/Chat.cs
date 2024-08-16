using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Chat : NetworkBehaviour
{
	[SerializeField] InputReader inputReader;
    [SerializeField] NetworkManager netwokManager;

    [SerializeField] TMP_InputField inputField;
	[SerializeField] TextMeshProUGUI textField;

	FixedString128Bytes message;

	List<string> MessageList = new List<string>();

	void Start()
	{
		if (inputReader != null)
		{
			inputReader.SendEvent += OnSend;
		}
	}

	private void OnSend()
	{
        message = new(inputField.text);
		if (message[0] == '/')
		{
			if (IsHost && message[1] == '/' || IsServer && message[1] == '/')
            {
				CommandRPC(message);
			}
			else
			{
				LocalCommand(message);
			}
		}
		else
		{
			SubmittMessageRPC(message);
		}
		inputField.text = "";
	}
	[Rpc(SendTo.Server)]
	private void SubmittMessageRPC(FixedString128Bytes message)
	{
		UpdateMessageRPC(message);
        Debug.Log("message sent to server: " + message);
    }
    [Rpc(SendTo.Everyone)]
	private void UpdateMessageRPC(FixedString128Bytes message) 
	{
		MessageList.Add(message.ToString());
		textField.text = "";
		for (int i = 0; i <= MessageList.Count - 1; i++)
		{
			if (MessageList.Count - 1 == i)
			{
				textField.text += MessageList[i].ToString();
            }
			else
			{
				textField.text += MessageList[i].ToString() + "\n";
			}
		}
		if (MessageList.Count == 23)
		{
			MessageList.RemoveAt(0);
		}
        Debug.Log("message recived for clients: " + message);
    }

    private void LocalCommand(FixedString128Bytes Command)
	{
        if (Command == "/cls")
        {
            MessageList.Clear();
            textField.text = "";
        }
		else if (Command == "/kill")
		{
            NetworkObject net = netwokManager.SpawnManager.GetPlayerNetworkObject(netwokManager.LocalClientId);
			net.gameObject.GetComponent<HealthManager>().KillRPC();
			Debug.Log("kill: " + net.gameObject);
        }
    }

    [Rpc(SendTo.Server)]
    private void SubmittCommandRPC(FixedString128Bytes Command)
	{
		CommandRPC(Command);
	}
    [Rpc(SendTo.Everyone)]
    private void CommandRPC(FixedString128Bytes Command)
	{
        if (Command == "//cls")
		{
			MessageList.Clear();
            textField.text = "";
        }
		else if (Command == "//kill all")
		{
			for (int i = 0; i <= netwokManager.ConnectedClientsIds.Count - 1; i++)
			{
                NetworkObject net = netwokManager.SpawnManager.GetPlayerNetworkObject(netwokManager.ConnectedClientsIds[i]);
                net.gameObject.GetComponent<HealthManager>().KillRPC();
            }

        }
    }
}
