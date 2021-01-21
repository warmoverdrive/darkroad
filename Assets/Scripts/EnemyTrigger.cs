using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    [SerializeField]
    float triggerRange = 100f;
	[SerializeField]
	float triggerTimer = 0.5f;
    [SerializeField]
    LayerMask playerMask;

	float currentTimer = 0f;
    EnemyMovement movement;

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
    }

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRange);
	}

	void Update()
	{
		currentTimer += Time.deltaTime;

		if (currentTimer >= triggerTimer)
		{
			PlayerSearch();
			currentTimer = 0;
		}
	}

	private void PlayerSearch()
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, triggerRange, playerMask);

		if (hits.Length != 0)
			foreach (var hit in hits)
				movement.SetTarget(hit.transform);
		else movement.SetTarget(null);
	}
}
