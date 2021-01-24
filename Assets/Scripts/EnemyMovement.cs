using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField]
    Transform eyes;
    [SerializeField]
    Transform target;
    [SerializeField]
    float moveSpeed = 5f;
    Rigidbody rb;
    EnemyHealth health;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<EnemyHealth>();
    }

    public void SetTarget(Transform newTarget) => target = newTarget;

    public bool HasTarget()
	{
        if (target) return true;
        else return false;
	}

    // Update is called once per frame
    void Update()
	{
        if (health.isDead) return;
        if (target)
		    MoveTowardsTarget();
	}

	private void MoveTowardsTarget()
	{
	    rb.MovePosition(Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime));
		eyes.LookAt(target);
	}
}
