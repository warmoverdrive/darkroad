using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    float healthMax = 100f;
    [SerializeField]
    ParticleSystem fog;
    [SerializeField]
    ParticleSystem[] eyes;
    [Header("Fog Settings")]
    [SerializeField]
    float startSpeed = 1f;
    [SerializeField]
    float deathSpeed = 20f;

    public float currentHealth;
    public bool isDead = false;
    ParticleSystem.MainModule fogMain;

	private void Awake()
	{
        currentHealth = healthMax;
        fogMain = fog.main;

        if (!fog) Debug.LogError("Fog not found");
	}

	public void DamageEnemy(float damage)
	{
        if (isDead) return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0, healthMax);

        if (currentHealth == 0)
            Kill();
        else if (currentHealth < healthMax / 10)
		{
            var fogMain = fog.main;
            fogMain.startSpeed = Mathf.Lerp(deathSpeed, startSpeed, currentHealth / (healthMax / 6));
        }
	}

    void Kill()
	{
        isDead = true;
        fogMain.loop = false;
        foreach (var eye in eyes)
		{
            var main = eye.main;
            main.loop = false;
		}

        Destroy(gameObject, fogMain.startLifetime.constant + fogMain.duration);
	}
}
