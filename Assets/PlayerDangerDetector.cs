using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDangerDetector : MonoBehaviour
{
	[SerializeField]
	float dangerDetectRange = 15f;
	[SerializeField]
	LayerMask dangerMask;

	PostProcessFX ppFX;

	// Start is called before the first frame update
	void Start()
	{
		ppFX = FindObjectOfType<PostProcessFX>();
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		DetectDanger();
	}

	private void DetectDanger()
	{
		float shortestDistance = dangerDetectRange;

		Collider[] hits = Physics.OverlapSphere(transform.position, dangerDetectRange, dangerMask);

		if (hits.Length != 0)
			foreach (var hit in hits)
			{
				if (Vector3.Distance(transform.position, hit.transform.position)-1 < shortestDistance)
					shortestDistance = Vector3.Distance(transform.position, hit.transform.position)-1;
			}

		ppFX.LerpDanger(shortestDistance / dangerDetectRange);
	}
}
