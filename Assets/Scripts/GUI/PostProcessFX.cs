using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Utility for manipulating Post Processing FX routines for dynamic VFX.
/// <list type="bullet">
/// <item>
/// <term>Focal Length</term>
/// <description>LerpFocalLength interpolates Focal length settings
/// to provide depth of view effects relative to phone placement.</description>
/// </item>
/// <item>
/// <term>Danger Effects</term>
/// <description>LerpDanger interpolates between configured presets for "safe"
/// and "danger" Post Processing states.</description>
/// </item>
/// <item>More to Come..?</item>
/// </list>
/// </summary>
public class PostProcessFX : MonoBehaviour
{
    // Config Params ------------------
    [Header("Depth of Field Controls")]
    [SerializeField]
    int focalLengthRest = 1;
    [SerializeField]
    int focalLengthMenu = 40;
    [Header("Enemy Proximity Effect Controls")]
    [SerializeField]
    int splitToneBase = 100;
    [SerializeField]
    int splitToneDanger = 0;
    [SerializeField]
    float chromAbbBase = 0f;
    [SerializeField]
    float chromAbbDanger = 1f;

    // Component References -----------
    Volume postFX;
    DepthOfField depthOfField;
    ChromaticAberration chromAbb;
    SplitToning splitTone;

    void Start()
    {
        postFX = GetComponent<Volume>();

        postFX.profile.TryGet(out depthOfField);
        postFX.profile.TryGet(out chromAbb);
        postFX.profile.TryGet(out splitTone);
    }

    /// <summary>
    /// <para>Linearly interpolates from "Danger" VFX state *towards* "Base" VFX state</para>
    /// <para>Higher 't' values balance more into the Base VFX state.</para>
    /// </summary>
    /// <param name="t">time value along Lerp</param>
    public void LerpDanger(float t)
	{
        chromAbb.intensity.value = Mathf.Lerp(chromAbbDanger, chromAbbBase, t);
        splitTone.balance.value = Mathf.Lerp(splitToneDanger, splitToneBase, t);
	}

    /// <summary>
    /// <para>Linearly interpolates between resting focal length and menu focal length by t.</para>
    /// </summary>
    /// <param name="t">time value along Lerp</param>
    public void LerpFocalLength(float t)
	{
        depthOfField.focalLength.value = Mathf.Lerp(focalLengthRest, focalLengthMenu, t);
	}
}
