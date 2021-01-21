using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessFX : MonoBehaviour
{
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

    Volume postFX;
    DepthOfField depthOfField;
    ChromaticAberration chromAbb;
    SplitToning splitTone;

    // Start is called before the first frame update
    void Start()
    {
        postFX = GetComponent<Volume>();

        postFX.profile.TryGet(out depthOfField);
        postFX.profile.TryGet(out chromAbb);
        postFX.profile.TryGet(out splitTone);
    }

    /// <summary>
    /// <para>Lerps from "Danger" VFX state *towards* "Base" VFX state</para>
    /// <para>Higher 't' values balance more into the Base VFX state.</para>
    /// </summary>
    /// <param name="t">time value along Lerp</param>
    public void LerpDanger(float t)
	{
        chromAbb.intensity.value = Mathf.Lerp(chromAbbDanger, chromAbbBase, t);
        splitTone.balance.value = Mathf.Lerp(splitToneDanger, splitToneBase, t);
	}

    public void LerpFocalLength(float t)
	{
        depthOfField.focalLength.value = Mathf.Lerp(focalLengthRest, focalLengthMenu, t);
	}
}
