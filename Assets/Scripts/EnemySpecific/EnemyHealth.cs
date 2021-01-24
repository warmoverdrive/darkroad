using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages enemy health, handles death VFX for enemies, and the kill routines.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    // Config Parameters ---------------
    [SerializeField]
    float healthMax = 100f;
    [SerializeField]
    ParticleSystem fog;
    [SerializeField]
    ParticleSystem[] eyes;
    [Header("Fog Settings")]
    [SerializeField]
    float dyingThresholdPercent = 0.2f;
    [SerializeField]
    float startSpeed = 1f;
    [SerializeField]
    float deathSpeed = 20f;

    // Internal Variables --------------
    public float currentHealth;
    public bool isDead = false;

    // Component References ------------
    ParticleSystem.MainModule fogMain;

	private void Awake()
	{
        currentHealth = healthMax;
        fogMain = fog.main;

        if (!fog) Debug.LogError("Fog not found");
	}

    /// <summary>
    /// <para>Subtracts damage from Enemy Health, clamping values between 0 and Max Health.</para>
    /// <para>Additionally checks for death at 0, and if health is below Threshold% of Max Health,
    /// Fog particle start speed linearly interpolates between normal and Death Speed for
    /// exploding death effect.</para>
    /// </summary>
    /// <param name="damage">Damage value to remove from enemy health</param>
	public void DamageEnemy(float damage)
	{
        if (isDead) return;

        currentHealth -= damage;

        currentHealth = Mathf.Clamp(currentHealth, 0, healthMax);

        if (currentHealth == 0)
            Kill();
        else if (currentHealth < healthMax * dyingThresholdPercent)
		{
            var fogMain = fog.main;
            fogMain.startSpeed = Mathf.Lerp(
                deathSpeed, startSpeed, currentHealth / (healthMax * dyingThresholdPercent));
        }
	}

    /// <summary>
    /// <para>Triggers death routine for enemy.</para>
    /// <para>Sets looping to false for all particle systems, waits until all particles 
    /// naturally die, then removes the game object.</para>
    /// </summary>
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
