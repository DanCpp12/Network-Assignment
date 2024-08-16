using Unity.Netcode;
using UnityEngine;

public class Asteroid : NetworkBehaviour
{
	public float speed;
	public float damage;
	public float LifeSpan = 15;

    Vector2 point;
	Vector2 dir;

    GameObject HitActor;

    void Start()
	{
		point = Random.insideUnitCircle * 3;
		dir = new Vector2(point.x - transform.position.x, point.y - transform.position.y);
        dir.Normalize();
		gameObject.GetComponent<Rigidbody2D>().velocity = dir * speed;
	}
    private void Update()
    {
        if (LifeSpan <= 0)
        {
            KillRPC();
        }
        LifeSpan -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HitActor = collision.gameObject;
        if (HitActor.GetComponent<Asteroid>()) { return; }
        if (true)
        {
            if (HitActor.GetComponent<HealthManager>() != null)
            {
                Damage(damage);
            }
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
