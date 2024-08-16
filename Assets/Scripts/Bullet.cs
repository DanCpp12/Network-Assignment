using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
	public float Speed;
	public float LifeSpan = 3;
	public float damage = 1;
	bool bCanHit = false;
	public Vector2 dir;
	GameObject HitActor;

    private void Start()
    {
        dir.Normalize();
		gameObject.GetComponent<Rigidbody2D>().velocity = dir * Speed;
    }

    // Update is called once per frame
    void Update()
	{
		if (LifeSpan <= 0)
		{
			KillRPC();
		}
		LifeSpan -= Time.deltaTime;
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		bCanHit = true;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (bCanHit)
		{
			HitActor = collision.gameObject;
			if (HitActor.GetComponent<HealthManager>() != null)
			{
				Damage(damage);
			}
			KillRPC();
		}
	}
	private void Damage(float Value)
	{
        HitActor.GetComponent<HealthManager>().Damage(Value);
    }
    [Rpc(SendTo.Server)]
    private void KillRPC()
    {
        Destroy(gameObject);
    }
}
