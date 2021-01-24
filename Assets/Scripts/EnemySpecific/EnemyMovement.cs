using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles enemy movement towards targets.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    // Config Parameters ---------------
    [SerializeField]
    Transform eyes;
    [SerializeField]
    Transform target;
    [SerializeField]
    float moveSpeed = 5f;

    // Componenet References -----------
    Rigidbody rb;
    EnemyHealth health;
    PlayerDanger playerDanger;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<EnemyHealth>();
    }

    /// <summary>
    /// Assign a new target for this enemy.
    /// <para>New target will be assigned to this enemy, and their
    /// Danger component will be cached for death checks.</para>
    /// </summary>
    /// <param name="newTarget">Transform of the target Player</param>
    public void SetTarget(Transform newTarget)
    {
        if (newTarget.gameObject.GetComponent<PlayerDanger>())
            playerDanger = newTarget.GetComponent<PlayerDanger>();
        target = newTarget;
    }

    /// <summary>
    /// Checks if this enemy has a target and returns the result.
    /// </summary>
    /// <returns>true if this enemy has a target, false if target is null</returns>
    public bool HasTarget()
	{
        if (target) return true;
        else return false;
	}

    void Update()
	{
        if (health.isDead) return;
        if (target)
		    MoveTowardsTarget();
	}

    /// <summary>
    /// Moves this enemy toward the target by moveSpeed if the player is not dead. Additionally 
    /// rotates the eyes towards the player.
    /// </summary>
	private void MoveTowardsTarget()
	{
        if (playerDanger == null || playerDanger.isDead == true)
            return;
	    rb.MovePosition(Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime));
		eyes.LookAt(target);
	}
}
