using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatteryManager : MonoBehaviour
{
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

	public enum BatteryState { high, midHigh, mid, midLow, low, dead };

	// Internal Vars ----------------------
	Image thisImage;
	BatteryState batteryState = BatteryState.high;
	bool flickerOn = true;
	float currentFlickerTime = 0f;

	private void Start()
	{
		thisImage = GetComponent<Image>();
	}

	void Update()
	{
		if (batteryState == BatteryState.low || batteryState == BatteryState.dead)
			LowPowerCycle();
	}

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
