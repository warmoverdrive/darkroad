using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Enemy Player Detection and target acquisition.
/// </summary>
public class EnemyTrigger : MonoBehaviour
{
	// Config Parameters ---------------
	[SerializeField]
    float triggerRange = 100f;
	[SerializeField]
	float triggerTimer = 0.5f;
    [SerializeField]
    LayerMask playerMask;

	// Internal Variables --------------
	float currentTimer = 0f;

	// Component References ------------
	EnemyMovement movement;

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
    }

	// Draws a red sphere around the enemies trigger range for easy placement
	// in the editor
	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRange);
	}

	// FixedUpdate is used here to run a timer between Player Search calls
	// to try and keep casts per frame slightly lower.
	// TODO, implement a randomize feature to offset the Search calls,
	// possibly by multiplying delta time by a random value
	void FixedUpdate()
	{
		currentTimer += Time.fixedDeltaTime;

		if (currentTimer >= triggerTimer)
		{
			PlayerSearch();
			currentTimer = 0;
		}
	}

	/// <summary>
	/// <para>Casts an overlap sphere from the Enemy's location, applying a mask to filter
	/// out all but the player. It then checks if this enemy already has a target, and
	/// if not, assigns a new hit to the target. If nothing is hit by the overlap sphere,
	/// then the target is set to null.</para>
	/// </summary>
	private void PlayerSearch()
	{
		Collider[] hits = Physics.OverlapSphere(transform.position, triggerRange, playerMask);

		if (hits.Length != 0)
		{
			foreach (var hit in hits)
				if(!movement.HasTarget())
					movement.SetTarget(hit.transform);
		}
		else movement.SetTarget(null);
	}
}
