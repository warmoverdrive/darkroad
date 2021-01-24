using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles Game Over GUI Elements.
/// </summary>
public class GameOverMenu : MonoBehaviour
{
    // Config Params ------------------
    [Header("Game Over Transition Settings")]
    [SerializeField]
    float transitionTime = 3f;
    [SerializeField]
    Image panel;
    [SerializeField]
    TextMeshProUGUI text;

    // Internal Vars ------------------
    float currentTransitionTime = 0f;
    float textTransitionTime = 0f;

    /// <summary>
    /// <para>Fades in the Game Over screen elements over time.</para>
    /// <para>The text object is offset to start halfway into the panel fade-in, from the textAlpha var.</para>
    /// </summary>
    /// <returns>IEnumerator</returns>
    public IEnumerator FadeInGameOver()
	{
        do
        {
            float panelAlpha = Mathf.Lerp(0f, 1f, currentTransitionTime += Time.deltaTime / transitionTime);
            float textAlpha = (currentTransitionTime < transitionTime / 2) ?
                0f : Mathf.Lerp(0f, 1f, textTransitionTime += Time.deltaTime / (transitionTime / 2));
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panelAlpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, textAlpha);
            yield return new WaitForEndOfFrame();
        } 
        while (currentTransitionTime < transitionTime);
	}
}
