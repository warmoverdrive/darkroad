using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDangerDetector : MonoBehaviour
{
	[SerializeField]
	float dangerDetectRange = 15f;
	[SerializeField]
	float recoverySpeed = 1f;
	[SerializeField]
	LayerMask dangerMask;

	float currentShortestDistance;

	PostProcessFX ppFX;

	// Start is called before the first frame update
	void Start()
	{
		ppFX = FindObjectOfType<PostProcessFX>();
		currentShortestDistance = dangerDetectRange;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		DetectDanger();
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
