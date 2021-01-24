using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Danger Detection for player, influencing Post Processing FX
/// and triggering death routines.
/// </summary>
public class PlayerDanger : MonoBehaviour
{
	// Config Params ------------------
	[SerializeField]
	float dangerDetectRange = 15f;
	[SerializeField]
	float fatalRange = 1f;
	[SerializeField]
	float recoverySpeed = 1f;
	[SerializeField]
	LayerMask dangerMask;

	// Internal Vars ------------------
	float currentShortestDistance;
	public bool isDead = false;

	// Component References -----------
	PostProcessFX ppFX;
	Rigidbody rb;
	CapsuleCollider capsuleCollider;
	CharacterController controller;

	void Start()
	{
		ppFX = FindObjectOfType<PostProcessFX>();
		rb = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		controller = GetComponent<CharacterController>();
		currentShortestDistance = dangerDetectRange;
	}

	// If player is dead, return. Else, detect danger. if an enemy is within 
	// fatal range, kill player.
	void FixedUpdate()
	{
		if (isDead) 
			return;

		DetectDanger();

		if (currentShortestDistance <= fatalRange)
			KillPlayer();
	}

	/// <summary>
	/// <para>Triggers player death. Toggles colliders and rigidbodies, allowing the 
	/// player to fall. Then starts the Game Over UI fade-in co-routine.</para>
	/// </summary>
	void KillPlayer()
	{
		isDead = true;
		capsuleCollider.enabled = true;
		controller.enabled = false;
		rb.isKinematic = false;
		FindObjectOfType<GameOverMenu>().StartCoroutine("FadeInGameOver");
	}

	/// <summary>
	/// <para>Casts an overlap sphere from the Players position applying the 
	/// Enemy Mask to skip irrelevant hits. It then checks the results searching 
	/// for the closest enemy, updating the Current Shortest Distance member
	/// for the new closest enemy, or increments back up to max (Detect Range).</para>
	/// <para>Ignores dead enemies.</para>
	/// </summary>
	private void DetectDanger()
	{
		float lastShortestDistance = currentShortestDistance;

		Collider[] hits = Physics.OverlapSphere(transform.position, dangerDetectRange, dangerMask);

		if (hits.Length != 0)
			foreach (var hit in hits)
			{
				if (hit.GetComponent<EnemyHealth>().isDead)
					return;
				if (Vector3.Distance(transform.position, hit.transform.position)-1 < currentShortestDistance)
					currentShortestDistance = Vector3.Distance(transform.position, hit.transform.position)-1;
			}

		if (currentShortestDistance == lastShortestDistance)
			currentShortestDistance += recoverySpeed * Time.deltaTime;
		currentShortestDistance = Mathf.Clamp(currentShortestDistance, 0, dangerDetectRange);

		ppFX.LerpDanger(currentShortestDistance / dangerDetectRange);
	}
}
