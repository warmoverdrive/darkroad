using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDanger : MonoBehaviour
{
	[SerializeField]
	float dangerDetectRange = 15f;
	[SerializeField]
	float fatalRange = 1f;
	[SerializeField]
	float recoverySpeed = 1f;
	[SerializeField]
	LayerMask dangerMask;

	float currentShortestDistance;

	public bool isDead = false;

	PostProcessFX ppFX;
	Rigidbody rb;
	CapsuleCollider capsuleCollider;
	CharacterController controller;

	// Start is called before the first frame update
	void Start()
	{
		ppFX = FindObjectOfType<PostProcessFX>();
		rb = GetComponent<Rigidbody>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		controller = GetComponent<CharacterController>();
		currentShortestDistance = dangerDetectRange;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (isDead) return;

		DetectDanger();

		if (currentShortestDistance <= fatalRange)
			KillPlayer();
	}

	void KillPlayer()
	{
		isDead = true;
		capsuleCollider.enabled = true;
		controller.enabled = false;
		rb.isKinematic = false;
		FindObjectOfType<GameOverMenu>().StartCoroutine("FadeInGameOver");
	}

	private void DetectDanger()
	{
		float lastShortestDistance = currentShortestDistance;

		Collider[] hits = Physics.OverlapSphere(transform.position, dangerDetectRange, dangerMask);

		if (hits.Length != 0)
			foreach (var hit in hits)
			{
				if (Vector3.Distance(transform.position, hit.transform.position)-1 < currentShortestDistance)
					currentShortestDistance = Vector3.Distance(transform.position, hit.transform.position)-1;
			}

		if (currentShortestDistance == lastShortestDistance)
			currentShortestDistance += recoverySpeed * Time.deltaTime;
		currentShortestDistance = Mathf.Clamp(currentShortestDistance, 0, dangerDetectRange);

		ppFX.LerpDanger(currentShortestDistance / dangerDetectRange);
	}
}
