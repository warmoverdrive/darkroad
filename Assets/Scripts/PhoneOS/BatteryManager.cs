using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles GUI battery elements for the PhoneOS UI.
/// </summary>
public class BatteryManager : MonoBehaviour
{
	public enum BatteryState { high, midHigh, mid, midLow, low, dead };

	// Config Params ----------------------
	[SerializeField]
	Image lowImage, midLowImage, midImage, midHighImage, highImage;
	[SerializeField]
	Color normalBatteryColor = Color.white;
	[SerializeField]
	Color lowBatteryColor = Color.red;
	[SerializeField]
	float lowBattFlickerSpeed = 0.5f;
	[SerializeField]
	float deadFlickerSpeed = 0.25f;

	// Internal Vars ----------------------
	BatteryState batteryState = BatteryState.high;
	bool flickerOn = true;
	float currentFlickerTime = 0f;

	// Component References ---------------
	Image thisImage;

	private void Start()
	{
		thisImage = GetComponent<Image>();
	}

	// Checks for a low battery to call the Low Power Cycle
	void Update()
	{
		if (batteryState == BatteryState.low || batteryState == BatteryState.dead)
			LowPowerCycle();
	}

	/// <summary>
	/// Handles the GUI behavior for the battery icon based off of the input BatteryState.
	/// <para>Only triggers a change if the passed state is different from the current state.</para>
	/// <para>Review note -> considering all of the different pieces that need to be enabled
	/// or disabled per state, I couldn't think of a prettier way to go about coding this logic.
	/// It works just fine, and for such a focused script it'll do.</para>
	/// </summary>
	/// <param name="state">Target battery state</param>
	public void ChangeBatteryState(BatteryState state)
	{
		if (batteryState != state)
		{
			switch (state)
			{
				case BatteryState.high:
					thisImage.enabled = true;
					highImage.gameObject.SetActive(true);
					midHighImage.gameObject.SetActive(true);
					midImage.gameObject.SetActive(true);
					midLowImage.gameObject.SetActive(true);
					lowImage.gameObject.SetActive(true);
					thisImage.color = normalBatteryColor;
					lowImage.color = normalBatteryColor;
					break;
				case BatteryState.midHigh:
					thisImage.enabled = true;
					highImage.gameObject.SetActive(false);
					midHighImage.gameObject.SetActive(true);
					midImage.gameObject.SetActive(true);
					midLowImage.gameObject.SetActive(true);
					lowImage.gameObject.SetActive(true);
					thisImage.color = normalBatteryColor;
					lowImage.color = normalBatteryColor;
					break;
				case BatteryState.mid:
					thisImage.enabled = true;
					highImage.gameObject.SetActive(false);
					midHighImage.gameObject.SetActive(false);
					midImage.gameObject.SetActive(true);
					midLowImage.gameObject.SetActive(true);
					lowImage.gameObject.SetActive(true);
					thisImage.color = normalBatteryColor;
					lowImage.color = normalBatteryColor;
					break;
				case BatteryState.midLow:
					thisImage.enabled = true;
					highImage.gameObject.SetActive(false);
					midHighImage.gameObject.SetActive(false);
					midImage.gameObject.SetActive(false);
					midLowImage.gameObject.SetActive(true);
					lowImage.gameObject.SetActive(true);
					thisImage.color = normalBatteryColor;
					lowImage.color = normalBatteryColor;
					break;
				case BatteryState.low:
					thisImage.enabled = true;
					highImage.gameObject.SetActive(false);
					midHighImage.gameObject.SetActive(false);
					midImage.gameObject.SetActive(false);
					midLowImage.gameObject.SetActive(false);
					lowImage.gameObject.SetActive(true);
					thisImage.color = lowBatteryColor;
					lowImage.color = lowBatteryColor;
					break;
				case BatteryState.dead:
					thisImage.enabled = true;
					highImage.gameObject.SetActive(false);
					midHighImage.gameObject.SetActive(false);
					midImage.gameObject.SetActive(false);
					midLowImage.gameObject.SetActive(false);
					lowImage.gameObject.SetActive(false);
					thisImage.color = lowBatteryColor;
					lowImage.color = lowBatteryColor;
					break;
			}
			batteryState = state;
		}
	}

	/// <summary>
	/// Handles the Low Power animation logic based on which low power state is active.
	/// <para>Both states make use of the same timer variables, but the flicker speeds are
	/// different per state. Speeds are configurable in the member variables.</para>
	/// </summary>
	private void LowPowerCycle()
	{
		if (currentFlickerTime > lowBattFlickerSpeed && batteryState == BatteryState.low)
		{
			flickerOn = !flickerOn;
			lowImage.gameObject.SetActive(!lowImage.gameObject.activeSelf);
			currentFlickerTime = 0;
		}
		else if (currentFlickerTime > deadFlickerSpeed && batteryState == BatteryState.dead)
		{
			flickerOn = !flickerOn;
			thisImage.enabled = !thisImage.enabled;
			currentFlickerTime = 0;
		}
		currentFlickerTime += Time.deltaTime;
	}
}
