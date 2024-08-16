using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
	[SerializeField] InputReader inputReader;
	[SerializeField] GameObject Bullet;

	[SerializeField] float Speed = 20;

	NetworkVariable<Vector2> moveInput = new NetworkVariable<Vector2>();

	Vector2 dir;
    public bool bCanShoot;


	void Start()
	{
		if (inputReader != null && IsLocalPlayer)
		{
			bCanShoot = true;
			inputReader.MoveEvent += OnMove;
			inputReader.ShootEvent += OnShoot;
		}
	}
	void Update()
	{
		if (IsServer)
		{
			transform.position += (Vector3)moveInput.Value * Time.deltaTime * Speed;
			transform.up = dir;
			Teleport();
		}
		if (IsLocalPlayer)
		{
			Rotate();
		}
	}

	private void OnMove(Vector2 input)
	{
		MoveRPC(input);
	}
	[Rpc(SendTo.Server)]
	private void MoveRPC(Vector2 data)
	{
		moveInput.Value = data;
	}

	private void Rotate()
	{
		Vector3 mousePosition = Input.mousePosition;
		mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
		if (MouseOverViewport())
		{
			RotateRPC(mousePosition);
		}
	}
	[Rpc(SendTo.Server)]
	private void RotateRPC(Vector3 data)
	{
		dir = new Vector2(data.x - transform.position.x, data.y - transform.position.y);
	}

	private void OnShoot()
	{
		if (bCanShoot)
		{
			ShootRPC();
		}
	}
	[Rpc(SendTo.Server)]
	private void ShootRPC()
	{
		NetworkObject ob = Instantiate(Bullet, transform.position, Quaternion.identity).GetComponent<NetworkObject>();
		ob.Spawn(true);
		ob.GetComponent<Bullet>().dir = dir;
	}

	private void Teleport()
	{
		if (transform.position.x < -10 - (transform.lossyScale.y))
		{
			transform.position =  new Vector3(9 + (transform.lossyScale.y / 2), transform.position.y, transform.position.z);
		}
		if (transform.position.x > 10 + (transform.lossyScale.y))
		{
			transform.position = new Vector3(-9 - (transform.lossyScale.y / 2), transform.position.y, transform.position.z);
		}
		if (transform.position.y < -6 - (transform.lossyScale.y))
		{
			transform.position = new Vector3(transform.position.x, 5 + (transform.lossyScale.y / 2), transform.position.z);
		}
		if (transform.position.y > 6 + (transform.lossyScale.y))
		{
			transform.position = new Vector3(transform.position.x, -5 - (transform.lossyScale.y / 2), transform.position.z);
		}
	}

	bool MouseOverViewport()
	{
		if (!Input.mousePresent) { return true; }

		Vector3 main_mou = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		return Camera.main.rect.Contains(main_mou);
	}
}
