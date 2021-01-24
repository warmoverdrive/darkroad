using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Player Flashlight attack and applying damage to enemies
/// it contacts.
/// </summary>
public class PlayerAttack : MonoBehaviour
{
	// Config Parameters ---------------
	[Header("Attack Properties")]
    [SerializeField]
    float damageRange = 25f;
    [SerializeField]
    float DPS = 5f;
    [SerializeField]
    float spread = 1f;
    [SerializeField]
    LayerMask enemyMask;

	// Component References ------------
	PhoneController phoneController;
	PlayerDanger danger;

    void Start()
    {
        phoneController = GetComponentInChildren<PhoneController>();
		danger = GetComponent<PlayerDanger>();
    }

	// If the light's off or player is dead, do nothing,
	// otherwise make an attack check
    void Update()
	{
		if (!phoneController.IsLightOn() || danger.isDead) return;

		ShineLight();
	}

	/// <summary>
	/// <para>Cases a sphere damageRange distance from the Phone Controllers transform down, 
	/// with a Layer Mask for filtering results to only enemies. For each enemy hit, 
	/// apply damage using the targets DamageEnemy method, passing in DPS (damage per second) 
	/// multiplied by delta time.</para>
	/// </summary>
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
