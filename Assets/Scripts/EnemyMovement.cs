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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetTarget(Transform newTarget) => target = newTarget;

    // Update is called once per frame
    void Update()
	{
        if (target)
		    MoveTowardsTarget();
	}

	private void MoveTowardsTarget()
	{
	    rb.MovePosition(Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime));
		eyes.LookAt(target);
	}
}
