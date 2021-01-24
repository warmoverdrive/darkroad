using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Properties")]
    [SerializeField]
    float damageRange = 25f;
    [SerializeField]
    float DPS = 5f;
    [SerializeField]
    float spread = 1f;
    [SerializeField]
    LayerMask enemyMask;

    PhoneController phoneController;

    void Start()
    {
        phoneController = GetComponentInChildren<PhoneController>();
    }

    void Update()
	{
		if (!phoneController.IsLightOn()) return;

		ShineLight();
	}

	private void ShineLight()
	{
		// "-phoneController.transform.up" is a hacky way of shooting the ray down, 
		// since theres no field for transform.down in the API for some reason.
		RaycastHit[] hits = Physics.SphereCastAll(
			phoneController.transform.position, spread, -phoneController.transform.up, damageRange, enemyMask);
		Debug.DrawRay(phoneController.transform.position, -phoneController.transform.up * damageRange, Color.yellow);

		if (hits.Length != 0)
			foreach (var hit in hits)
			{
				hit.collider.GetComponent<EnemyHealth>().DamageEnemy(DPS * Time.deltaTime);
			}
	}
}
