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

        RaycastHit[] hits = Physics.SphereCastAll(
            phoneController.transform.position, spread, -phoneController.transform.up, damageRange, enemyMask);
        Debug.DrawRay(phoneController.transform.position, -phoneController.transform.up * damageRange, Color.yellow);

        if (hits.Length != 0)
            foreach (var hit in hits)
                Debug.Log(hit.collider.gameObject.name);
    }
}
